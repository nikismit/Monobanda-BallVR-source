using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlanes : MonoBehaviour
{
    private TutorialHandler tutHandler;
    private ParticleSystem activateParticle;
    [Header("Debug")]
    [SerializeField] private AudioMovement audioMovement;
    [SerializeField] private GameObject[] readyUI;
    [SerializeField] private int UIid;

    private int playerNum;
    private int hasEntered = 0;

    void OnEnable()
    {
        audioMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioMovement>();
        //Debug.Log("AUDIO MOVE = " + audioMovement);
        //readyUI = GameObject.FindGameObjectsWithTag("StartUI");
        activateParticle = GetComponentInChildren<ParticleSystem>();
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


            if (hasEntered >= playerNum - 1 && !hasActivated && audioMovement.debugKeyControl || hasEntered >= playerNum && !hasActivated && !audioMovement.debugKeyControl)
            //if (hasEntered >= playerNum)
            {
                activateParticle.Play();
                hasActivated = true;
                tutHandler.StartCoroutine(tutHandler.Hold());
                Invoke("DisableSelf", 1);
            }

            if (other.gameObject.GetComponent<AudioMovement>() && tutHandler.activatePlane >= 2)
            {
                //activateParticle.Play();
                //tutHandler.StartCoroutine(tutHandler.Hold());
                //Invoke("DisableSelf", 1);
                readyUI[0].SetActive(true);
            }
            if (other.gameObject.GetComponent<AudioMovementPlayer2>() && tutHandler.activatePlane >= 2)
            {
                //activateParticle.Play();
                //tutHandler.StartCoroutine(tutHandler.Hold());
                //Invoke("DisableSelf", 1);
                readyUI[1].SetActive(true);
            }


            /*
            else if (hasEntered >= playerNum && tutHandler.activatePlane >= 2)
            {
                tutHandler.StartCoroutine(tutHandler.Hold());
                Invoke("DisableSelf", 1);

            }
            */
        }     //tutHandler.StartCoroutine(tutHandler.Hold());        
    }

    void DisableSelf()
    {
        gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        //if(UIid == 1)
        for (int i = 0; i < readyUI.Length; i++)
        readyUI[i].SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            hasEntered--;

        if (other.gameObject.GetComponent<AudioMovement>() && tutHandler.activatePlane >= 2)
        {
            //tutHandler.StartCoroutine(tutHandler.Hold());
            //Invoke("DisableSelf", 1);
            readyUI[0].SetActive(false);
        }
    }
}
