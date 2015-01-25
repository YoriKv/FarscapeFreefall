using UnityEngine;
using System.Collections;

public class Spike:MonoBehaviour {
	public AudioClip flattenSound;

	private tk2dSprite sp;
	
	public bool bloody = false;
	private bool flattened = false;

	public GameObject particlePrefab;

	public void Awake() {
		// Sprite
		sp = GetComponent<tk2dSprite>();
		// Set random sprite
		sp.SetSprite("Spikes" + Random.Range(1, 3));
	}

	public void OnCollisionEnter2D(Collision2D coll){
		if(flattened || bloody) {
			return;
		}
		if(Vector2.Dot(coll.contacts[0].normal, -Vector2.up) > 0.2f) {
			RockSpecial rock = coll.gameObject.GetComponent<RockSpecial>();
			if(rock != null && rock.rocking) {
				Destroy(gameObject);
			} else if(!flattened) {
				Player player = coll.gameObject.GetComponent<Player>();
				if(player != null) {
					player.Kill();
					bloody = true;
					sp.SetSprite(sp.CurrentSprite.name + "_Blood");
				}
			}
		}
		if(!bloody) {
			Sound_Manager.Instance.PlayEffectOnce(flattenSound);
			flattened = true;
			collider2D.isTrigger = true;
			// Spawn particles
			((GameObject) Instantiate(particlePrefab, transform.position + Vector3.back * 2f, particlePrefab.transform.rotation)).GetComponent<DestroyParticlesOnFinish>().followTarget = transform;
		}
	}
}
