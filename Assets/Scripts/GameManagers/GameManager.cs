using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using InControl;

public class GameManager : MonoBehaviour {
	public AudioClip gameMusic;
	public AudioClip gameOverSound;
	public AudioClip highScoreSound;

	public static GameManager instance;

	public static int highScore;

	public Text scoreText;
	public Text highScoreText;
	private float score;
	private bool ended = false;
	private float endTime;

	public void Awake() {
		if(InputManager.Devices.Count < PlayerControl.NumberOfPlayers) {
			InputManager.AttachDevice(new UnityInputDevice(new KeyboardProfileArrows()));
		}
		if(InputManager.Devices.Count < PlayerControl.NumberOfPlayers) {
			InputManager.AttachDevice(new UnityInputDevice(new KeyboardProfileWASD()));
		}

		instance = this;
		score = 0f;
		highScore = PlayerPrefs.GetInt("HighScore", 0);
		highScoreText.text = "HIGH SCORE: " + highScore;

		Sound_Manager.Instance.PlayMusicLoop(gameMusic);
	}

	public void Update() {
		if(Input.GetKeyUp(KeyCode.Escape)) {
			Application.LoadLevel(0);
			Time.timeScale = 1f;
		}

		score += Time.deltaTime * 100f;
		if(! ended) {
			scoreText.text = "SCORE: " + Mathf.RoundToInt(score);
		}
	}

	public void Collect() {
		score += 500f;
	}

	public void EndLevel() {
		if(!ended) {
			ended = true;
			Time.timeScale = 0f;
			endTime = Time.realtimeSinceStartup;
			StartCoroutine(EndRoutine());
		}
	}

	private IEnumerator EndRoutine() {
		DeathText();
		while(Time.realtimeSinceStartup - endTime < 3f) {
			yield return false;
		}
		Time.timeScale = 1f;
		Application.LoadLevel(1);
	}

	private void DeathText() {
		// Save score
		bool high = false;
		if(score > highScore) {
			highScore = Mathf.RoundToInt(score);
			PlayerPrefs.SetInt("HighScore", highScore);
			highScoreText.text = "HIGH SCORE: " + highScore;
			PlayerPrefs.Save();
			high = true;
			Sound_Manager.Instance.PlayEffectOnce(highScoreSound);
		} else {
			Sound_Manager.Instance.PlayEffectOnce(gameOverSound);
		}
		// Text
		if(high) {
			scoreText.text = "YOU DIED\n\n" + scoreText.text + "\n\nHIGH SCORE ACHIEVED!!!";
		} else {
			scoreText.text = "YOU DIED\n\n" + scoreText.text;
		}
		// Reposition
		Vector3 pos = scoreText.rectTransform.localPosition;
		pos.y = 80f;
		scoreText.rectTransform.localPosition = pos;
	}
}
