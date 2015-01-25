using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCorpse:MonoBehaviour {
	public void Update() {
		rigidbody2D.velocity = Vector3.up * Block.SPEED;
	}
}

