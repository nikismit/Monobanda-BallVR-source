﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAnimationSinglePlayer : MonoBehaviour
{
    public AudioMovement P1;
    //public AudioMovementPlayer2 P2;
    public GameObject Player;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      if (Player.name == "Player1Model"){
        var target = P1.currentTurn*-10f;
        Player.transform.localRotation = Quaternion.Euler(0, 0, target);
      }
      // else if (Player.name == "Player2Model"){
      //   var target = P2.currentTurn*-10f;
      //   Player.transform.localRotation = Quaternion.Euler(0, 0, target);
      // }
    }
}
