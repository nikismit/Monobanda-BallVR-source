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

    //Health
    [SerializeField] private AnimationCurve shakeCurve;
    [SerializeField] private Image[] hpFill;
    [SerializeField] private Color[] ColorRef = new Color[2];
    public Transform[] hpTrans;
    public RectTransform[] scoreRect;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < score.Length; i++)
        {
            ColorRef[i] = hpFill[i].color;
            score[i] = 0;
        }
    }

    IEnumerator Score(int playerNum, float value)
    {
        score[playerNum] += value;

        float elapsedTime = 0;
        float currentScore = Mathf.RoundToInt(score[playerNum]) - 100;

        while (elapsedTime < 1)
        {
            elapsedTime += Time.deltaTime;
            float ease = Mathf.Lerp(0, 1, elapsedTime);
            ringCountText[playerNum].transform.localPosition = Vector3.up * 1.5f;

            if (currentScore < score[playerNum])
            {
                currentScore += elapsedTime * 100;
            }
            else
                currentScore = Mathf.RoundToInt(score[playerNum]);


            if (playerNum == 0)
            {
                ringCountTrans[playerNum].localPosition = new Vector2(0, 0 + (scoreCurve.Evaluate(ease) * 20));
            }
            else
            {
                ringCountTrans[playerNum].localPosition = new Vector2(0, 0);
            }


            ringCountText[playerNum].text = Mathf.RoundToInt(currentScore).ToString();
            ringCountText[playerNum].fontSize = 32 + (scoreScaleCurve.Evaluate(ease) * 20);
            yield return null;
        }

        ringCountText[playerNum].text = score[playerNum].ToString();

        if (playerNum == 0)
            ringCountTrans[playerNum].localPosition = new Vector2(0, 0);
        else
            ringCountTrans[playerNum].localPosition = new Vector2(0, 0);
        
        ringCountText[playerNum].fontSize = 32;
    }

    public void UpdateScore(float value, int playerNum)
    {
        StartCoroutine(Score(playerNum, value));
    }

    public void UpdateHealth(int playerNum)
    {
        if (playerNum == 0)
        {
            StartCoroutine(Shake(hpTrans[playerNum], playerNum));
        }
        if (playerNum == 1)
        {
            StartCoroutine(Shake(hpTrans[playerNum], playerNum));
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
