using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlanes : MonoBehaviour
{
    private TutorialHandler tutHandler;

    private int playerNum;
    private int hasEntered = 0;

    void OnEnable()
    {
        playerNum = GameObject.FindGameObjectsWithTag("Player").Length;
        tutHandler = gameObject.GetComponentInParent<TutorialHandler>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            hasEntered++;
            Debug.Log("Has Entered" + hasEntered);
            if (hasEntered >= playerNum - 1)
            {
                tutHandler.StartCoroutine(tutHandler.Hold());
            }
                //tutHandler.startHoldCourotine();
                //tutHandler.StartCoroutine(tutHandler.Hold());
                //tutHandler.SpawnPlane();

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            hasEntered--;
    }
}
