using System;
using UnityEngine;
using UnityEngine.UI;
using InControl;

public class RockSpecial:MonoBehaviour {
	public const float FORCE = 8000f;

	public AudioClip specialSnd;

	// Player and input
	public Player player;
	private InputDevice inputDevice;
	private bool actionOn = false;
	private bool actionAvailable = true;
	public bool rocking = false;

	// Cooldown
	private Slider cooldownSlider;
	private Image cooldownFillImage;
	private Color origColor;
	private Color offColor;

	// Rocking
	private const float ROCK_TIME = 1f;
	private float rockTimer;

	// Leftover force timer
	private const float COOLDOWN_TIME = 2f;
	private float cooldownTimer;

	public void Start() {
		player = GetComponent<Player>();
		if(player.inGame) {
			cooldownSlider = player.cooldownSlider;
			cooldownFillImage = cooldownSlider.transform.FindChild("Fill Area").GetComponentInChildren<Image>();
			origColor = cooldownFillImage.color;
			offColor = origColor * Color.gray;
			inputDevice = player.inputDevice;
		}
	}

	public void Update() {
		// Action
		actionOn = inputDevice.Action1;
		// Cooldown
		if(cooldownTimer > 0f) {
			cooldownTimer -= Time.deltaTime;
			cooldownSlider.value = 1f - (cooldownTimer / COOLDOWN_TIME);
		} else {
			cooldownSlider.value = 1f;
			if(!actionAvailable) {
				actionAvailable = true;
				cooldownFillImage.color = origColor;
			}
		}
		// Rock action
		if(rockTimer > 0f) {
			rockTimer -= Time.deltaTime;
		} else if(rocking) {
			rocking = false;
			player.spAnim.Play("SmashHit");
		}
	}

	public void FixedUpdate() {
		if(actionOn && cooldownTimer <= 0f) {
			// Cooldown
			cooldownTimer = COOLDOWN_TIME;
			
			cooldownFillImage.color = offColor;
			actionAvailable = false;

			// Turn on rock
			rocking = true;
			rockTimer = ROCK_TIME;

			// Anim
			player.spAnim.Play("Smash");
			player.spAnim.AnimationCompleted += delegate(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip) {
				player.spAnim.Play("Float");
			};

			// Sound
			Sound_Manager.Instance.PlayEffectOnce(specialSnd);

			// Initial impulse force
			rigidbody2D.AddForce(-Vector2.up * FORCE * 0.5f, ForceMode2D.Impulse);
		}

		if(rocking) {
			// Jet down force
			rigidbody2D.AddForce(-Vector2.up * FORCE, ForceMode2D.Force);
		}
	}

	public void DisableSpecial() {
		enabled = false;
	}
}

