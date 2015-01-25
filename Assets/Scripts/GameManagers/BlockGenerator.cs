using UnityEngine;
using System.Collections;

public class BlockGenerator:MonoBehaviour {
	public GameObject blockPrefab;

	private const float SPAWN_TIME = 0.6f;
	private float spawnTimer = 0f;

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
			SpawnBlock();

			spawnTimer = SPAWN_TIME + Random.Range(0f, 1f);
		}
	}

	public void SpawnBlock() {
		Vector3 pos = new Vector3(16f + Random.Range(0, 10) * 7f, -10f);
		GameObject block;

		int cnt = Random.Range(1, 5);
		if(cnt < 3) {
			cnt = 1;
		} else {
			cnt = 2;
		}
		for(int i = 0; i < cnt; i++) {
			block = (GameObject) GameObject.Instantiate(blockPrefab, pos, Quaternion.identity);
			block.name = blockPrefab.name;
			pos.x += 7f;
		}
	}
}
