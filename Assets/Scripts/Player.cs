using System;
using UnityEngine;
using InControl;

public class Player:MonoBehaviour {
	public const float FORCE = 50f;
	public const float MAX_SPEED = 50f;

	// Input
	public int playerNum;
	private InputDevice inputDevice;

	// Physics
	private float forceX;
	private float velX;
	private Vector2 velocity;

	private tk2dSprite sp;

	public void Awake() {
		sp = GetComponent<tk2dSprite>();
		sp.SetSprite("astros/" + playerNum);
	}

	public void Update() {
		InputDevice inputDevice = (InputManager.Devices.Count > playerNum) ? InputManager.Devices[playerNum] : null;
		if(inputDevice == null) {
			// If no controller exists for this player, destroy it
			//Destroy(gameObject);
		} else {
			UpdateInput(inputDevice);
		}
	}

	private void UpdateInput(InputDevice inputDevice) {
		// Action
		if(inputDevice.Action1) {
			// Perform action
		}
		// Direction
		forceX = FORCE * inputDevice.Direction.X;
	}

	public void FixedUpdate() {
		// Move target left and right with input stick
		if(Mathf.Abs(forceX) != 0f) {
			rigidbody2D.AddForce(Vector2.right * forceX, ForceMode2D.Impulse);
		}
		// Limit speed
		velX = rigidbody2D.velocity.x;
		if(Mathf.Abs(velX) >= MAX_SPEED) {
			if(velX > 0) {
				velocity.x = MAX_SPEED;
			} else {
				velocity.x = -MAX_SPEED;
			}
			velocity.y = rigidbody2D.velocity.y;
			rigidbody2D.velocity = velocity;
		}
	}
}

