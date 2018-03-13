using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualMovement : MonoBehaviour {

	public GameObject target;
	public float speed = 5;
	Vector3 targetPos;
	Vector3 targetRot;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Vector3.Distance(this.transform.position, target.transform.position) > 1){
			targetPos = new Vector3(target.transform.position.x, this.transform.position.y, target.transform.position.z);
			this.transform.position = targetPos;
			targetRot = new Vector3(this.transform.eulerAngles.x, target.transform.eulerAngles.y, this.transform.eulerAngles.z);
			this.transform.eulerAngles = targetRot;
		}
	}
}
