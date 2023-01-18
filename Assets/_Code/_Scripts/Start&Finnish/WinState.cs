using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
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
    [SerializeField] AudioSource finishAudio; 
    [SerializeField] TutorialHandler tutHandler; 

    private int winner;

    [SerializeField] CanvasGroup highScoreUI;
    [SerializeField] TextMeshProUGUI[] highScores;
    //private string[] highscorePref = new string(HighScore1);

    private void Start()
    {
        highScoreUI.alpha = 0;
        //highScores = gameObject.GetComponentsInChildren<TextMeshProUGUI>();
        demoUI = GetComponent<DemoUI>();
        uiHandler = GetComponent<PlayersUIHandler>();
    }

    bool InvokeOnce = false;

    private void Update()
    {



        if (timer <= timeLength && initialized)
        {
            //InvokeOnce = false;
            timer += Time.unscaledDeltaTime;
            //float ease = Mathf.Lerp(0, easeOutlength / 2, timer * 1.2f);
            //float ease = Mathf.Lerp(0, 3, timer);
            float ease = Mathf.Clamp(timer / 3 , 0, 1);
            Time.timeScale = curve.Evaluate(ease);

            if (ease == 1 && !InvokeOnce)
            {
                //Debug.Log("INVOKEONCE");
                InvokeOnce = true;
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
        finishAudio.Play();
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
            winner = 1;
        }
    }
    public IEnumerator ResetScene(float waitTime)
    {
        if(tutHandler.androidDebug && winner == 1)
            winUI[2].gameObject.SetActive(true);
        else
            winUI[winner].gameObject.SetActive(true);

        if (winner == 0)
        {
            score[0].text = uiHandler.score[0].ToString();
            if (players[1] != null)
                score[1].text = uiHandler.score[1].ToString();
        }
        else if (winner == 1)
        {
            if (!tutHandler.androidDebug)
            {
                score[2].text = uiHandler.score[0].ToString();
                if (players[0] != null)
                    score[3].text = uiHandler.score[1].ToString();
            }
            else
                score[4].text = uiHandler.score[0].ToString();


        }
        else if (winner == 2)//TIE!
        {
            score[2].text = uiHandler.score[0].ToString();
            if (players[0] != null)
                score[3].text = uiHandler.score[1].ToString();
        }
        yield return new WaitForSecondsRealtime(waitTime);


        StartCoroutine(ShowHighScores());
    }

    bool invokeHighScores = false;

    IEnumerator ShowHighScores()
    {
        //Debug.Log("SHOWKKR HIghSCore");
        if (!invokeHighScores)
        {
            invokeHighScores = true;
            ShowHighScore(winner);
        }

        float elapsedTime = 0;

        while (elapsedTime < 1)
        {
            elapsedTime += Time.unscaledDeltaTime;
            highScoreUI.alpha = elapsedTime;

            yield return null;
        }
    }


    void ShowHighScore(int playerWon)
    {
        bool stopChecking = false;

        //Debug.Log("LMAO");
        
        for (int i = 0; i < highScores.Length; i++)
        {
            if (uiHandler.score[playerWon] > PlayerPrefs.GetFloat("HighScore" + i) && !stopChecking)
            {
                float scoreRef = 0;

                //Debug.Log("PlayerWon = " + i);
                stopChecking = true;
                if (PlayerPrefs.GetFloat("HighScore" + i) != 0)
                    scoreRef = PlayerPrefs.GetFloat("HighScore" + i);

                PlayerPrefs.SetFloat("HighScore" + i, uiHandler.score[playerWon]);
                //highScores[i].text = PlayerPrefs.GetFloat("HighScore" + i).ToString();

                DownRankHighScores(i, playerWon, scoreRef);
            }
        }
        if(!stopChecking)
        {
            stopChecking = true;
            GenerateHighScores(420);
        }

        //Debug.Log("1 = " + PlayerPrefs.GetFloat("HighScore1") + ", 2 = " + PlayerPrefs.GetFloat("HighScore2") + ", 3 = " + PlayerPrefs.GetFloat("HighScore3")
        //+ ", 4 = " + PlayerPrefs.GetFloat("HighScore4") + ", 5 = " + PlayerPrefs.GetFloat("HighScore5") + ", 6 = " + PlayerPrefs.GetFloat("HighScore6"));

        //Invoke("RestartScene", 15);
        //Time.timeScale = 1;
        StartCoroutine(RestartScene());
    }

    IEnumerator RestartScene()
    {
        yield return new WaitForSecondsRealtime(15);
        Time.timeScale = 1;
        SceneManager.LoadScene("EndlessRunnerTEST_MainMultiplayer");
    }

    void GenerateHighScores(int newHighScore)
    {
        for (int s = 0; s < highScores.Length; s++)
        {
            if(newHighScore == s)
            {
                highScores[s].color = new Color(255, 215, 0);
                highScores[s].text = "NEW HIGHSCORE! " + PlayerPrefs.GetFloat("HighScore" + s).ToString();
            }
            else
            {
                highScores[s].color = Color.white;
                if (s == 0)
                    highScores[s].text = "1st " + PlayerPrefs.GetFloat("HighScore" + s).ToString();
                if (s == 1)
                    highScores[s].text = "2nd " + PlayerPrefs.GetFloat("HighScore" + s).ToString();
                if (s == 2)
                    highScores[s].text = "3rd " + PlayerPrefs.GetFloat("HighScore" + s).ToString();
                if (s == 3)
                    highScores[s].text = "4th " + PlayerPrefs.GetFloat("HighScore" + s).ToString();
                if (s == 4)
                    highScores[s].text = "5th " + PlayerPrefs.GetFloat("HighScore" + s).ToString();
                if (s == 5)
                    highScores[s].text = "6th " + PlayerPrefs.GetFloat("HighScore" + s).ToString();
                if (s == 6)
                    highScores[s].text = "7th " + PlayerPrefs.GetFloat("HighScore" + s).ToString();
            }

        }
    }

    void DownRankHighScores(int i, int player, float prevScoreRef)
    {
        bool downRankCurrent = false;
        
        for (int s = 1 + i; s < highScores.Length; s++)
        {
            if (prevScoreRef != 0 && !downRankCurrent)
            {
                downRankCurrent = true;
                PlayerPrefs.SetFloat("HighScore" + s, prevScoreRef);
            }
            else
            {
                float scoreRef = PlayerPrefs.GetFloat("HighScore" + s);
                PlayerPrefs.SetFloat("HighScore" + s, scoreRef);
            }
        }
        GenerateHighScores(i);
    }
}
