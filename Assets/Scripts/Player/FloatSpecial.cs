using System;
using UnityEngine;
using UnityEngine.UI;
using InControl;

public class FloatSpecial:MonoBehaviour {
	public const float MAX_SPEED = 5f;
	public const float FORCE = 5000f;

	public AudioClip specialSnd;

	// Player and input
	public Player player;
	private InputDevice inputDevice;
	private bool actionOn = false;
	private bool actionDisabled = false;

	// Cooldown
	private Slider cooldownSlider;
	private Image cooldownFillImage;
	private Color origColor;

	// Float power
	private const float MAX_FLOAT_POWER = 1f;
	private float floatPower = MAX_FLOAT_POWER;

	// Leftover force timer
	private const float WIND_DOWN_TIME = 0.2f;
	private float leftoverForceTimer;

	// Physics
	private float velY;
	private Vector2 velocity;

	public void Start() {
		player = GetComponent<Player>();
		if(player.inGame) {
			cooldownSlider = player.cooldownSlider;
			cooldownFillImage = cooldownSlider.transform.FindChild("Fill Area").GetComponentInChildren<Image>();
			origColor = cooldownFillImage.color;
			inputDevice = player.inputDevice;
		}
	}

	public void Update() {
		// Action
		if(!actionDisabled && floatPower > (MAX_FLOAT_POWER * 0.1f)) {
			if(!actionOn && inputDevice.Action1) {
				Sound_Manager.Instance.PlayEffectOnce(specialSnd);
				player.spAnim.Play("Jet");
			} else if(actionOn && !inputDevice.Action1) {
				player.spAnim.Play("Float");
			}
			actionOn = inputDevice.Action1;
			if(actionOn) {
				floatPower -= Time.deltaTime;
			}
		} else {
			if(!actionDisabled) {
				actionDisabled = true;
				cooldownFillImage.color = Color.red;
			}
			if(actionOn) {
				actionOn = false;
				player.spAnim.Play("Float");
			}
		}
		// Undisable
		if(actionDisabled && floatPower > (MAX_FLOAT_POWER * 0.9f)) {
			actionDisabled = false;
			cooldownFillImage.color = origColor;
		}
		// Recover float power
		if(!actionOn) {
			floatPower += Time.deltaTime;
			floatPower = Mathf.Min(floatPower, MAX_FLOAT_POWER);
		}
		// Display remaining float power
		cooldownSlider.value = floatPower / MAX_FLOAT_POWER;
	}
	
	public void FixedUpdate() {
		if(actionOn) {
			// Limit falling speed
			velY = rigidbody2D.velocity.y;
			if(velY <= MAX_SPEED) {
				leftoverForceTimer = WIND_DOWN_TIME;
			}
			// "wind down" our counter force
			if(leftoverForceTimer > 0f) {
				rigidbody2D.AddForce(Vector2.up * FORCE * (MAX_SPEED - velY) * (leftoverForceTimer / WIND_DOWN_TIME), ForceMode2D.Force);
				leftoverForceTimer -= Time.deltaTime;
			}
		}
	}

	public void DisableSpecial() {
		enabled = false;
	}
}

