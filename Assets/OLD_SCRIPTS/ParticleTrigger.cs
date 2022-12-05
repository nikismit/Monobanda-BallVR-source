using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTrigger : MonoBehaviour {

	public ParticleSystem _partSysRing;

	// Use this for initialization
	void Start () {
		//_partSysRing = GetComponent<ParticleSystem>();
		var emission = _partSysRing.emission;
		emission.rateOverTime = 0;
	}

	// Update is called once per frame
	void Update () {

	}

	private void OnTriggerEnter(Collider other){
		if (other.gameObject.CompareTag ("Player")) {
			var emission = _partSysRing.emission;
			emission.rateOverTime = 10;
			Debug.Log ("BHAHJDHSJ");
		}
	}
}
