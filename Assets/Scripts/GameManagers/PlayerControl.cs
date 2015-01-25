using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using InControl;

public class PlayerControl:MonoBehaviour {
	public static int NumberOfPlayers = 4;

	private Text text;

	public void Awake() {
		text = GetComponent<Text>();
		text.text = text.text + " " + NumberOfPlayers;
	}

	public void Update() {
		if(Input.GetKeyUp(KeyCode.Alpha2)) {
			RemoveKeyboardProfiles();
			NumberOfPlayers = 2;
			Application.LoadLevel(0);
		} else if(Input.GetKeyUp(KeyCode.Alpha3)) {
			RemoveKeyboardProfiles();
			NumberOfPlayers = 3;
			Application.LoadLevel(0);
		} else if(Input.GetKeyUp(KeyCode.Alpha4)) {
			RemoveKeyboardProfiles();
			NumberOfPlayers = 4;
			Application.LoadLevel(0);
		}
	}

	public void RemoveKeyboardProfiles() {
		for(int i = InputManager.Devices.Count - 1; i > 0; i--) {
			if(InputManager.Devices[i].Name.StartsWith("Keyboard")) {
				InputManager.DetachDevice(InputManager.Devices[i]);
			}
		}
	}
}
