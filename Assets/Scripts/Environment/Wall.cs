using UnityEngine;
using System.Collections;

public class Wall:MonoBehaviour {
	private const float SPEED = 10f;

	private bool dying = false;
	private float deathTimer = 1f;
	private tk2dSprite sp;
	private Color c;

	public void Awake() {
		// Sprite
		sp = GetComponent<tk2dSprite>();
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
		}
	}

	public void OnCollisionEnter2D(Collision2D coll){
		if(!dying) {
			dying = true;
		}
	}
}
