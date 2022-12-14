using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPlanes : MonoBehaviour
{
    private TutorialHandler tutHandler;
    private ParticleSystem activateParticle;
    [Header("Debug")]
    [SerializeField] private AudioMovement[] audioMovement;
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

            if (hasEntered >= playerNum - 1 && !hasActivated && audioMovement[0].debugKeyControl ||
                hasEntered >= playerNum && !hasActivated && !audioMovement[0].debugKeyControl)
            {
                StartCoroutine(Scale(circle));
                activateParticle.Play();
                hasActivated = true;

                if (tutHandler.activatePlane <= 2)
                    LastInvoke();
                else
                    Invoke("DisableSelf", waitSec);
            }
        }
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
        gameObject.SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            hasEntered--;
    }
}
