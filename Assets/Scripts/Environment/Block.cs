using UnityEngine;
using System.Collections;

public class Block:MonoBehaviour {
	public const float SPEED = 10f;

	public bool starterBlock = false;

	// Sounds
	public AudioClip[] breakSnd;
	public AudioClip[] crumbleSnd;

	private int blockNum;
	private bool dying = false;
	private bool dyingPhase2 = false;
	private float deathTimer = 1f;
	private tk2dSprite sp;
	private tk2dSprite spikeSprite;
	private Color c;

	private bool spikes = false;

	public GameObject batteryPrefab;
	public GameObject spikePrefab;

	public GameObject particlesPrefab;

	private Spike spike = null;
	private GameObject battery = null;

	public void Awake() {
		// Sprite
		sp = GetComponent<tk2dSprite>();
		// Set random sprite
		blockNum = Random.Range(1, 4);
		sp.SetSprite("block" + blockNum + "_1");
		// Spikes
		if(!starterBlock) {
			if(Random.Range(0, 6) == 0) {
				GameObject spikeGO = (GameObject) Instantiate(spikePrefab, transform.position + Vector3.right * 0.4f + Vector3.up * 4f + Vector3.back, Quaternion.Euler(new Vector3(0f, 0f, -90f)));
				spikeGO.transform.parent = transform;
				spikeSprite = spikeGO.GetComponent<tk2dSprite>();
				spike = spikeGO.GetComponent<Spike>();
				spikes = true;
			} else if(Random.Range(0, 8) == 0) {
				battery = (GameObject)Instantiate(batteryPrefab, transform.position + Vector3.up * 6f + Vector3.back, Quaternion.identity);
				battery.rigidbody2D.velocity = Vector2.up * SPEED;
			}
		}
		// Velocity
		rigidbody2D.velocity = Vector2.up * SPEED;
	}

	public void Update() {
		// Destroy when we leave the screen
		if(transform.position.y > 70f) {
			Destroy(gameObject);
			if(battery != null) {
				Destroy(battery);
			}
		}

		// Keep the block alive if our spikes are bloody
		if(spikes) {
			if(spike.bloody && dying) {
				sp.color = Color.white;
				spikeSprite.color = Color.white;
				dying = false;
			}
		}

		// Death timer
		if((!spikes || !spike.bloody) && dying) {
			deathTimer -= Time.deltaTime;
			c = sp.color;
			c.a = deathTimer / 2f;
			//sp.color = c;
			if(spikes) {
				//spikeSprite.color = c;
			}
			if(!dyingPhase2 && deathTimer < 0.5f) {
				sp.SetSprite("block" + blockNum + "_3");
				dyingPhase2 = true;
				// Spawn particles
			}
			if(deathTimer <= 0f) {
				Sound_Manager.Instance.PlayEffectOnce(breakSnd[Random.Range(0, 3)], false, true, 0.4f);
				Destroy(gameObject);
			}
		}
	}

	public void OnCollisionEnter2D(Collision2D coll){
		if(dying || (spikes && spike.bloody)) {
			return;
		}

		if(Vector2.Dot(coll.contacts[0].normal, -Vector2.up) > 0.2f) {
			RockSpecial rock = coll.gameObject.GetComponent<RockSpecial>();
			if(rock != null && rock.rocking) {
				Destroy(gameObject);
				if(spikes) {
					Destroy(spike);
				}
			}
			dying = true;
			sp.SetSprite("block" + blockNum + "_2");
			// Spawn particles
			((GameObject) Instantiate(particlesPrefab, transform.position + new Vector3(0.2f, 2f, 1f), Quaternion.identity)).GetComponent<DestroyParticlesOnFinish>().followTarget = transform;
			// Play sound
			Sound_Manager.Instance.PlayEffectOnce(crumbleSnd[Random.Range(0, 3)], false, true, 0.6f);
		}

	}
}
