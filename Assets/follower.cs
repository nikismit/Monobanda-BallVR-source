using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class follower : MonoBehaviour {

	public Transform target;
	public float distance = 10.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		// Early out if we don't have a target
         if (!target)
             return;
     
         // Calculate the current rotation angles
         float wantedRotationAngle = target.eulerAngles.y;
         float wantedHeight = target.position.y;
         float currentRotationAngle = transform.eulerAngles.y;
         float currentHeight = transform.position.y;
     
         // Damp the rotation around the y-axis
         //currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, Time.deltaTime);
 
         // Damp the height
         //currentHeight = Mathf.Lerp (currentHeight, wantedHeight, Time.deltaTime);
 
         // Convert the angle into a rotation
         Quaternion currentRotation = Quaternion.Euler (0, wantedRotationAngle, 0);
     
         // Set the position of the camera on the x-z plane to:
         // distance meters behind the target
         transform.position = target.position;
         transform.position -= currentRotation * Vector3.forward * distance;
 
         // Set the height of the camera
         transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
     
         // Always look at the target
         transform.LookAt (target);

	}
}
