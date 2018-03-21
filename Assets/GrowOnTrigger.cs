using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowOnTrigger : MonoBehaviour {

	public bool checkObjectToTrigger = false;
	public GameObject ObjectToTrigger;

	
	public float sizeMultiplier = 2.0f;
	public float growSpeed = 2.0f;

	public float triggerTime = 2.0f;

	float timer = 0.0f;
	bool addingTime = false;

	bool growing = false;

	Vector3 normalSize = new Vector3 (1,1,1);
	Vector3 wantedSize = new Vector3 (2,2,2);

	

	// Use this for initialization
	void Start () {
		normalSize = this.transform.localScale;
		wantedSize = normalSize * sizeMultiplier;
	}
	
	// Update is called once per frame
	void Update () {

		if(addingTime){
			timer += Time.deltaTime;
		}
		if (timer >= triggerTime){
			growing = true;
			timer = 0.0f;
		}

		
		if(growing){
			if(this.transform.localScale.x < wantedSize.x){
				transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * growSpeed;
			}
		} else {
			if(this.transform.localScale.x > normalSize.x){
				transform.localScale -= new Vector3(1, 1, 1) * Time.deltaTime * growSpeed;
			}
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
		growing = false;
	}

	

}
