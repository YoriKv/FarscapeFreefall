using System;
using UnityEngine;
using UnityEngine.UI;
using InControl;

public class JetSpecial:MonoBehaviour {
	public const float FORCE = 10000f;

	public AudioClip specialSnd;

	// Player and input
	public Player player;
	private InputDevice inputDevice;
	private bool actionOn = false;
	private bool actionAvailable = true;

	// Cooldown
	private Slider cooldownSlider;
	private Image cooldownFillImage;
	private Color origColor;
	private Color offColor;

	// Leftover force timer
	private const float COOLDOWN_TIME = 3f;
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
	}

	public void FixedUpdate() {
		if(actionOn && cooldownTimer <= 0f) {
			Sound_Manager.Instance.PlayEffectOnce(specialSnd);
			// Jet up force
			rigidbody2D.AddForce(Vector2.up * FORCE, ForceMode2D.Impulse);
			// Cooldown
			cooldownTimer = COOLDOWN_TIME;
			
			cooldownFillImage.color = offColor;
			actionAvailable = false;
		}
	}

	public void DisableSpecial() {
		enabled = false;
	}
}

