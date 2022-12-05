using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleLimiter : MonoBehaviour
{

  // public Transform tracking;
  private float absTurnVal;
  public AudioMovement angle;
  public float timer;
  public float output = 1f;
  [Range(0f,90f)]
  public float angleOfMovement;

    void FixedUpdate()
    {
      if (Mathf.Abs(angle.currentTurn)>0.1f){
        timer += angle.turningSpeed * output * angle.currentTurn * Time.fixedDeltaTime;
      }
      else{
        timer = 0f;
      }
      if (Mathf.Abs(timer)>angleOfMovement && Mathf.Abs(timer)<=angleOfMovement+10f){
        if(output>0.1f){
          output-=0.01f;
        }
      }
      else if (Mathf.Abs(timer)>angleOfMovement+10f){
        output = 0f;
    }
    else{
      output = 1f;
    }
  }
}
