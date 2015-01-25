using UnityEngine;
using System.Collections;

public class Wall:MonoBehaviour {
	public GameObject sideSpikePrefab;

	private bool spawnedNextSegment = false;

	public void Awake() {
		rigidbody2D.velocity = Vector3.up * Block.SPEED;
		// Only do this on the first wall segment, not any clone
		if(name == "RightWall" || name == "LeftWall") {
			transform.Translate(Vector3.up * Random.Range(0f, 40f));
		}
	}

	public void Update() {
		if(!spawnedNextSegment && transform.position.y > 50f) {
			spawnedNextSegment = true;
			GameObject wall = (GameObject) Instantiate(gameObject, transform.position + Vector3.down * 108f, transform.rotation);
			// This happens after awake so our if "RightWall" still works
			wall.name = this.name;
		}
		// Kill us once we're off the screen
		if(transform.position.y > 120f) {
			Destroy(gameObject);
		}
	}
}
