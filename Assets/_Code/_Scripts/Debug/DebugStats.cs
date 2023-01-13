using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugStats : MonoBehaviour
{
    private AudioMovement[] players = new AudioMovement[2];
    private AudioPitch_Player1[] audioPitches = new AudioPitch_Player1[2];
    [SerializeField] GameObject[] pitchInputs;
    //[SerializeField] AudioMovementPlayer2 audioPlayerTwo;

    /* Assign this script to any object in the Scene to display frames per second */
    bool isActive = false;


    public float updateInterval = 0.5f; //How often should the number update

    float accum = 0.0f;
    int frames = 0;
    float timeleft;
    float fps;

    GUIStyle textStyle = new GUIStyle();

    [SerializeField] Slider[] playerSliders;

    // Use this for initialization
    void Start()
    {
        timeleft = updateInterval;

        textStyle.fontStyle = FontStyle.Bold;
        textStyle.normal.textColor = Color.white;
        textStyle.fontSize = 24;

        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if(player[i] != null)
            {
                players[i] = player[i].GetComponent<AudioMovement>();
                audioPitches[i] = players[i].pitch;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (isActive)
            {
                isActive = false;
            }
            else
            {
                isActive = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!pitchInputs[0].activeSelf)
            {
                for (int i = 0; i < pitchInputs.Length; i++)
                {
                    pitchInputs[i].SetActive(true);
                }
            }
            else
            {
                for (int i = 0; i < pitchInputs.Length; i++)
                {
                    pitchInputs[i].SetActive(false);
                }
            }
        }

        playerSliders[0].value = players[0].currentPitch;

        if(players[1] != null)
        playerSliders[1].value = players[1].currentPitch;


        if (Input.GetKeyDown(KeyCode.F))
        {
            if (Screen.fullScreen)
            {
                Fullscreen(false);
            }
            else
            {
                Fullscreen(true);
            }
        }


        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.LogWarning("DELETING HIGH SCORES...");
            for (int i = 0; i < 6; i++)
            {
                PlayerPrefs.DeleteKey("HighScore" + i);
            }
        }

        if (isActive)
        {
            timeleft -= Time.deltaTime;
            accum += Time.timeScale / Time.deltaTime;
            ++frames;

            // Interval ended - update GUI text and start new interval
            if (timeleft <= 0.0)
            {
                // display two fractional digits (f2 format)
                fps = (accum / frames);
                timeleft = updateInterval;
                accum = 0.0f;
                frames = 0;
            }
        }
    }

    public void Fullscreen(bool fullScreen)
    {
        bool fullScreenMode;

        if (fullScreen)
            fullScreenMode = false;
        else
            fullScreenMode = true;

        Screen.fullScreen = fullScreen;
        Cursor.visible = fullScreenMode;
    }

    public void MobileEnableUI(bool _bool)
    {
        isActive = _bool;
    }

    void OnGUI()
    {
        if (isActive)
        {
            //Display the fps and round to 2 decimals
            GUI.Label(new Rect(5, 5, 150, 25), "debugCommands: R = remove highScores, F = FullScreen, O = skip tutorial, Spacebar = manual calibration", textStyle);
            GUI.Label(new Rect(40, 210, 150, 25), fps.ToString("F2") + "FPS", textStyle);
            GUI.Label(new Rect(5, 55, 150, 25), "P1 " + players[0].minimumPitch + " MinPitch, " + players[0].maximumPitch + " MaxPitch, " + players[0].currentPitch + " Current", textStyle);
            if(players[1] != null)
            GUI.Label(new Rect(5, 80, 150, 25), "P2 " + players[1].minimumPitch + " MinPitch, " + players[1].maximumPitch + " MaxPitch, " + players[1].currentPitch + " Current", textStyle);
            GUI.Label(new Rect(5, 105, 150, 50), "P1 CurrentAmp = " + players[0].currentAmp, textStyle);
            if (players[1] != null)
                GUI.Label(new Rect(5, 130, 150, 50), "P2 CurrentAmp = " + players[1].currentAmp, textStyle);
            GUI.Label(new Rect(5, 155, 150, 50), "P1 Device = " + audioPitches[0].selectedDevice, textStyle);
            if (players[1] != null)
                GUI.Label(new Rect(5, 180, 150, 50), "P2 device = " + audioPitches[1].selectedDevice, textStyle);

        }
    }

    public void MaxPlayerOneEdit(float number)
    {
        SendPitchValue(0, number, 1);
    }

    public void MinPlayerOneEdit(float number)
    {
        SendPitchValue(0, number, 0);
    }

    public void MaxPlayerTwoEdit(float number)
    {
        SendPitchValue(1, number, 1);
    }

    public void MinPlayerTwoEdit(float number)
    {

        SendPitchValue(1, number, 0);
    }

    void SendPitchValue(int player, float pitchVal, int minMax)
    {
        if (minMax == 0)
            players[player].minimumPitch = pitchVal;
        else
            players[player].maximumPitch = pitchVal;
    }
}
