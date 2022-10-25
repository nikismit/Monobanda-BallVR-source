using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewJumpPad : MonoBehaviour
{
    public float initialVel;
    public float angle;
    public float height;
    public Transform targetPos;

    [SerializeField] LineRenderer line;
    [SerializeField] float step;

    private void Start()
    {
        //targetPos.position =- transform.position;
        height = targetPos.position.y + targetPos.position.magnitude / 2f;
        height = Mathf.Max(0.01f, height);
    }

    private void Update()
    {

        float v0;
        float time;
        float angle;
        //jumpRef.CalculatePath(jumpRef.targetPos.position, newAngle, out v0, out time);
        CalculatePathWithHeight(targetPos.position, height, out v0, out angle, out time);

        DrawPath(v0, angle, time, step);
        /*
                 float newAngle = angle * Mathf.Deg2Rad;
        StopAllCoroutines();
        StartCoroutine(CourotineMovement(initialVel, newAngle));
         */
    }

    void DrawPath(float v0, float angle, float time, float step)
    {
        step = Mathf.Max(0.01f, step);
        line.positionCount = (int)(time / step) + 2;
        int count = 0;

        for (float i = 0; i < time; i += step)
        {
            float x = v0 * i * Mathf.Cos(angle);
            float y = v0 * i * Mathf.Sin(angle) - 0.5f * -Physics.gravity.y * Mathf.Pow(i, 2);

            line.SetPosition(count,transform.position + new Vector3(x, y, 0));
            count++;
        }

        float xfinal = v0 * time * Mathf.Cos(angle);
        float yfinal = v0 * time * Mathf.Sin(angle) - 0.5f * -Physics.gravity.y * Mathf.Pow(time, 2);
        line.SetPosition(count, transform.position + new Vector3(xfinal, yfinal, 0));
    }

    float QuadratricEquation(float a, float b, float c,float sign)
    {
        return (-b + sign * Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
    }

    public void CalculatePathWithHeight(Vector3 targetPos, float h, out float v0, out float angle, out float time)
    {
        float xt = targetPos.x;
        float yt = targetPos.y;
        float g = -Physics.gravity.y;

        float b = Mathf.Sqrt(2 * g * h);
        float a = (-0.5f * g);
        float c = -yt;

        float tplus = QuadratricEquation(a, b, c, 1);
        float tmin = QuadratricEquation(a, b, c, -1);
        time = tplus > tmin ? tplus : tmin;
        
        angle = Mathf.Atan(b * time / xt);

        v0 = b / Mathf.Sin(angle);
    }

    public void CalculatePath(Vector3 targetPos, float angle, out float v0, out float time)
    {
        float xt = targetPos.x;
        float yt = targetPos.y;
        float g = -Physics.gravity.y;

        float v1 = Mathf.Pow(xt, 2) * g;
        float v2 = 2 * xt * Mathf.Sin(angle) * Mathf.Cos(angle);
        float v3 = 2 * yt * Mathf.Pow(Mathf.Cos(angle), 2);
        v0 = Mathf.Sqrt(v1 / (v2 - v3));

        time = xt / (v0 * Mathf.Cos(angle));
    }

    /*
    IEnumerator CourotineJump(float v0, float angle)
    {
        float t = 0;
        while (t < 100)
        {
            float x = v0 * t * Mathf.Cos(angle);
            float y = v0 * t * Mathf.Sin(angle) - (1f / 2f) * -Physics.gravity.y * Mathf.Pow(t, 2);
            transform.position = transform.position + new Vector3(x, y, 0);

            t += Time.deltaTime;
            yield return null;
        }
    }
    */
}
