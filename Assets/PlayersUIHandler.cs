using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayersUIHandler : MonoBehaviour
{
    public Slider[] playerHealth;
    public TextMeshProUGUI[] ringCountText;
    public Transform[] ringCountTrans;
    private float[] score = new float [2];

    [SerializeField] private AnimationCurve scoreCurve;
    private float timerOne, timerTwo;
    private float length = 1;

    private bool updateScoreOne;
    private bool updateScoreTwo;

    Vector2 vectorRefOne, vectorRefTwo;

    // Start is called before the first frame update
    void Start()
    {
        //transRef = transform;
        vectorRefOne = ringCountTrans[0].localPosition;
        vectorRefTwo = ringCountTrans[1].localPosition;
        timerOne = length + 1;
        timerTwo = length + 1;

        for (int i = 0; i < score.Length; i++)
        {
            //playerHealth[i].maxValue = 5;
            //playerHealth[i].value = 5;
            score[i] = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (updateScoreOne)
        {
            scoreTimerOne(0);
        }
        if (updateScoreTwo)
        {
            scoreTimerTwo(1);
        }
    }
    void scoreTimerOne(int playerNum)
    {
        if (timerOne <= length)
        {
            timerOne += Time.deltaTime;

            //float ease = Mathf.Lerp(0, easeOutlength / 2, timer * 1.2f);
            float ease = Mathf.Lerp(0, 1, timerOne);

            ringCountText[playerNum].text = score[playerNum].ToString();
            ringCountTrans[playerNum].localPosition = new Vector2(245, 55 + (scoreCurve.Evaluate(ease) * 20));
        }
        else
        {
            if (updateScoreOne)
            {
                ringCountTrans[playerNum].localPosition = new Vector2(245, 55);
                updateScoreOne = false;
            }
        }
    }


    void scoreTimerTwo(int playerNum)
    {
        if (timerTwo <= length)
        {
            timerTwo += Time.deltaTime;

            //float ease = Mathf.Lerp(0, easeOutlength / 2, timer * 1.2f);
            float ease = Mathf.Lerp(0, 1, timerTwo);

            ringCountText[playerNum].text = score[playerNum].ToString();
            ringCountTrans[playerNum].localPosition = new Vector2(-155f , 55 + (scoreCurve.Evaluate(ease) * 20));
        }
        else
        {
            if (updateScoreTwo)
            {
                ringCountTrans[playerNum].localPosition = new Vector2(-155f, 55);
                updateScoreTwo = false;
            }
        }
    }

    public void UpdateScore(float value, int playerNum)
    {
        if (playerNum == 0)
        {
            updateScoreOne = true;
            score[0] += value;
            ringCountText[playerNum].transform.localPosition = Vector3.up * 1.5f;
            timerOne = 0;
        }
        if (playerNum == 1)
        {
            updateScoreTwo = true;
            score[1] += value;
            ringCountText[playerNum].transform.localPosition = Vector3.up * 1.5f;
            timerTwo = 0;
        }


    }
}
