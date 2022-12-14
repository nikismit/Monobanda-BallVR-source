using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoiceCalibrator : MonoBehaviour
{
    [SerializeField] AudioMovement[] players = new AudioMovement[2];
    //AudioMovementPlayer2 playerTwo;

    //public AudioPitch_MenuPlayer2 pitch;
    [SerializeField] Slider[] sliders;// 0/1 = player1 min/max, 2/3 = player2 min/max
    [SerializeField] Slider[] currentPitchSliders;

    void Start()
    {
        //players = FindObjectsOfType<AudioMovement>();

        sliders[0].value = players[0].minimumPitch; 
        sliders[1].value = players[0].maximumPitch;

        sliders[2].value = players[1].minimumPitch;
        sliders[3].value = players[1].maximumPitch;

        InvokeRepeating("UpdateValues", 1, 0.5f);
    }

    void UpdateValues()
    {
        currentPitchSliders[0].value = players[0].currentPitch;
        currentPitchSliders[1].value = players[1].currentPitch;

        players[0].minimumPitch = sliders[0].value;
        players[0].maximumPitch = sliders[1].value;

        players[1].minimumPitch = sliders[2].value;
        players[1].maximumPitch = sliders[3].value;
    }

    /*
    private void Update()
    {
        currentPitchSliders[0].value = playerOne.currentPitch;
        currentPitchSliders[1].value = playerTwo.currentPitch;

        playerOne.minimumPitch = sliders[0].value;
        playerOne.maximumPitch = sliders[1].value;

        playerTwo.minimumPitch = sliders[2].value;
        playerTwo.maximumPitch = sliders[3].value;
    }
    */
}
