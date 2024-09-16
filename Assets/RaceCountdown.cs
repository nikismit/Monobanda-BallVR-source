using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceCountdown : MonoBehaviour
{
  public AudioMovement Player1;
  public AudioMovementPlayer2 Player2;

  public GameObject No5;
  public GameObject No4;
  public GameObject No3;
  public GameObject No2;
  public GameObject No1;
  public GameObject GO;
  public GameObject Player1Slider;
  public GameObject Player2Slider;

  public float timer;

    void Start()
    {
      No5.SetActive(false);
      No4.SetActive(false);
      No3.SetActive(false);
      No2.SetActive(false);
      No1.SetActive(false);
      GO.SetActive(false);
      Player1Slider.SetActive(false);
      Player2Slider.SetActive(false);
      timer = 6f;
    }

    void FixedUpdate()
    {
      //Freezes cars until the countdown is over
      if (timer > 0f){
        timer -= 0.02f;
        Player1.currentSpeed = 0f;
        Player2.currentSpeed = 0f;
      }
      //Switched Between The Countdown Ui Elements
      if(timer>5f){
        No5.SetActive(true);
      }
      else if(timer>4f && timer<5f){
        No5.SetActive(false);
        No4.SetActive(true);
      }
      else if(timer>3f && timer<4f){
        No4.SetActive(false);
        No3.SetActive(true);
      }
      else if(timer>2f && timer<3f){
        No3.SetActive(false);
        No2.SetActive(true);
      }
      else if(timer>1f && timer<2f){
        No2.SetActive(false);
        No1.SetActive(true);
      }
      else if (timer>0f && timer<1f){
        No1.SetActive(false);
        GO.SetActive(true);
      }
      else if(timer<=0f){
        GO.SetActive(false);
        Player1Slider.SetActive(true);
        Player2Slider.SetActive(true);
      }
    }
}
