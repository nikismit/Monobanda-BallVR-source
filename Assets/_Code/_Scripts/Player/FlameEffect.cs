using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameEffect : MonoBehaviour
{
    private Vector3 sizeRef;

    public AnimationCurve curve;

    void Start()
    {
        sizeRef = transform.localScale;
        //timer = easeOutlength + 1;
    }

    public IEnumerator InitiateBoostEffect()
    {
        float elapsedTime = 0;

        while (elapsedTime < 1)
        {
            
            elapsedTime += Time.deltaTime;
           // highScoreUI.alpha = elapsedTime;

            float ease = Mathf.Lerp(0, 1, elapsedTime);
            transform.localScale = sizeRef * (1 + 2.5f * (curve.Evaluate(ease)));

            yield return null;
        }

        transform.localScale = sizeRef;
    }
}
