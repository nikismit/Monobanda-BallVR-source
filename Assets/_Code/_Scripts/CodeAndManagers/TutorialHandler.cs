using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class TutorialHandler : MonoBehaviour
{
    [SerializeField] RaceCountdown countDown;
    [SerializeField] DemoUI demo;
    [SerializeField] GameObject[] movingRoads;

    [SerializeField] GameObject[] tutorialPlane;
    [HideInInspector] public int activatePlane = 0;

    [SerializeField] private GameObject[] startCanvasUIs;
    [SerializeField] private GameObject[] player2UI;
    [SerializeField] private SFXManager soundManager;
    private AudioSource tutorialSound;

    [HideInInspector] public int calibratorsComplete;

    private string playerMic;

    [Header("Debug")]
    public bool androidDebug;

    private void Start()
    {
        playerMic = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioMovement>().pitch.selectedDevice;
        Debug.Log("Selected device = " + playerMic);
        tutorialSound = GetComponent<AudioSource>();

        if (androidDebug)
        {
            for (int i = 0; i < player2UI.Length; i++)
            {
                player2UI[i].SetActive(false);
            }
        }
        //SpawnPlane();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
            demo.RemoveDemoUIEvent();
    }

    public void SpawnPlane()
    {
        //if (androidDebug)
            demo.RemoveDemoUIEvent();
        /*
        if (activatePlane == 0)
            tutorialPlane[0].SetActive(true);
        if (activatePlane == 1)
            tutorialPlane[1].SetActive(true);
        if (activatePlane == 2)
            tutorialPlane[2].SetActive(true);

        if (activatePlane >= tutorialPlane.Length)
            demo.RemoveDemoUIEvent();
        */
    }

    public IEnumerator Hold()
    {
        tutorialSound.Play();
        yield return new WaitForSeconds(3);
        activatePlane++;
        SpawnPlane();
    }

    public void RemoveRoads()
    {
        if (!countDown.startCountDown)
        {
            countDown.startCountDown = true;
            soundManager.StartGame();

            for (int i = 0; i < movingRoads.Length; i++)
            {
                movingRoads[i].SetActive(false);
            }
        }
    }
}
