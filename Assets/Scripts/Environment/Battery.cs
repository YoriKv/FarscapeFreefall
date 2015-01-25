using UnityEngine;
using System.Collections;

public class Battery:MonoBehaviour {
	public bool collected = false;

	public void OnTriggerEnter2D() {
		if(collected)
			return;
		collected = true;
		GameManager.instance.Collect();
		Destroy(gameObject);
	}
}
