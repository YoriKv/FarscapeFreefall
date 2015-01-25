using System;
using UnityEngine;
using UnityEngine.UI;
using InControl;

public class Player:MonoBehaviour {
	public const float FORCE = 50f;
	public const float MAX_SPEED = 50f;

	// Camera
	private Camera cam;

	// Layer mask
	public LayerMask stickMask;

	// Sprites
	public String[] playerSprites;

	// Audio
	public AudioClip[] specialSounds;
	public AudioClip[] goreSounds;
	public AudioClip[] deathSounds;

	// Input
	public int playerNum;
	public Slider cooldownSlider;
	[HideInInspector]
	public bool inGame = false;
	[HideInInspector]
	public bool playerOnTheEnd = false;
	[HideInInspector]
	public InputDevice inputDevice;
	[HideInInspector]
	public bool isDead = false;

	// Butt shield prefab
	public GameObject buttShieldPrefab;

	// Corpse
	public GameObject corpsePrefab;

	// Physics
	private float forceX;
	private float velX;
	private Vector2 velocity;

	private tk2dSprite sp;

	public void Awake() {
		cam = Camera.main;
		sp = GetComponent<tk2dSprite>();
		sp.SetSprite(playerSprites[playerNum]);
		inputDevice = (InputManager.Devices.Count > playerNum && PlayerControl.NumberOfPlayers > playerNum) ? InputManager.Devices[playerNum] : null;
		if(inputDevice == null) {
			cooldownSlider.gameObject.SetActive(false);
			// If no controller exists for this player, destroy it
			Destroy(gameObject);
		} else {
			inGame = true;
		}
		// Actions
		if(playerNum == 0) {
			// Milyway Mike
			gameObject.AddComponent<JetSpecial>().specialSnd = specialSounds[playerNum];
		} else if(playerNum == 1) {
			// Quasar Quade
			gameObject.AddComponent<FloatSpecial>().specialSnd = specialSounds[playerNum];
		} else if(playerNum == 2) {
			// Stardust Stan
			gameObject.AddComponent<StickSpecial>().specialSnd = specialSounds[playerNum];
		} else if(playerNum == 3) {
			// Cosmonaut (Cosmo) Carla
			gameObject.AddComponent<RockSpecial>().specialSnd = specialSounds[playerNum];
		}
	}

	public void Update() {
		// End Game
		if(transform.position.y < -5f) {
			GameManager.instance.EndLevel();
		} else if(transform.position.y > 70f) {
			GameManager.instance.EndLevel();
		}

		// Don't do anything if we're dead
		if(isDead) {
			return;
		}

		// Input
		UpdateInput(inputDevice);

		// Reposition slider over us
		cooldownSlider.transform.position = cam.WorldToScreenPoint(transform.position) + Vector3.up * 40f;
		if(transform.position.y > 49f || transform.position.y < 3f || Time.timeScale == 0f) {
			cooldownSlider.gameObject.SetActive(false);
		} else {
			cooldownSlider.gameObject.SetActive(true);
		}
	}

	private void UpdateInput(InputDevice inputDevice) {
		if(Time.timeScale == 0f) return;

		// Direction
		forceX = FORCE * inputDevice.Direction.X;
		if(playerOnTheEnd) {
			forceX *= 0.75f;
		}
		// Facing direction
		if(forceX > 0f && sp.FlipX) {
			sp.FlipX = false;
		} else if(forceX < 0f && !sp.FlipX) {
			sp.FlipX = true;
		}
	}

	public void FixedUpdate() {
		if(isDead)
			return;
		// Limit speed
		velX = rigidbody2D.velocity.x;
		if(velX >= MAX_SPEED && forceX > 0) {
			// Too fast, no more force
		} else if(velX <= -MAX_SPEED && forceX < 0) {
			// Too fast in negative, no more force
		} else {
			// Move target left and right with input stick
			rigidbody2D.AddForce(Vector2.right * forceX, ForceMode2D.Impulse);
		}
	}

	public void Kill() {
		if(isDead)
			return;
		isDead = true;
		rigidbody2D.fixedAngle = false;
		cooldownSlider.gameObject.SetActive(false);
		// Adjust collider
		BoxCollider2D box = (BoxCollider2D)collider2D;
		box.size = new Vector2(2f, 2f);
		box.center = new Vector2(0, -1.7f);
		// Spawn corpse
		((GameObject)Instantiate(corpsePrefab, transform.position, transform.rotation)).GetComponent<tk2dSprite>().SetSprite(sp.CurrentSprite.name + "Dead");
		// Sprite
		sp.SetSprite(sp.CurrentSprite.name + "DeadLegs");
		// Sounds
		Sound_Manager.Instance.PlayEffectOnce(deathSounds[playerNum * 3 + UnityEngine.Random.Range(0, 3)]);
		// Special
		SendMessage("DisableSpecial");
		// Gore sound
		Sound_Manager.Instance.PlayEffectOnce(goreSounds[UnityEngine.Random.Range(0, 3)]);
	}
}

