using UnityEngine;
using System.Collections;

public class Block:MonoBehaviour {
	public const float SPEED = 10f;

	public bool starterBlock = false;

	// Sounds
	public AudioClip[] breakSnd;
	public AudioClip[] crumbleSnd;

	private bool dying = false;
	private float deathTimer = 1f;
	private tk2dSprite sp;
	private tk2dSprite spikeSprite;
	private Color c;

	private bool spikes = false;

	public GameObject batteryPrefab;
	public GameObject spikePrefab;

	private Spike spike = null;
	private GameObject battery = null;

	public void Awake() {
		// Sprite
		sp = GetComponent<tk2dSprite>();
		// Set random sprite
		sp.SetSprite("block" + Random.Range(1, 4));
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
			sp.color = c;
			if(spikes) {
				spikeSprite.color = c;
			}
			if(deathTimer <= 0f) {
				Sound_Manager.Instance.PlayEffectOnce(breakSnd[Random.Range(0, 3)], false, true, 0.4f);
				Destroy(gameObject);
			}
		}
	}

	public void OnCollisionEnter2D(Collision2D coll){
		if(dying) {
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
			Sound_Manager.Instance.PlayEffectOnce(crumbleSnd[Random.Range(0, 3)], false, true, 0.6f);
		}

	}
}
