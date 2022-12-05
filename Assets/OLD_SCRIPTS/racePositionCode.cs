using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class racePositionCode : MonoBehaviour
{
  public collisionAdjustmentScriptPlayer1 P1;
  public collisionAdjustmentScriptPlayer2 P2;
  public Slider SliderP1;
  public Slider SliderP2;

    void FixedUpdate()
    {
      SliderP1.value = P1.P1Pos;
      SliderP2.value = P2.P2Pos;
    }
}
