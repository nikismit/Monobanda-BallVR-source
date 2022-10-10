using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class DemoUI : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] RaceCountdown countDown;
    [SerializeField] TutorialHandler tutHandler;
    
    AudioMovement player1;
    AudioMovementPlayer2 player2;

    private bool startTut = false;
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

    public void RemoveDemoUIEvent()
    {
        startTut = true;
        //SceneManager.LoadScene("EndlessRunnerTEST_MainMultiplayer");    
        //countDown.startCountDown = true;
        tutHandler.InitializeTutorial();
    }



    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKey(KeyCode.Space))
        {
            RemoveDemoUIEvent();
        }
        
        if (startTut && canvasGroup.alpha >= 0)
        {
            canvasGroup.alpha -= Time.deltaTime;
        }
    }
}
