using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BoosterRing : MonoBehaviour
{
    [SerializeField] GameObject[] ringModels;
    [SerializeField] GameObject cam;

    [SerializeField] float rotateSpeed = 1;

    private GameObject[] players;
    public int[] playerCheck = new int[2];

    float easeOutlength = 2;

    private float timer;
    private AudioSource boostSound;

    private Vector3 sizeRef;

    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        cam = GameObject.FindGameObjectWithTag("MainCamera");

        playerCheck[0] = 0;
        playerCheck[1] = 0;

        sizeRef = transform.localScale;
        //timer = easeOutlenght;
        timer = easeOutlength + 1;
        boostSound = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (timer < easeOutlength)
        {
            timer += Time.deltaTime;

            float ease = Mathf.InverseLerp(0, easeOutlength, timer);

            ringModels[0].transform.Rotate(new Vector3(0, 0, 200));
            
            transform.localScale = sizeRef * 1.2f;

        }
        else
        {
            ringModels[0].transform.Rotate(new Vector3(0, 0, 1));
            transform.localScale = sizeRef;
        }
    }

    void OutCam()
    {
        //if (transform.position.x < cam.transform.position.x)
        //if (Vector3.Distance(transform.position, cam.transform.position) < 10)
        {
            for (int i = 0; i < playerCheck.Length; i++)
            {
                if (playerCheck[i] == 0)
                {
                    AudioMovement audiomove = players[i].GetComponent<AudioMovement>();
                    AudioMovementPlayer2 audiomove2 = players[i].GetComponent<AudioMovementPlayer2>();

                    if (audiomove != null)
                    {
                        audiomove.RemoveRing();
                    }

                    if (audiomove2 != null)
                    {
                        audiomove2.RemoveRing();
                    }
                }    
                    
            }

            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "OutboundCam")
        {
            OutCam();
            return;
        }

        timer = 0;
        boostSound.Play();
        if (other.gameObject == players[0])
        {
            Debug.Log(other);//player2
            playerCheck[0] = 1;
        }
        else if (other.gameObject == players[1])
        {
                Debug.Log(other);//player1
            playerCheck[1] = 1;
        }



    }

    float easeOutQuart(float x){
        return 1 - Mathf.Pow(1 - x, 4);
    }
}
