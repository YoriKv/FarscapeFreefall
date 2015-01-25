using UnityEngine;
using System.Collections;

public class DestroyParticlesOnFinish:MonoBehaviour {
	public Transform followTarget = null;
	private Vector3 offset;
	private ParticleSystem ps;

	public void Awake() {
		ps = GetComponent<ParticleSystem>();
	}

	public void Start() {
		if(followTarget != null) {
			offset = transform.position - followTarget.position;
		}
	}

	public void Update() {
		if(followTarget != null) {
			transform.position = followTarget.position + offset;
		}
		if(!ps.IsAlive()) {
			Destroy(gameObject);
		}
	}
}
