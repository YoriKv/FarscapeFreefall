using UnityEngine;
using System.Collections;

public class Geyser:MonoBehaviour {
	public const float FORCE = 800f;
	public const float SPEED = 50f;

	public GameObject geyserParticles;

	public AudioClip geyserSnd;

	public void Awake() {
		// Sound
		Sound_Manager.Instance.PlayEffectOnce(geyserSnd, false, true, 0.3f);
	}

	public void Start() {
		// Particles
		Vector3 pos = transform.position;
		pos.y = 0f;
		pos.z = -1.5f;
		Instantiate(geyserParticles, pos, geyserParticles.transform.rotation);
	}

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
