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
    [SerializeField] private int UIid;
    [SerializeField] private Text PitchCountdownText;
    [SerializeField] private Transform circle;
    [SerializeField] private SpriteRenderer outerStar;
    private Color goneCol = new Color(1, 1, 1, 0);

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

            if (hasEntered >= playerNum - 1 && !hasActivated && audioMovement.debugKeyControl ||
                hasEntered >= playerNum && !hasActivated && !audioMovement.debugKeyControl)
            {
                StartCoroutine(Scale(circle));
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
        }
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

    IEnumerator Scale(Transform target)
    {
        Transform targetRef = target;
        float elapsedTime = 0;



        while (elapsedTime < 60)
        {
            elapsedTime += Time.deltaTime;
            float lerp = Mathf.Lerp(targetRef.localScale.x, 0.01f, elapsedTime / 60);
            float lerpCol = Mathf.Lerp(0, 1, elapsedTime / 60);
            outerStar.color = Color.Lerp(outerStar.color, goneCol, lerpCol * 2);
            target.localScale = new Vector3(lerp, lerp, lerp);

            yield return null;
        }
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

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            hasEntered--;
    }
}
