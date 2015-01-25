using UnityEngine;
using System.Collections;

public class Wall:MonoBehaviour {
	public const float SPEED = 10f;

	public bool starterWall = false;

	private bool dying = false;
	private float deathTimer = 1f;
	private tk2dSprite sp;
	private Color c;

	private bool spikes = false;

	public GameObject batteryPrefab;
	public ParticleSystem particles;

	private GameObject battery = null;

	public void Awake() {
		// Sprite
		sp = GetComponent<tk2dSprite>();
		// Spikes
		if(!starterWall) {
			if(Random.Range(0, 5) == 0) {
				sp.SetSprite("tiles/24");
				spikes = true;
			} else if(Random.Range(0, 8) == 0) {
				battery = (GameObject)Instantiate(batteryPrefab, transform.position + Vector3.up * 4f, Quaternion.identity);
				battery.rigidbody2D.velocity = Vector2.up * SPEED;
			}
		}
		// Velocity
		rigidbody2D.velocity = Vector2.up * SPEED;
	}

	public void Update() {
		// Death timer
		if(dying) {
			deathTimer -= Time.deltaTime;
			c = sp.color;
			c.a = deathTimer / 2f;
			sp.color = c;
			if(deathTimer <= 0f) {
				Destroy(gameObject);
			}
		}
		// Destroy when we leave the screen
		if(transform.position.y > 54f) {
			Destroy(gameObject);
			if(battery != null) {
				Destroy(battery);
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
				transform.DetachChildren();
				particles.gameObject.SetActive(true);
				Destroy(gameObject);
			} else if(spikes) {
				Player player = coll.gameObject.GetComponent<Player>();
				if(player != null) {
					player.Kill();
				}
			}
		}

		dying = true;
	}
}
