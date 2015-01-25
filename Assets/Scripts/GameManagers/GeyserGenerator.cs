using UnityEngine;
using System.Collections;

public class GeyserGenerator:MonoBehaviour {
	public GameObject geyserPrefab;

	private const float SPAWN_TIME = 10f;
	private float spawnTimer = 2f;

	private float startTime;
	private bool started = false;

	public void Start() {
		Time.timeScale = 0f;
		startTime = Time.realtimeSinceStartup;
	}
	
	public void Update() {
		// Start
		if(!started && (Time.realtimeSinceStartup - startTime) >= 1f) {
			Time.timeScale = 1f;
			started = true;
		}
		// Timer
		spawnTimer -= Time.deltaTime;

		if(spawnTimer <= 0f) {
			// Spawn!
			SpawnGeyser();

			spawnTimer = SPAWN_TIME;
		}
	}

	public void SpawnGeyser() {
		Vector3 pos = new Vector3(Random.Range(0, 22) * 4f, -80f, -1f);
		GameObject geyser;

		geyser = (GameObject) GameObject.Instantiate(geyserPrefab, pos, Quaternion.identity);
		geyser.name = geyserPrefab.name;
	}
}
