using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class DemoUI : MonoBehaviour
{
    //[SerializeField] CanvasGroup canvasGroup;
    [SerializeField] RaceCountdown countDown;
    [SerializeField] TutorialHandler tutHandler;
    [SerializeField] GameObject transitionUI;
    [SerializeField] GameObject demoCanvas;
    ///[SerializeField] TutorialHandler tutHandler;
    [HideInInspector] RectTransform uiTransform;
    [HideInInspector] CanvasGroup uiFade;

    AudioMovement player1;
    AudioMovementPlayer2 player2;


    float scale = 0;

    private bool startTut = false;
    void Start()
    {
        uiTransform = transitionUI.GetComponent<RectTransform>();
        uiFade = transitionUI.GetComponent<CanvasGroup>();

        uiTransform.sizeDelta = new Vector2(scale, scale);

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
        //tutHandler.InitializeTutorial();
    }

    bool courotineStarted = false;

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKey(KeyCode.Space))
        {
            RemoveDemoUIEvent();
        }
        
        */
        /*
        if (startTut && canvasGroup.alpha < 1)
        {
            canvasGroup.alpha -= Time.deltaTime / 2;
        }
        */

        if (startTut && scale <= 50)
        {
            scale += 60 * Time.deltaTime;
            uiTransform.sizeDelta = new Vector2(scale * 55, scale * 55);
            uiTransform.Rotate(Vector3.forward, scale * 5);
        }
        else if (scale >= 50)
        {
            tutHandler.RemoveRoads();
            demoCanvas.SetActive(false);
            uiFade.alpha -= Time.deltaTime / 3;
        }
        //Debug.Log("KLaarjonge");
    }
}
