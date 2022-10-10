using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHandler : MonoBehaviour
{
    [SerializeField] RaceCountdown countDown;
    [SerializeField] GameObject[] movingRoads;

    [SerializeField] GameObject[] tutorialPlane;
    private int activatePlane = 0;

    public void InitializeTutorial()
    {
        SpawnPlane();
    }

    public void SpawnPlane()
    {
        for (int i = 0; i < tutorialPlane.Length; i++)
        {
            if (activatePlane == i)
                tutorialPlane[i].SetActive(true);
            else
                tutorialPlane[i].SetActive(false);
        }

        //activatePlane++;

        if (activatePlane >= tutorialPlane.Length)
        {
            StartCoroutine(HoldStart());
        }
    }

    public IEnumerator Hold()
    {
        Debug.Log("holding");
        yield return new WaitForSeconds(1);
        activatePlane++;
        SpawnPlane();
    }

    public IEnumerator HoldStart()
    {
        yield return new WaitForSeconds(1);
        RemoveRoads();
        countDown.startCountDown = true;
        //SpawnPlane();
    }

    void RemoveRoads()
    {
        for (int i = 0; i < movingRoads.Length; i++)
        {
            movingRoads[i].SetActive(false);
        }
    }
}
