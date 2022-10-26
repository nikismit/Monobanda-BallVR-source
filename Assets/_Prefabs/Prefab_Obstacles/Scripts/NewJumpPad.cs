using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewJumpPad : MonoBehaviour
{
    [SerializeField] AnimationCurve jumpCurveUP;
    [SerializeField] AnimationCurve jumpCurveDown;

    private float time = 0;
    private float duration = 1;
    private float height = 3f;

    void Jump(Transform target, float speed)
    {
        target.Translate(target.transform.up * jumpCurveUP.Evaluate(curveProgress() * speed * Time.deltaTime));
    }

    public IEnumerator jumpCoroutine(Transform target, float speed)
    {
        
        while (time < duration)
        {
            time += Time.deltaTime;
            float linearT = time / duration;
            float heightT = jumpCurveUP.Evaluate(linearT);

            float newHeight = Mathf.Lerp(0f, height, heightT);


            target.Translate(target.transform.up * newHeight);
            //target.position.y += jumpCurveUP.Evaluate(curveProgress());

            

        }
        return null;

    }

    float curveProgress()
    {
        time += Time.deltaTime;
        return time / duration;
    }
}
