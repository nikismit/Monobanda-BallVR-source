using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Audio;

public class PipeBehavior : MonoBehaviour
{
    Transform pipeEnter;
    [SerializeField]Transform pipeExit;
    //AudioMovementPlayer2 playerTwo;
    [SerializeField] float travelTime = 1;

    [Space(5)]
    [SerializeField] VisualEffect poofEffect;
    public AudioSource pipeEnterSound;
    public AudioSource pipeExitSound;

    [Space(10)]
    public GameObject[] flames;
    [SerializeField] Outline[] outlines;

    [SerializeField] GameObject poofPrefab;

    // Start is called before the first frame update
    void Start()
    {
        pipeEnter = GetComponent<Transform>();
        //pipeExit = GetComponentInChildren<Transform>();
    }

    private void OnTriggerEnter(Collider other)
    {
        pipeEnterSound.Play();
        if (other.gameObject.tag == "Player")
        {
            if (other.GetComponent<AudioMovement>())
            {
                AudioMovement otherPlayer = other.GetComponent<AudioMovement>();
                StartCoroutine(PipeTravel(otherPlayer, other.GetComponent<Transform>()));
            }
        }
    }

    IEnumerator PipeTravel(AudioMovement player, Transform playerTrans)
    {
        //Debug.Log("YOOLO");
        player.isInPipe = true;
        outlines[0].OutlineWidth = 0;
        flames[0].layer = 0;

        float elapsedTime = 0;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime;
            playerTrans.position = new Vector3(pipeEnter.position.x + elapsedTime * 30, pipeEnter.position.y, pipeEnter.position.z);

            yield return null;
        }

        yield return new WaitForSeconds(travelTime);

        //poofEffect.Play();
        Instantiate(poofPrefab, pipeExit);

        playerTrans.position = pipeExit.position;
        outlines[0].OutlineWidth = 4;
        flames[0].layer = 9;
        player.ExitPipe();
        player.isInPipe = false;
        pipeExitSound.Play();
    }
}
