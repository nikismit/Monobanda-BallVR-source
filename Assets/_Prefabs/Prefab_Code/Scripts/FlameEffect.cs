using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameEffect : MonoBehaviour
{
    float easeOutlength = 2;

    private float timer;

    private Vector3 sizeRef;

    public AnimationCurve curve;

    void Start()
    {
        sizeRef = transform.localScale;
        timer = easeOutlength + 1;
    }

    void Update()
    {
        if (timer < 1)
        {
            timer += Time.deltaTime;
            Debug.Log("LOLOLOL");
            float ease = Mathf.Lerp(0, 1, timer);
            transform.localScale = sizeRef * (1 + 2.5f * (curve.Evaluate(ease)));
        }
        else
        {
            transform.localScale = sizeRef;
        }
    }

    public void InitiateBoostEffect()
    {
        timer = 0;
    }
}
