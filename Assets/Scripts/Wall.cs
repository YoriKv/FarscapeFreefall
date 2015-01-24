using UnityEngine;
using System.Collections;

public class Wall:MonoBehaviour {
	private const float SPEED = 10f;

	public void Awake() {
		// Velocity
		rigidbody2D.velocity = Vector2.up * SPEED;
	}

	public void Update() {
		// Destroy when we leave the screen
		if(transform.position.y > 54f) {
			Destroy(gameObject);
		}
	}
}
