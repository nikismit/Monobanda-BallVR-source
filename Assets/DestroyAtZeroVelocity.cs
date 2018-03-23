using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAtZeroVelocity : MonoBehaviour {

	private Rigidbody rigbody;
	public float deathTimer = 5.0f;
	private float timer = 0.0f;
	public bool startTimer;
	bool launched = false;

	// Use this for initialization
	void Start () {
		rigbody = this.GetComponent<Rigidbody>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if(rigbody.velocity.x > 0.0f || rigbody.velocity.y > 0.0f || rigbody.velocity.z > 0.0f && launched == false){
			launched = true;
		}

		if((rigbody.velocity == new Vector3(0.0f, 0.0f, 0.0f) && launched == true) || startTimer == true){
			timer += Time.deltaTime;
		} else {
			timer = 0.0f;
		}

		if(timer >= deathTimer){
			timer = 0.0f;
			this.gameObject.SetActive(false);
			launched = false;
		}

	}
}
