using UnityEngine;
using System.Collections;

public class Geyser:MonoBehaviour {
	public const float FORCE = 800f;
	public const float SPEED = 50f;

	public void Update() {
		transform.Translate(Vector3.up * SPEED * Time.deltaTime);

		if(transform.position.y > 70f) {
			Destroy(gameObject);
		}
	}

	public void OnTriggerStay2D(Collider2D coll) {
		coll.rigidbody2D.AddForce(Vector2.up * FORCE, ForceMode2D.Force);
	}
}
