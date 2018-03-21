using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lampActivate : MonoBehaviour {

	
	public float triggerTime = 2.0f;

	public Color wantedColor;
	public float colorErrorMargin = 0.1f;

	float timer = 0.0f;
	public bool addingTime = false;
	bool redGood = false;
	bool greenGood = false;
	bool blueGood = false;

	// Use this for initialization
	void Start () {
		this.GetComponent<Light>().enabled = false;
		wantedColor = this.GetComponent<Light>().color;
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
		Color otherColor = other.GetComponent<Renderer>().material.color;
		if(otherColor.r <= wantedColor.r + colorErrorMargin && otherColor.r >= wantedColor.r - colorErrorMargin){
			redGood = true;
		} else {
			redGood = false;
		}

		if(otherColor.b <= wantedColor.b + colorErrorMargin && otherColor.b >= wantedColor.b - colorErrorMargin){
			blueGood = true;
		} else {
			blueGood = false;
		}

		if(otherColor.g <= wantedColor.g + colorErrorMargin && otherColor.g >= wantedColor.g - colorErrorMargin){
			greenGood = true;
		} else {
			greenGood = false;
		}

		if(redGood && greenGood && blueGood){
			//print("color is good!");
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
