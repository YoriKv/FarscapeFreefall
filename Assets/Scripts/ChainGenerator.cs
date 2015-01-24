using UnityEngine;
using System.Collections;

public class ChainGenerator:MonoBehaviour {
	public Player[] players;
	public GameObject chainLinkPrefab;

	private const float PLAYER_DISTANCE = 10f;
	private const int NUM_LINKS = 10;

	public void Awake() {
		// Reposition players
		for(int i = 1; i < players.Length; i++) {
			players[i].transform.Translate(Vector3.right * PLAYER_DISTANCE * i);
		}

		// Create prefabs for chain between players
		for(int i = 1; i < players.Length; i++) {
			CreateChain(i, players[i-1], players[i]);
		}
	}

	public void CreateChain(int playerInd, Player p1, Player p2) {
		Vector2 linkSize = ((BoxCollider2D) chainLinkPrefab.collider2D).size;
		float linkOffset = linkSize.x * 0.5f;
		Vector3 pos = p1.transform.position + (Vector3.right * linkOffset);

		GameObject link;
		HingeJoint2D hinge;
		Rigidbody2D prevLink = p1.GetComponent<Rigidbody2D>();

		for(int i = 0; i < NUM_LINKS; i++) {
			link = (GameObject) GameObject.Instantiate(chainLinkPrefab, pos + Vector3.right * linkSize.x * i, Quaternion.identity);
			link.name = "ChainP" + playerInd + "_" + i;
			// Connect current link with previous link (or player)
			hinge = link.AddComponent<HingeJoint2D>();
			hinge.connectedBody = prevLink;
			hinge.anchor = new Vector2(-linkOffset, 0f);
			hinge.connectedAnchor = new Vector2(linkOffset, 0f);
			// Set previous link to current link
			prevLink = link.GetComponent<Rigidbody2D>();
		}

		// Final hinge
		hinge = prevLink.gameObject.AddComponent<HingeJoint2D>();
		hinge.connectedBody = p2.GetComponent<Rigidbody2D>();
		hinge.anchor = new Vector2(linkOffset, 0f);
		hinge.connectedAnchor = new Vector2(-linkOffset, 0f);
	}
}
