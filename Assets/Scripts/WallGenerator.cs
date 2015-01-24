using UnityEngine;
using System.Collections;

public class WallGenerator:MonoBehaviour {
	public GameObject wallPrefab;

	private const float SPAWN_TIME = 2f;
	private float spawnTimer = 0f;
	
	public void Update() {
		// Timer
		spawnTimer -= Time.deltaTime;

		if(spawnTimer <= 0f) {
			// Spawn!
			SpawnWall();

			spawnTimer = SPAWN_TIME;
		}
	}

	public void SpawnWall() {
		Vector3 pos = new Vector3(Random.Range(0, 22) * 4f, -3f);
		GameObject wall;

		for(int i = 0; i < Random.Range(3, 10); i++) {
			wall = (GameObject) GameObject.Instantiate(wallPrefab, pos, Quaternion.identity);
			wall.name = wallPrefab.name;
			pos.x += 4f;
		}
	}
}
