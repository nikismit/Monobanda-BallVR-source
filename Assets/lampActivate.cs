using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lampActivate : MonoBehaviour {

	public bool checkObjectToTrigger = false;
	public GameObject ObjectToTrigger;
	public float triggerTime = 2.0f;

	float timer = 0.0f;
	bool addingTime = false;


	// Use this for initialization
	void Start () {
		this.GetComponent<Light>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
		if(addingTime){
			timer += Time.deltaTime;
		}
		if (timer >= triggerTime){
			this.GetComponent<Light>().enabled = true;
			timer = 0.0f;
		}

	}

	private void OnTriggerEnter(Collider other)
	{
		if(checkObjectToTrigger == true){
			if(other.gameObject == ObjectToTrigger){
				addingTime = true;
				timer = 0.0f;
			}
		} else {
			addingTime = true;
			timer = 0.0f;
		}

	}

	private void OnTriggerExit(Collider other)
	{
		addingTime = false;
		timer = 0.0f;
		this.GetComponent<Light>().enabled = false;
	}

}
