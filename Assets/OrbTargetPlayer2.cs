using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbTargetPlayer2 : MonoBehaviour
{
  public AudioMovementPlayer2 audioMove;
  public float orbTarget;

    void Start()
    {
      orbTarget = 0f;
    }

    void Update()
    {
      if(audioMove.currentAmp > 0f){
        orbTarget = audioMove.currentTurn * -4.5f;
        this.transform.localPosition = new Vector3 (0f, 0f, orbTarget);
      }
      else{
        this.transform.localPosition = new Vector3 (0f, 0f, 0f);
      }
    }
}
