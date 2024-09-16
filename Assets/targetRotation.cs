using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetRotation : MonoBehaviour
{

  public AudioMovement3 move;

    void Update()
    {
      this.transform.Rotate(0,0,move.yAngle, Space.Self);
    }
}
