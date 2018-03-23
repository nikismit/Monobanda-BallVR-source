using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleMove : MonoBehaviour {

	public float speed = 10;
	public float rotSpeed = 100;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * Time.deltaTime * speed, Space.Self);
		transform.Rotate(new Vector3(0, Input.GetAxis("Horizontal") * Time.deltaTime * rotSpeed,0));

	}
}
