using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour {
	public static GameManager instance;
	
	public Text scoreText;
	private float score;
	private bool ended = false;
	private float endTime;

	public void Awake() {
		instance = this;
		score = 0f;
	}

	public void Update() {
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
		Application.LoadLevel(0);
	}

	private void DeathText() {
		// Text
		scoreText.text = "YOU DIED\n\n" + scoreText.text;
		// Reposition
		Vector3 pos = scoreText.rectTransform.localPosition;
		pos.y = 80f;
		scoreText.rectTransform.localPosition = pos;
	}
}
