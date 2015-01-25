using UnityEngine;
using System.Collections;

public class Battery:MonoBehaviour {
	public bool collected = false;
	public AudioClip collectSnd;

	public void Awake() {
		tk2dSpriteAnimator anim = GetComponent<tk2dSpriteAnimator>();
		anim.PlayFromFrame(Random.Range(0, 4));
	}

	public void OnTriggerEnter2D() {
		if(collected)
			return;
		collected = true;
		GameManager.instance.Collect();
		Sound_Manager.Instance.PlayEffectOnce(collectSnd);
		Destroy(gameObject);
	}
}
