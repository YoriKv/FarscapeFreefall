using UnityEngine;
using System.Collections;

public class SideSpike:MonoBehaviour {
	public AudioClip flattenSound;

	private tk2dSprite sp;
	
	public bool bloody = false;
	private bool flattened = false;

	public void Awake() {
		// Sprite
		sp = GetComponent<tk2dSprite>();
	}

	public void OnCollisionEnter2D(Collision2D coll){
		if(flattened || bloody) {
			return;
		}
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
		if(!bloody) {
			flattened = true;
			collider2D.isTrigger = true;
		}
	}
}
