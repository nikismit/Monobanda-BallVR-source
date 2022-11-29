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
    [HideInInspector] public float[] score = new float [2];

    [SerializeField] private AnimationCurve scoreCurve;
    [SerializeField] private AnimationCurve scoreScaleCurve;
    private float timerOne, timerTwo;
    private float timerOneHp, timerTwoHp;
    private float length = 1;

    private bool updateScoreOne, updateScoreTwo;



    //Health
    [SerializeField] Image[] hpFill;
    [SerializeField] Color[] ColorRef = new Color[2];
    private bool updateHpOne, updateHpTwo;
    public Transform[] hpTrans;
    public RectTransform[] scoreRect;
    [SerializeField] AnimationCurve shakeCurve;


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
        /*
        if (updateHpOne)
        {
            HpTimerOne(0);
        }
        if (updateHpTwo)
        {
            HpTimerOne(1);
        }
        */
    }

    int addCount;

    void scoreTimerOne(int playerNum)
    {
        if (timerOne <= length)
        {
            timerOne += Time.deltaTime;

            //float ease = Mathf.Lerp(0, easeOutlength / 2, timer * 1.2f);
            float ease = Mathf.Lerp(0, 1, timerOne);

            ringCountText[playerNum].text = score[playerNum].ToString();
            ringCountTrans[playerNum].localPosition = new Vector2(245, 55 + (scoreCurve.Evaluate(ease) * 20));
            ringCountText[playerNum].fontSize = 32 + (scoreScaleCurve.Evaluate(ease) * 20);

            //scoreRect[playerNum].sizeDelta = new Vector2(1 + scoreScaleCurve.Evaluate(ease) * 20, 1 + scoreScaleCurve.Evaluate(ease) * 20);
        }
        else
        {
            if (updateScoreOne)
            {
                ringCountTrans[playerNum].localPosition = new Vector2(245, 55);
                //scoreRect[playerNum].sizeDelta = new Vector2(1, 1);
                ringCountText[playerNum].fontSize = 32;
                updateScoreOne = false;
            }
        }
    }

    IEnumerator Score(int playerNum, float value)
    {
        //Vector3 startPos = target.localPosition;
        score[playerNum] += value;
        //ringCountText[playerNum].transform.localPosition = Vector3.up * 1.5f;

        float elapsedTime = 0;

        int currentScore = Mathf.RoundToInt(score[playerNum]) - 100;

        while (elapsedTime < 1)
        {
            elapsedTime += Time.deltaTime;
            float ease = Mathf.Lerp(0, 1, elapsedTime);
            //float strength = shakeCurve.Evaluate(elapsedTime / 1) * 10;
            //target.localPosition = startPos + Random.insideUnitSphere * strength;
            ringCountText[playerNum].transform.localPosition = Vector3.up * 1.5f;

            if (currentScore < score[playerNum])
            {
                float countEase = Mathf.Lerp(0, 1, Time.deltaTime * 1.2f);

                if(playerNum == 0)
                ringCountTrans[playerNum].localPosition = new Vector2(245, 55 + (scoreCurve.Evaluate(ease) * 20));
                else
                    ringCountTrans[playerNum].localPosition = new Vector2(-155f, 55 + (scoreCurve.Evaluate(ease) * 20));

                currentScore += Mathf.RoundToInt(countEase * 100);
            }
            else
                currentScore = Mathf.RoundToInt(score[playerNum]);

            ringCountText[playerNum].text = currentScore.ToString();

            //ringCountTrans[playerNum].localPosition = new Vector2(245, 55 + (scoreCurve.Evaluate(ease) * 20));
            ringCountText[playerNum].fontSize = 32 + (scoreScaleCurve.Evaluate(ease) * 20);
            yield return null;
        }
        //ringCountText[playerNum].text = currentScore.ToString();
        ringCountText[playerNum].text = score[playerNum].ToString();

        if (playerNum == 0)
        ringCountTrans[playerNum].localPosition = new Vector2(245, 55);
        else
            ringCountTrans[playerNum].localPosition = new Vector2(-155f, 55);
        //scoreRect[playerNum].sizeDelta = new Vector2(1, 1);
        ringCountText[playerNum].fontSize = 32;
        //updateScoreOne = false;
        //target.localPosition = startPos;
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
            scoreRect[playerNum].sizeDelta = new Vector2(1 + scoreScaleCurve.Evaluate(ease) * 20, 1 + scoreScaleCurve.Evaluate(ease) * 20);
        }
        else
        {
            if (updateScoreTwo)
            {
                ringCountTrans[playerNum].localPosition = new Vector2(-155f, 55);
                scoreRect[playerNum].sizeDelta = new Vector2(1, 1);
                updateScoreTwo = false;
            }
        }
    }

    void HpTimerOne(int playerNum)
    {
        if (timerOneHp <= 0.1f)
        {
            timerOneHp += Time.deltaTime;
            hpFill[playerNum].color = Color.white;
        }
        else
        {
            if (updateHpOne)
            {
                hpFill[playerNum].color = ColorRef[playerNum];
                updateHpOne = false;
            }
        }
    }

    public void UpdateScore(float value, int playerNum)
    {
        if (playerNum == 0)
        {
            //updateScoreOne = true;
            //score[playerNum] += value;
            //ringCountText[playerNum].transform.localPosition = Vector3.up * 1.5f;
            timerOne = 0;
            StartCoroutine(Score(playerNum, value));
        }
        if (playerNum == 1)
        {
            //updateScoreTwo = true;
            //score[playerNum] += value;
            //ringCountText[playerNum].transform.localPosition = Vector3.up * 1.5f;
            timerTwo = 0;
            StartCoroutine(Score(playerNum, value));
        }
    }

    public void UpdateHealth(int playerNum)
    {
        if (playerNum == 0)
        {
            updateHpOne = true;
            StartCoroutine(Shake(hpTrans[playerNum], playerNum));
            //ringCountText[playerNum].transform.localPosition = Vector3.up * 1.5f;
            timerOneHp = 0;
        }
        if (playerNum == 1)
        {
            updateHpTwo = true;
            StartCoroutine(Shake(hpTrans[playerNum], playerNum));
            //ringCountText[playerNum].transform.localPosition = Vector3.up * 1.5f;
            timerTwoHp = 0;
        }
    }

    IEnumerator Shake(Transform target, int playerNum)
    {
        hpFill[playerNum].color = Color.white;

        Vector3 startPos = target.localPosition;
        float elapsedTime = 0;

        while (elapsedTime < 1)
        {
            if (elapsedTime >= 0.1)
                hpFill[playerNum].color = ColorRef[playerNum];

            elapsedTime += Time.deltaTime;
            float strength = shakeCurve.Evaluate(elapsedTime / 1) * 10;
            target.localPosition = startPos + Random.insideUnitSphere * strength;
            yield return null;
        }

        target.localPosition = startPos;
    }
}
