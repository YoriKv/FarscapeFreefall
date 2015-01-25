using UnityEngine;

public class Sound_Manager:MonoBehaviour {
	// Audio source
	private AudioSource _musicSrc;
	private AudioSource _musicSrcAlt;
	private AudioSource _musicSrcTemp; // Temp, used for swapping main and alt music source
	private AudioSource _effectSrc;
	private AudioSource[] _loopEffectSrcs;
	private int[] _loopEffectsChannelInUse;

	private const int LOOP_EFFECTS_CHANNELS = 7; // Increase this number if the number of looping sound effects goes up

	private const float ONE_SHOT_DELAY = 0.15f;

	// One Shot sound delay timer
	private string oneShotLastPlayed = "";
	private float oneShotLastPlayedTime = 0f;

	// Instance variable
	private static Sound_Manager _instance;
	
	// Instance
	public static Sound_Manager Instance {
		get {
			if(_instance == null && Application.isPlaying) {
				// Create container gameobject
				GameObject smc = new GameObject("SoundManagerContainer");
				DontDestroyOnLoad(smc); // Persist
				// Add sound manager as component
				smc.AddComponent<Sound_Manager>();
				_instance = smc.GetComponent<Sound_Manager>();
			}
			return _instance;
		}
	}
	
	// External Volume Controls (mute if in editor so Yori doesn't go crazy)
	#if MUTEMUSIC && UNITY_EDITOR
	public static float MusicVolume = 0f;
	#else
	public static float MusicVolume = 1f;
	#endif
	public static float EffectVolume = 1f;
	
	// Internal Volume variables that interact with actual source volumes
	private float _musicVolume {
		get {
			return _musicSrc.volume;
		}
		set {
			_musicSrc.volume = value;
		}
	}
	private float _effectVolume {
		get {
			return _effectSrc.volume;
		}
		set {
			_effectSrc.volume = value;
			for(int i = 0; i < LOOP_EFFECTS_CHANNELS; i++) {
				_loopEffectSrcs[i].volume = value;
			}
		}
	}
	
	// Main controls		
	public void Awake() {
		// Add audio sources and set volumes
		_musicSrc = gameObject.AddComponent<AudioSource>();
		_musicSrc.volume = MusicVolume;

		// Alternate music source used for crossfading
		_musicSrcAlt = gameObject.AddComponent<AudioSource>();
		_musicSrcAlt.volume = MusicVolume;

		_effectSrc = gameObject.AddComponent<AudioSource>();
		_effectSrc.volume = EffectVolume;

		_loopEffectSrcs = new AudioSource[LOOP_EFFECTS_CHANNELS];
		_loopEffectsChannelInUse = new int[LOOP_EFFECTS_CHANNELS];
		for(int i = 0; i < LOOP_EFFECTS_CHANNELS; i++) {
			_loopEffectSrcs[i] = gameObject.AddComponent<AudioSource>();
			_loopEffectSrcs[i].volume = EffectVolume;
			_loopEffectsChannelInUse[i] = 0;
		}
	}

	// Instantly change all sources to the current volume setting
	public void CommitVolumes() {
		_effectVolume = EffectVolume;
		_musicVolume = MusicVolume;
	}

	public void PlayEffectOnceAllowOverlap(AudioClip snd, bool overMusic = false, float volumePercent = 1f) {
		PlayEffectOnce(snd, overMusic, false, volumePercent);
	}
	
	public void PlayEffectOnce(AudioClip snd, bool overMusic = false, bool noOverlap = true, float volumePercent = 1f) {
		if(snd == null) {
			Debug.LogError("SOUND NOT FOUND");
			return;
		}

		// Don't overlap play effect once clips
		if(Time.realtimeSinceStartup >= oneShotLastPlayedTime || noOverlap == false || snd.name != oneShotLastPlayed) {
			// Play effect
			_effectSrc.PlayOneShot(snd, volumePercent); // Already scaled by the volume of the source
			oneShotLastPlayed = snd.name;
			oneShotLastPlayedTime = Time.realtimeSinceStartup + ONE_SHOT_DELAY;
		}
	}
	
	public void PlayEffectLoop(AudioClip snd, int channel) {
		// Check for playing a different sound
		if(_loopEffectSrcs[channel].clip != snd) {
			_loopEffectSrcs[channel].loop = true;
			_loopEffectSrcs[channel].clip = snd;
		}
		// Add to usage on this channel
		_loopEffectsChannelInUse[channel]++;
		// If we're not already playing, play the sound
		if(! _loopEffectSrcs[channel].isPlaying) {
			_loopEffectSrcs[channel].Play();
		}
	}

	public void StopAllEffectLoops() {
		for(int i = 0; i < LOOP_EFFECTS_CHANNELS; i++) {
			// Reset usage
			_loopEffectsChannelInUse[i] = 0;
			// Stop sound
			StopEffectLoop(i);
		}
	}

	public void StopEffectLoop(int channel) {
		// Subtract usage from this channel, don't go below zero
		if(_loopEffectsChannelInUse[channel] > 0) _loopEffectsChannelInUse[channel]--;
		// Make sure we're actually playing a sound and usage is zero
		if(_loopEffectSrcs[channel].isPlaying && _loopEffectsChannelInUse[channel] == 0) {
			_loopEffectSrcs[channel].Stop();
		}
	}
	
	public void PlayMusicOnce(AudioClip snd, float fadeIn = 0f, float volumePercent = 1f) {
		// Check for fading out and interrupt it
		_musicSrc.Stop();
		_musicSrc.loop = false;
		_musicSrc.clip = snd;

		if(fadeIn > 0f) {
			_musicVolume = 0f;
		} else {
			_musicVolume = volumePercent * MusicVolume;
		}
		_musicSrc.Play();
	}
	
	public void PlayMusicLoop(AudioClip snd, float fadeIn = 0f, float volumePercent = 1f) {
		if(_musicSrc.clip != snd) {
			_musicVolume = volumePercent * MusicVolume;
			_musicSrc.loop = true;
			_musicSrc.clip = snd;
		}
		if(! _musicSrc.isPlaying) {
			if(fadeIn > 0f) {
				_musicVolume = 0f;
			}
			_musicSrc.Play();
		}
	}

	public void PlayMusicAlt(AudioClip snd) {
		_musicSrcAlt.loop = false;
		_musicSrcAlt.volume = 0f; // Alt music source never gets volume

		if(_musicSrcAlt.clip != snd) {
			_musicSrcAlt.clip = snd;
		}
		_musicSrcAlt.Play();
	}

	public void PlayMusicLoopAlt(AudioClip snd, bool syncWithPrimarySrc = false) {
		_musicSrcAlt.loop = true;
		_musicSrcAlt.volume = 0f; // Alt music source never gets volume

		if(_musicSrcAlt.clip != snd) {
			_musicSrcAlt.clip = snd;
		}
		if(! _musicSrcAlt.isPlaying) {
			if(syncWithPrimarySrc) {
				_musicSrcAlt.timeSamples = _musicSrc.timeSamples;
			}
			_musicSrcAlt.Play();
		}
	}

	public void StopMusic() {
		_musicSrc.Stop();
	}
}
