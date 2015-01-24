using System;
using UnityEngine;
using UnityEngine.UI;
using InControl;

public class Player:MonoBehaviour {
	public const float FORCE = 50f;
	public const float MAX_SPEED = 50f;

	// Camera
	private Camera cam;

	// Input
	public int playerNum;
	public Slider cooldownSlider;
	[HideInInspector]
	public bool inGame = false;
	[HideInInspector]
	public bool playerOnTheEnd = false;
	[HideInInspector]
	public InputDevice inputDevice;

	// Physics
	private float forceX;
	private float velX;
	private Vector2 velocity;

	private tk2dSprite sp;

	public void Awake() {
		cam = Camera.main;
		sp = GetComponent<tk2dSprite>();
		sp.SetSprite("astros/" + playerNum);
		inputDevice = (InputManager.Devices.Count > playerNum) ? InputManager.Devices[playerNum] : null;
		if(inputDevice == null) {
			cooldownSlider.gameObject.SetActive(false);
			// If no controller exists for this player, destroy it
			Destroy(gameObject);
		} else {
			inGame = true;
		}
		// Actions
		if(playerNum == 0) {
			gameObject.AddComponent<JetSpecial>();
		} else if(playerNum == 1) {
			gameObject.AddComponent<FloatSpecial>();
		}
	}

	public void Update() {
		UpdateInput(inputDevice);
		// Reposition slider over us
		cooldownSlider.transform.position = cam.WorldToScreenPoint(transform.position) + Vector3.up * 50f;
	}

	private void UpdateInput(InputDevice inputDevice) {
		// Action
		if(inputDevice.Action1) {
			// Perform action
		}
		// Direction
		forceX = FORCE * inputDevice.Direction.X;
		if(playerOnTheEnd) {
			forceX *= 0.7f;
		}

		// Death
		if(transform.position.y < -5f) {
			Application.LoadLevel(0);
		}
		if(transform.position.y > 60f) {
			Application.LoadLevel(0);
		}
	}

	public void FixedUpdate() {
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
}

