using System;
using System.Collections;
using UnityEngine;
using InControl;

// This custom profile is enabled by adding it to the Custom Profiles list
// on the InControlManager component, or you can attach it yourself like so:
// InputManager.AttachDevice( new UnityInputDevice( "KeyboardProfile" ) );
// 
public class KeyboardProfileWASD : UnityInputDeviceProfile {
	public KeyboardProfileWASD() {
		Name = "Keyboard WASD";
		Meta = "A WASD keyboard profile.";

		// This profile only works on desktops.
		SupportedPlatforms = new[]
			{
				"Windows",
				"Mac",
				"Linux"
			};

		Sensitivity = 1.0f;
		LowerDeadZone = 0.0f;
		UpperDeadZone = 1.0f;

		ButtonMappings = new[]
			{
				new InputControlMapping
				{
					Handle = "Fire - Keyboard",
					Target = InputControlType.Action1,
					// KeyCodeButton fires when any of the provided KeyCode params are down.
					Source = KeyCodeButton( KeyCode.Space )
				},
			};

		AnalogMappings = new[]
			{
				new InputControlMapping {
					Handle = "Move X",
					Target = InputControlType.LeftStickX,
					Source = KeyCodeAxis( KeyCode.A, KeyCode.D )
				},
				new InputControlMapping {
					Handle = "Move Y",
					Target = InputControlType.LeftStickY,
					Source = KeyCodeAxis( KeyCode.S, KeyCode.W )
				}
			};
	}
}
