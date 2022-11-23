using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDownEffect : MonoBehaviour
{
    public AnimationCurve curve;
    [SerializeField] private CanvasGroup textUI;
    [SerializeField] private RectTransform uiTransform;
    [SerializeField] AudioSource audio;


    /*
    private void Start()
    {

        uiTransform = GetComponent<RectTransform>();
        textUI = GetComponent<CanvasGroup>();
    }
    */

    private void OnEnable()
    {
        StartCoroutine(CounterAnim());
    }

    IEnumerator CounterAnim()
    {
        audio.Play();
        float elapsedTime = 0;
        while (elapsedTime < 0.25f)
        {
            elapsedTime += Time.deltaTime;
            float ease = Mathf.Lerp(0, 1, elapsedTime * 1.75f);

            uiTransform.sizeDelta = new Vector2(curve.Evaluate(ease) * 120, curve.Evaluate(ease) * 120);

            yield return null;

        }

        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            textUI.alpha -= elapsedTime;

            yield return null;

        }

    }
}
