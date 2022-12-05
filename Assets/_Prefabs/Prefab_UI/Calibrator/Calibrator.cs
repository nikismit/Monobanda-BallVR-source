using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calibrator : MonoBehaviour
{
    [SerializeField] int playerInt;
    [Space(20)]
    private AudioMovement[] players = new AudioMovement[2];

    float minPitch;
    float maxPitch;

    private float timer = 0;
    private float maxTime = 3;
    private bool stopCalPitch = false;

    private int[] pitchCount = new int[2];
    private float currentVel = 0;

    [SerializeField] Slider[] loadSliders;

    [SerializeField] RectTransform barScaler;
    [SerializeField] Slider pitchSlider;

    [SerializeField] Image backgroundImage;

    void Start()
    {
        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");

        loadSliders[0].maxValue = maxTime;
        loadSliders[1].maxValue = maxTime;

        for (int i = 0; i < player.Length; i++)
        {
            players[i] = player[i].GetComponent<AudioMovement>();
        }


        pitchSlider.maxValue = players[0].maximumPitch;
    }

    void Update()
    {
        if(pitchCount[0] == 0 && !stopCalPitch)
            CalibratePitch(playerInt, 0);
        if (pitchCount[0] == 1 && !stopCalPitch)
            CalibratePitch(playerInt, 1);
        if (pitchCount[0] == 2 && !stopCalPitch)
        {
            //StartGame
            gameObject.SetActive(false);
        }
    }

    void CalibratePitch(int player, int minMax)
    {
        loadSliders[pitchCount[player]].value = timer;

        if (players[player].currentAmp >= -25 && timer <= 5)
        {
            pitchSlider.value = SmoothPitch(players[player].currentPitch);

            if (players[player].currentPitch > minPitch && players[player].currentPitch < maxPitch)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = 0;
                minPitch = players[player].currentPitch - 5;
                maxPitch = players[player].currentPitch + 5;
            }
        }
        else
            pitchSlider.value = SmoothPitch(0);

        if (timer >= maxTime)
        {
            Debug.Log("PitchCount = " + pitchCount[0] + "PitchSet = " + players[player].currentPitch);

            stopCalPitch = true;
            timer = 0;
            players[player].SetPitchVal(minMax);
            StartCoroutine(NextPitch(player));
        }
    }

    IEnumerator NextPitch(int player)
    {
        backgroundImage.color = Color.green;
        yield return new WaitForSeconds(4);
        backgroundImage.color = Color.white;
        pitchCount[player]++;

        stopCalPitch = false;
    }

    float SmoothPitch(float val)
    {
        float currentPitch = Mathf.SmoothDamp(pitchSlider.value, val, ref currentVel, 5 * Time.deltaTime);
        return currentPitch;
    }
}
