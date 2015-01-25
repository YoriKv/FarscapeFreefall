using UnityEngine;
using System.Collections;

public class BGDecoration:MonoBehaviour {
	private bool spawnedNextSegment = false;

	public void Awake() {
		rigidbody2D.velocity = Vector3.up * Block.SPEED * 0.8f;
	}

	public void Update() {
		if(!spawnedNextSegment && transform.position.y >= 0f) {
			spawnedNextSegment = true;
			GameObject wall = (GameObject) Instantiate(gameObject, transform.position + Vector3.down * 101.4f, Quaternion.identity);
			// This happens after awake so our if "RightWall" still works
			wall.name = this.name;
		}
		// Kill us once we're off the screen
		if(transform.position.y > 70f) {
			Destroy(gameObject);
		}
	}
}
