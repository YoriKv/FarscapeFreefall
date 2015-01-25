using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using InControl;

public class FadeIn:MonoBehaviour {
	public tk2dSprite fade;
	private Color c;

	private float startTime;

	public void Start() {
		startTime = Time.realtimeSinceStartup;
	}

	public void Update() {
		float t = Time.realtimeSinceStartup - startTime;
		c.a = 1f - (t / 2f);
		fade.color = c;
		if(t > 2f) {
			Destroy(fade.gameObject);
			Destroy(gameObject);
		}
	}
}
