using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceCountdownSinglePlayer : MonoBehaviour
{
    public List<GameObject> rbCars;
    GameObject[] cars;

    public AudioMovement Player1;
  public AiPathFollowDriver Player2;
  public AiPathFollowDriver Player3;
  public AiPathFollowDriver Player4;
  public AiPathFollowDriver Player5;
  public AiPathFollowDriver Player6;

  Rigidbody rb1;
  Rigidbody rb2;
  Rigidbody rb3;
  Rigidbody rb4;
  Rigidbody rb5;
  Rigidbody rb6;

  public GameObject No5;
  public GameObject No4;
  public GameObject No3;
  public GameObject No2;
  public GameObject No1;
  public GameObject GO;
  public GameObject Player1Slider;
  public GameObject Player2Slider;

  public float timer;

    bool raceStarted = false;

    void Start()
    {
        cars = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < cars.Length; i++)
        {
            Rigidbody rb = cars[i].gameObject.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeAll;
            //Debug.LogWarning(cars[i] + " / " + rb);
        }

      //rbCars.Add(  GameObject.FindGameObjectsWithTag("Player"));

      No5.SetActive(false);
      No4.SetActive(false);
      No3.SetActive(false);
      No2.SetActive(false);
      No1.SetActive(false);
      GO.SetActive(false);
      Player1Slider.SetActive(false);
      Player2Slider.SetActive(false);
      timer = 6f;
    }

    void FixedUpdate()
    {
        if (!raceStarted)
        {
            if (timer > 0f)
            {
                timer -= 0.02f;
                Player1.currentSpeed = 0f;
                Player2.currentSpeed = 0f;
                Player3.currentSpeed = 0f;
                Player4.currentSpeed = 0f;
                Player5.currentSpeed = 0f;
                Player6.currentSpeed = 0f;
            }
            //Switched Between The Countdown Ui Elements
            if (timer > 5f)
            {
                No5.SetActive(true);
            }
            else if (timer > 4f && timer < 5f)
            {
                No5.SetActive(false);
                No4.SetActive(true);
            }
            else if (timer > 3f && timer < 4f)
            {
                No4.SetActive(false);
                No3.SetActive(true);
            }
            else if (timer > 2f && timer < 3f)
            {
                No3.SetActive(false);
                No2.SetActive(true);
            }
            else if (timer > 1f && timer < 2f)
            {
                No2.SetActive(false);
                No1.SetActive(true);
            }
            else if (timer > 0f && timer < 1f)
            {
                No1.SetActive(false);
                GO.SetActive(true);
            }
            else if (timer <= 0f)
            {
                GO.SetActive(false);

                for (int i = 0; i < cars.Length; i++)
                {
                    Rigidbody rb = cars[i].gameObject.GetComponent<Rigidbody>();
                    rb.constraints = RigidbodyConstraints.None;
                    rb.constraints = RigidbodyConstraints.FreezeRotationZ;
                    //Debug.LogWarning(cars[i] + " / " + rb);
                }
                //raceStarted = true;
                // Player1Slider.SetActive(true);
                // Player2Slider.SetActive(true);
            }
        }
    }
      //Freezes cars until the countdown is over

}
