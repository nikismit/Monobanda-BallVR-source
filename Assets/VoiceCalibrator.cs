using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoiceCalibrator : MonoBehaviour
{
    AudioMovement playerOne;
    AudioMovementPlayer2 playerTwo;

    //public AudioPitch_MenuPlayer2 pitch;
    [SerializeField] Slider[] sliders;// 0/1 = player1 min/max, 2/3 = player2 min/max
    [SerializeField] Slider[] currentPitchSliders;

    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<AudioMovement>())
                playerOne = players[i].GetComponent<AudioMovement>();

            if (players[i].GetComponent<AudioMovementPlayer2>())
                playerTwo = players[i].GetComponent<AudioMovementPlayer2>();
        }

        sliders[0].value = playerOne.minimumPitch; 
        sliders[1].value = playerOne.maximumPitch;

        sliders[2].value = playerTwo.minimumPitch;
        sliders[3].value = playerTwo.maximumPitch;

        InvokeRepeating("UpdateValues", 1, 0.5f);
    }

    void UpdateValues()
    {
        currentPitchSliders[0].value = playerOne.currentPitch;
        currentPitchSliders[1].value = playerTwo.currentPitch;

        playerOne.minimumPitch = sliders[0].value;
        playerOne.maximumPitch = sliders[1].value;

        playerTwo.minimumPitch = sliders[2].value;
        playerTwo.maximumPitch = sliders[3].value;
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
