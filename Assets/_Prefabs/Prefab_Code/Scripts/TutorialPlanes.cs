using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPlanes : MonoBehaviour
{
    private TutorialHandler tutHandler;
    private ParticleSystem activateParticle;
    [Header("Debug")]
    [SerializeField] private AudioMovement audioMovement;
    [SerializeField] private AudioMovementPlayer2 audioMovement2;
    [SerializeField] private GameObject[] readyUI;
    [SerializeField] private int UIid;
    [SerializeField] private Text PitchCountdownText;

    private int playerNum;
    private int hasEntered = 0;

    [HideInInspector] public int waitSec = 3;

    void OnEnable()
    {
        //audioMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioMovement>();
        //audioMovement2 = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioMovementPlayer2>();
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


            //if (hasEntered >= playerNum - 1 && !hasActivated && audioMovement.debugKeyControl || hasEntered >= playerNum && !hasActivated && !audioMovement.debugKeyControl)
            if (hasEntered >= playerNum - 1 && !hasActivated && audioMovement.debugKeyControl ||
                hasEntered >= playerNum && !hasActivated && !audioMovement.debugKeyControl)
            //if (hasEntered >= playerNum && !hasActivated)
            {
                activateParticle.Play();
                hasActivated = true;
                //tutHandler.StartCoroutine(tutHandler.Hold());
                if (tutHandler.activatePlane <= 2)
                {
                    LastInvoke();
                }
                else
                    Invoke("DisableSelf", waitSec);
                //other.GetComponentInChildren<PitchCalibrateCountDown>();
                //tutHandler.StartCoroutine(HoldPitchCountdown());
                //Invoke("DisableSelf", 1);
            }

            if (other.gameObject.GetComponent<AudioMovement>() && tutHandler.activatePlane <= 2 && audioMovement.debugKeyControl ||
                tutHandler.activatePlane <= 2 && hasEntered  > 1 && !audioMovement.debugKeyControl)
            {
                //activateParticle.Play();
                //tutHandler.StartCoroutine(tutHandler.Hold());
                //Invoke("DisableSelf", 1);
                readyUI[0].SetActive(true);
            }
            if (other.gameObject.GetComponent<AudioMovementPlayer2>() && tutHandler.activatePlane <= 2)
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

    IEnumerator HoldPitchCountdown()
    {
        PitchCountdownText.text = "HOLD PITCH   3";
        yield return new WaitForSeconds(1);
        PitchCountdownText.text = "HOLD PITCH   2";
        yield return new WaitForSeconds(1);
        PitchCountdownText.text = "HOLD PITCH   1";
        yield return new WaitForSeconds(1);
        PitchCountdownText.text = "";
        tutHandler.StartCoroutine(tutHandler.Hold());
        Invoke("DisableSelf", 0);

        //return null;
    }

    void LastInvoke()
    {
        tutHandler.StartCoroutine(tutHandler.Hold());
        Invoke("DisableSelf", waitSec);
    }

    void DisableSelf()
    {
            if (tutHandler.activatePlane <= 0)
            {
                audioMovement.SetMinPitchVal();
                audioMovement2.SetMinPitchVal();
            }
            if (tutHandler.activatePlane == 1)
            {
                audioMovement.SetMaxPitchVal();
                audioMovement2.SetMaxPitchVal();
            }


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
