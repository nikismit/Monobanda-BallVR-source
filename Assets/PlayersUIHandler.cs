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
    private float timerOneHp, timerTwoHp;
    private float length = 1;

    private bool updateScoreOne, updateScoreTwo;



    //Health
    [SerializeField] Image[] hpFill;
    [SerializeField] Color[] ColorRef = new Color[2];
    private bool updateHpOne, updateHpTwo;
    public Transform[] hpTrans;


    // Start is called before the first frame update
    void Start()
    {
        //transRef = transform;
        timerOne = length + 1;
        timerTwo = length + 1;

        for (int i = 0; i < score.Length; i++)
        {
            ColorRef[i] = hpFill[i].color;
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
        if (updateHpOne)
        {
            HpTimerOne(0);
        }
        if (updateHpTwo)
        {
            HpTimerOne(1);
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

    void HpTimerOne(int playerNum)
    {
        if (timerOneHp <= 0.05f)
        {
            timerOneHp += Time.deltaTime;
            hpFill[playerNum].color = Color.white;
            //float ease = Mathf.Lerp(0, easeOutlength / 2, timer * 1.2f);
            float ease = Mathf.Lerp(0, 1, timerOneHp);

            //ringCountText[playerNum].text = score[playerNum].ToString();
            //ringCountTrans[playerNum].localPosition = new Vector2(245, 55 + (scoreCurve.Evaluate(ease) * 20));
        }
        else
        {
            if (updateHpOne)
            {
                hpFill[playerNum].color = ColorRef[playerNum];
                //ringCountTrans[playerNum].localPosition = new Vector2(245, 55);
                updateHpOne = false;
            }
        }
    }

    public void UpdateScore(float value, int playerNum)
    {
        if (playerNum == 0)
        {
            updateScoreOne = true;
            score[playerNum] += value;
            ringCountText[playerNum].transform.localPosition = Vector3.up * 1.5f;
            timerOne = 0;
        }
        if (playerNum == 1)
        {
            updateScoreTwo = true;
            score[playerNum] += value;
            ringCountText[playerNum].transform.localPosition = Vector3.up * 1.5f;
            timerTwo = 0;
        }
    }

    public void UpdateHealth(int playerNum)
    {
        if (playerNum == 0)
        {
            updateHpOne = true;
            //ringCountText[playerNum].transform.localPosition = Vector3.up * 1.5f;
            timerOneHp = 0;
        }
        if (playerNum == 1)
        {
            updateHpTwo = true;
            //ringCountText[playerNum].transform.localPosition = Vector3.up * 1.5f;
            timerTwoHp = 0;
        }
    }
}
