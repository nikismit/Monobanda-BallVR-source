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
    private AudioSource tutorialSound;

    private void Start()
    {
        tutorialSound = GetComponent<AudioSource>();
        //startCanvasUIs = GameObject.FindGameObjectsWithTag("StartUI");
        SpawnPlane();
    }

    public void InitializeTutorial()
    {
        //SpawnPlane();
    }

    public void SpawnPlane()
    {
        /*
        for (int i = 0; i < tutorialPlane.Length; i++)
        {
            if (activatePlane == i)
                tutorialPlane[i].SetActive(true);
            else
                tutorialPlane[i].SetActive(false);
        }
        */

        if(activatePlane == 0)
        {
            tutorialPlane[0].SetActive(true);
            //tutorialPlane[1].SetActive(true);
        }
        if (activatePlane == 1)
        {
            //tutorialPlane[0].SetActive(true);
            tutorialPlane[1].SetActive(true);
        }
        if (activatePlane == 2)
            tutorialPlane[2].SetActive(true);

        //activatePlane++;

        if (activatePlane >= tutorialPlane.Length)
        {
            //StartCoroutine(HoldStart());
            demo.RemoveDemoUIEvent();
        }
    }

    public IEnumerator Hold()
    {
        //Debug.Log("holding");
        tutorialSound.Play();
        yield return new WaitForSeconds(3);
        activatePlane++;
        SpawnPlane();
    }

    public IEnumerator HoldStart()
    {
        //StartCoroutine(ReadyPopUp());
        yield return new WaitForSeconds(1);
        //RemoveRoads();
        countDown.startCountDown = true;
        //SpawnPlane();
    }

    public IEnumerator ReadyPopUp()
    {
        for (int i = 0; i < startCanvasUIs.Length; i++)
        {
            startCanvasUIs[i].SetActive(true);
        }
        yield return new WaitForSeconds(2);
        //RemoveRoads();
        for (int i = 0; i < startCanvasUIs.Length; i++)
        {
            startCanvasUIs[i].SetActive(false);
        }
        countDown.startCountDown = true;
        //SpawnPlane();
    }

    public void RemoveRoads()
    {
        countDown.startCountDown = true;

        for (int i = 0; i < movingRoads.Length; i++)
        {
            movingRoads[i].SetActive(false);
        }
    }
}
