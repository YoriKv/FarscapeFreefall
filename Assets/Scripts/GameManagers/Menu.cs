using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using InControl;

public class Menu:MonoBehaviour {
	public AudioClip menuMusic;
	public AudioClip startSound;

	public Text highScoreText;

	public static bool starting = false;
	public tk2dSprite fade;
	private Color c;

	private float startTimer = 2f;

	public void Awake() {
		if(InputManager.Devices.Count < PlayerControl.NumberOfPlayers) {
			InputManager.AttachDevice(new UnityInputDevice(new KeyboardProfileArrows()));
		}
		if(InputManager.Devices.Count < PlayerControl.NumberOfPlayers) {
			InputManager.AttachDevice(new UnityInputDevice(new KeyboardProfileWASD()));
		}

		// Look for device changes
		InputManager.OnDeviceAttached += inputDevice => Application.LoadLevel(0);
		InputManager.OnDeviceDetached += inputDevice => Application.LoadLevel(0);

		starting = false;
		Sound_Manager.Instance.PlayMusicLoop(menuMusic);

		GameManager.highScore = PlayerPrefs.GetInt("HighScore", 0);
		highScoreText.text = "HIGH SCORE: " + GameManager.highScore;
	}

	public void Update() {

		if(starting) {
			startTimer -= Time.deltaTime;
			c.a = 1f - (startTimer / 2f);
			fade.color = c;
			if(startTimer <= 0f) {
				Application.LoadLevel("Game");
			}
		} else {
			for(int i = 0; i < InputManager.Devices.Count && i < PlayerControl.NumberOfPlayers; i++) {
				if(InputManager.Devices[i].Action1) {
					starting = true;
					c = fade.color;
					Sound_Manager.Instance.PlayEffectOnce(startSound);
				}
			}
		}
	}
}
