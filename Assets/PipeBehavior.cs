using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Audio;

public class PipeBehavior : MonoBehaviour
{
    Transform pipeEnter;
    [SerializeField]Transform pipeExit;
    AudioMovement playerOne;
    AudioMovementPlayer2 playerTwo;
    public AudioSource pipeEnterSound;
    public AudioSource pipeExitSound;
    [Space(5)]
    [SerializeField] VisualEffect poofEffect;

    [Space(10)]
    public GameObject[] flames;
    [SerializeField] Outline[] outlines;

    // Start is called before the first frame update
    void Start()
    {
        pipeEnter = GetComponent<Transform>();
        //pipeExit = GetComponentInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        pipeEnterSound.Play();
        if (other.gameObject.tag == "Player")
        {
            if (other.GetComponent<AudioMovement>())
            {
                playerOne = other.GetComponent<AudioMovement>();
                StartCoroutine(PipeTravel(other.GetComponent<Transform>()));
            }
            if (other.GetComponent<AudioMovementPlayer2>())
            {
                playerTwo = other.GetComponent<AudioMovementPlayer2>();
                StartCoroutine(PipeTravelTwo(other.GetComponent<Transform>()));
            }
        }
    }

    IEnumerator PipeTravel(Transform playerTrans)
    {
        //Debug.Log("YOOLO");
        playerOne.isInPipe = true;
        outlines[0].OutlineWidth = 0;
        flames[0].layer = 0;

        float elapsedTime = 0;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime;
            playerTrans.position = new Vector3(pipeEnter.position.x + elapsedTime * 30, pipeEnter.position.y, pipeEnter.position.z);

            yield return null;

        }

        yield return new WaitForSeconds(1);
        poofEffect.Play();
        playerTrans.position = pipeExit.position;
        outlines[0].OutlineWidth = 4;
        flames[0].layer = 9;
        playerOne.ExitPipe();
        playerOne.isInPipe = false;
        pipeExitSound.Play();
    }

    IEnumerator PipeTravelTwo(Transform playerTrans)
    {
        //Debug.Log("YOOLO");
        playerTwo.isInPipe = true;
        outlines[1].OutlineWidth = 0;
        flames[1].layer = 0;

        float elapsedTime = 0;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime;
            playerTrans.position = new Vector3(pipeEnter.position.x + elapsedTime * 30, pipeEnter.position.y, pipeEnter.position.z);

            yield return null;

        }

        yield return new WaitForSeconds(1);
        poofEffect.Play();
        playerTrans.position = pipeExit.position;
        outlines[1].OutlineWidth = 4;
        flames[1].layer = 9;
        playerTwo.ExitPipe();
        playerTwo.isInPipe = false;
        pipeExitSound.Play();
    }
}
