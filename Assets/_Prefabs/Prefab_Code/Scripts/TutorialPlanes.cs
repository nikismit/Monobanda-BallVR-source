using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlanes : MonoBehaviour
{
    private TutorialHandler tutHandler;
    [Header("Debug")]
    [SerializeField] private AudioMovement audioMovement;

    private int playerNum;
    private int hasEntered = 0;

    void OnEnable()
    {
        playerNum = GameObject.FindGameObjectsWithTag("Player").Length;
        tutHandler = gameObject.GetComponentInParent<TutorialHandler>();
    }

    bool hasActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            hasEntered++;
            Debug.Log("Has Entered" + hasEntered);


            if (hasEntered >= playerNum - 1 && !hasActivated)
            //if (hasEntered >= playerNum)
            {
                hasActivated = false;
                tutHandler.StartCoroutine(tutHandler.Hold());
                Invoke("DisableSelf", 1);
            }

            /*
            else if (hasEntered >= playerNum)
            {
                tutHandler.StartCoroutine(tutHandler.Hold());
                Invoke("DisableSelf", 1);

            }*/
        }     //tutHandler.StartCoroutine(tutHandler.Hold());        
    }

    void DisableSelf()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            hasEntered--;
    }
}
