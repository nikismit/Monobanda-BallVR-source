using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rollScript : MonoBehaviour
{

  public float Angle;
  public AudioMovement3 move;
  float target;
  public float tiltSpeed;

    void FixedUpdate()
    {

      if (-move.yAngle*Angle > target){
        target += tiltSpeed;
      }
      else{
        target -= tiltSpeed;
      }
        // transform.localRotation = Quaternion.Euler(0, 180, -move.yAngle*Angle);
        transform.localRotation = Quaternion.Euler(0, 180, target);

    }
}
