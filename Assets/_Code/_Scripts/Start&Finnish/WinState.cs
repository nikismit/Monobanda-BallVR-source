using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;

public class WinState : MonoBehaviour
{
    public GameObject[] winUI;
    public GameObject[] players;
    
    public AnimationCurve curve;
    
    private PlayersUIHandler uiHandler;
    private DemoUI demoUI;
    private bool initialized = false;
    private float timer = 0;
    private float timeLength = 3;
    private int winner;
    
    [SerializeField] private TextMeshProUGUI[] score;
    [SerializeField] private AudioSource finishAudio; 
    [SerializeField] private TutorialHandler tutHandler; 
    [SerializeField] private CanvasGroup highScoreUI;
    [SerializeField] private TextMeshProUGUI[] highScores;

    private void Start()
    {
        highScoreUI.alpha = 0;
        demoUI = GetComponent<DemoUI>();
        uiHandler = GetComponent<PlayersUIHandler>();
    }

    bool InvokeOnce = false;

    private void Update()
    {
        if (timer <= timeLength && initialized)
        {
            timer += Time.unscaledDeltaTime;
            float ease = Mathf.Clamp(timer / 3 , 0, 1);
            Time.timeScale = curve.Evaluate(ease);

            if (ease == 1 && !InvokeOnce)
            {
                InvokeOnce = true;
                StartCoroutine(StartTransition());
            }

        }
    }

    IEnumerator StartTransition()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        demoUI.RemoveDemoUIEvent();
    }

    public void PlayerOneWins()
    {
        if (!initialized)
        {
            initialized = true;
            winner = 0;
        }
    }

    public void ScoreWinner()
    {
        finishAudio.Play();
        initialized = true;
        
        if (uiHandler.score[0] > uiHandler.score[1])
            winner = 0;
        else if (uiHandler.score[0] < uiHandler.score[1])
            winner = 1;
        else if (uiHandler.score[0] == uiHandler.score[1])
            winner = 0;//Needs changed to tie

        
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
            if (!tutHandler.androidDebug)
            {
                score[0].text = uiHandler.score[0].ToString();
                if (players[1] != null)
                    score[1].text = uiHandler.score[1].ToString();
            }
            else
            {
                winUI[3].gameObject.SetActive(true);
            }

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
        for (int i = 0; i < highScores.Length; i++)
        {
            if (uiHandler.score[playerWon] > PlayerPrefs.GetFloat("HighScore" + i) && !stopChecking)
            {
                float scoreRef = 0;
                stopChecking = true;
                if (PlayerPrefs.GetFloat("HighScore" + i) != 0)
                    scoreRef = PlayerPrefs.GetFloat("HighScore" + i);

                PlayerPrefs.SetFloat("HighScore" + i, uiHandler.score[playerWon]);
                DownRankHighScores(i, playerWon, scoreRef);
            }
        }
        if(!stopChecking)
        {
            stopChecking = true;
            GenerateHighScores(420);
        }

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
