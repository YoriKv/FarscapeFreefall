using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using InControl;

public class MenuPlayer:MonoBehaviour {
	public Image playerText;
	private Camera cam;
	// Input
	public int playerNum;
	public bool inGame = false;
	private InputDevice inputDevice;
	// Sound
	public AudioClip[] joinSnd;
	
	public void Awake() {
		inputDevice = (InputManager.Devices.Count > playerNum && PlayerControl.NumberOfPlayers > playerNum) ? InputManager.Devices[playerNum] : null;
		cam = Camera.main;
		if(inputDevice == null) {
			Destroy(playerText.gameObject);
			Destroy(gameObject);
		} else {
			inGame = true;
			StartCoroutine(JoinSoundRoutine());
		}
	}

	private IEnumerator JoinSoundRoutine() {
		float delay = 0.1f * (playerNum + 1);
		yield return new WaitForSeconds(delay);
		Sound_Manager.Instance.PlayEffectOnceAllowOverlap(joinSnd[playerNum]);
	}

	public void Update() {
		if(Menu.starting) {
			playerText.gameObject.SetActive(false);
		} else {
			playerText.transform.position = cam.WorldToScreenPoint(transform.position) + Vector3.down * (playerNum % 2 == 0 ? 60f : 130f);
		}
	}
}

