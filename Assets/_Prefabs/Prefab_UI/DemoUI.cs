using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class DemoUI : MonoBehaviour
{
    [SerializeField] GameObject[] movingRoads;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] RaceCountdown countDown;
    
    AudioMovement player1;
    AudioMovementPlayer2 player2;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].gameObject.GetComponent<AudioMovement>())
            {
                player1 = players[i].gameObject.GetComponent<AudioMovement>();
            }
            else if (players[i].gameObject.GetComponent<AudioMovementPlayer2>())
            {
                player2 = players[i].gameObject.GetComponent<AudioMovementPlayer2>();
            }
        }
    }

    public void StartGameEvent()
    {
        //SceneManager.LoadScene("EndlessRunnerTEST_MainMultiplayer");    
        countDown.startCountDown = true;

        for (int i = 0; i < movingRoads.Length; i++)
        {
            movingRoads[i].SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            StartGameEvent();
        }
        if (countDown.startCountDown && canvasGroup.alpha >= 0)
        {
            canvasGroup.alpha -= Time.deltaTime;
        }
    }
}
