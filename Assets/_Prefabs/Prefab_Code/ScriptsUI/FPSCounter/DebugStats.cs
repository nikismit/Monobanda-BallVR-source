using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugStats : MonoBehaviour
{
    [SerializeField] AudioMovement audioPlayerOne;
    [SerializeField] AudioMovementPlayer2 audioPlayerTwo;

    /* Assign this script to any object in the Scene to display frames per second */
    bool isActive = false;


    public float updateInterval = 0.5f; //How often should the number update

    float accum = 0.0f;
    int frames = 0;
    float timeleft;
    float fps;

    GUIStyle textStyle = new GUIStyle();

    // Use this for initialization
    void Start()
    {
        timeleft = updateInterval;

        textStyle.fontStyle = FontStyle.Bold;
        textStyle.normal.textColor = Color.white;
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

    void OnGUI()
    {
        if (isActive)
        {
            //Display the fps and round to 2 decimals
            GUI.Label(new Rect(5, 5, 100, 25), fps.ToString("F2") + "FPS", textStyle);
            GUI.Label(new Rect(5, 20, 100, 25), "P1 " + audioPlayerOne.minimumPitch + " MinPitch, " + audioPlayerOne.maximumPitch + " MaxPitch, " + audioPlayerOne.currentPitch + " Current", textStyle);
            GUI.Label(new Rect(5, 35, 100, 25), "P2 " + audioPlayerTwo.minimumPitch + " MinPitch, " + audioPlayerTwo.maximumPitch + " MaxPitch, " + audioPlayerTwo.currentPitch + " Current", textStyle);
        }
    }
}
