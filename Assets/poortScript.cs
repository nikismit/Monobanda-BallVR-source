using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class poortScript : MonoBehaviour {

	public float speedMultiplier = 2.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.GetComponent<AudioMovement>()){
			other.GetComponent<AudioMovement>().currentSpeed *= speedMultiplier;
		}
	}

}
