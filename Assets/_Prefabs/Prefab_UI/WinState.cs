using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WinState : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] score;
    private PlayersUIHandler uiHandler;
    public GameObject[] winUI;
    public GameObject[] players;
    DemoUI demoUI;
    public AnimationCurve curve;
    private bool initialized = false;
    private float timer = 0;
    private float timeLength = 3;

    private int winner;

    private void Start()
    {
        demoUI = GetComponent<DemoUI>();
        uiHandler = GetComponent<PlayersUIHandler>();
    }

    private void Update()
    {
        if (timer <= timeLength && initialized)
        {
            timer += Time.unscaledDeltaTime;
            //float ease = Mathf.Lerp(0, easeOutlength / 2, timer * 1.2f);
            //float ease = Mathf.Lerp(0, 3, timer);
            float ease = Mathf.Clamp(timer / 3, 0, 1);
            Time.timeScale = curve.Evaluate(ease);

            if (ease == 1)
            {
                //demoUI.RemoveDemoUIEvent();
                StartCoroutine(StartTransition());
            }

        }
    }

    IEnumerator StartTransition()
    {
        //yield return new WaitForSeconds(1.5f);
        yield return new WaitForSecondsRealtime(1.5f);
        demoUI.RemoveDemoUIEvent();
    }

    public void PlayerOneWins()
    {
        if (!initialized)
        {
            initialized = true;
            winner = 0;
            //Time.timeScale = 0;
            //StartCoroutine(ResetScene(2, 0));
            //Invoke("ResetScene", 2);
        }
    }

    public void ScoreWinner()
    {
        if (uiHandler.score[0] > uiHandler.score[1])
            winner = 0;
        else if (uiHandler.score[0] < uiHandler.score[1])
            winner = 1;
        else if (uiHandler.score[0] == uiHandler.score[1])
            winner = 0;//Needs changed to tie

        initialized = true;
    }

    public void PlayerTwoWins()
    {
        if (!initialized)
        {
            initialized = true;
            //winUI[1].gameObject.SetActive(true);
            //Time.timeScale = 0;
            //StartCoroutine(ResetScene(2, 1));

            winner = 1;
            //Invoke("ResetScene", 2);
        }
    }
    public IEnumerator ResetScene(float waitTime)
    {
        //Time.timeScale = 0;
        //Time.timeScale = 0;
        //Time.fixedDeltaTime = Time.timeScale * 0.02f;

        //yield return new WaitForSecondsRealtime(5);
        /*
        score[0].text = uiHandler.score[0].ToString();
        score[1].text = uiHandler.score[1].ToString();
        score[3].text = uiHandler.score[0].ToString();
        score[4].text = uiHandler.score[1].ToString();
        */
        winUI[winner].gameObject.SetActive(true);
        if (winner == 0)
        {
            score[0].text = uiHandler.score[0].ToString();
            if (players[1] != null)
                score[1].text = uiHandler.score[1].ToString();
        }
        else if (winner == 1)
        {
            score[2].text = uiHandler.score[0].ToString();
            if(players[0] != null)
            score[3].text = uiHandler.score[1].ToString();
        }
        else if (winner == 2)//TIE!
        {
            score[2].text = uiHandler.score[0].ToString();
            if (players[0] != null)
                score[3].text = uiHandler.score[1].ToString();
        }
        yield return new WaitForSecondsRealtime(waitTime);
        Time.timeScale = 1;
        SceneManager.LoadScene("EndlessRunnerTEST_MainMultiplayer");
        //DO STUFF
    }
    /*
    void ResetScene()
    {
        SceneManager.LoadScene("EndlessRunnerTEST_MainMultiplayer");
    }
    */
}
