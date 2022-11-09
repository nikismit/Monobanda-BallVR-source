﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour {

	/*
     This camera smoothes out rotation around the y-axis and height.
     Horizontal Distance to the target is always fixed.

     There are many different ways to smooth the rotation but doing it this way gives you a lot of control over how the camera behaves.

     For every of those smoothed values we calculate the wanted value and the current value.
     Then we smooth it using the Lerp function.
     Then we apply the smoothed values to the transform's position.
     */

     // The target we are following
     public Transform target;
     // The distance in the x-z plane to the target
     public float distance = 10.0f;
     // the height we want the camera to be above the target
     public float height = 5.0f;
     // How much we
     public float heightDamping = 2.0f;
     public float rotationDamping = 3.0f;

    [Header("Debug Settings")]
    [SerializeField] bool lockZaxis;
    [SerializeField] bool multiplayer;
    [SerializeField] private AudioMovement player;
    float playerSpeed;


    private Vector3 inGameVector = new Vector3(-44.2999992f, 17.5976677f, 2.99274302f);
    private Vector3 inGameEuler = new Vector3(36.3675804f, 90, 0);//Euler
    private Quaternion ingameQuaternion = new Quaternion(0.220664084f, 0.671794176f, -0.220664084f, 0.671794176f);

    void Start()
    {
        //transform.LookAt(target);

        //player = target.gameObject.GetComponent<AudioMovement>();
        if(player != null)
        playerSpeed = player.maximumForwardSpeed;

        //transform.position -= transform.eulerAngles.y * Vector3.forward * distance;
    }

     void  FixedUpdate ()
     {
         // Early out if we don't have a target
         if (!target && player != null)
             return;

         // Calculate the current rotation angles
         //float wantedRotationAngle = target.eulerAngles.y;
         //float wantedHeight = target.position.y;
         //float wantedHeight = target.position.y + height;
         float currentRotationAngle = transform.eulerAngles.y;
         float currentHeight = transform.position.y;

         // Damp the rotation around the y-axis
         //currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

         // Damp the height
         //currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);

         // Convert the angle into a rotation
         Quaternion currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);

        // Set the position of the camera on the x-z plane to:
        // distance meters behind the target
        if (multiplayer && player.isMoving)
        {
            //Debug.Log("LOLOLOLOLOL");
            //transform.position = Vector3.forward * 1 * Time.deltaTime;
            //transform.Translate((transform.forward * playerSpeed) * Time.fixedDeltaTime, Space.World);
            transform.position += Vector3.right * (playerSpeed * 1.111f)* Time.fixedDeltaTime;
        }
        else
        {
            //transform.position = target.position;
            //transform.position -= currentRotation * Vector3.forward * distance;
        }


        // Set the height of the camera
        //
        if (lockZaxis)
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
         else
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        // Always look at the target
        //transform.LookAt (target);
    }

    public void InitiateGameCam()
    {
        //transform.position = inGameVector;

            //transform.localPosition = Vector3.MoveTowards(transform.localPosition, inGameVector, 1000);

        //transform.rotation = ingameQuaternion;
        transform.rotation = Quaternion.Euler(36.3675804f, 90, 0);
    }
}
