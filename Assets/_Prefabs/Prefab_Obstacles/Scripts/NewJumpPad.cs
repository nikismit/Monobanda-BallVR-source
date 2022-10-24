using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewJumpPad : MonoBehaviour
{
    LineRenderer lineRender;

    int numPoints = 50;

    float timeBetweenPoints = 0.1f;

    public float power = 5;

    void Start()
    {
        lineRender = GetComponent<LineRenderer>();
    }

    void Update()
    {
        lineRender.positionCount = (int)numPoints;
        List<Vector3> points = new List<Vector3>();
        Vector3 startingPosition = transform.position;
        Vector3 startingVelocity = transform.up * power;

        for (float t = 0; t < numPoints; t += timeBetweenPoints)
        {
            Vector3 newPoint = startingPosition + t * startingVelocity;
            newPoint.y = startingPosition.y + startingPosition.y * t + Physics.gravity.y / 2f * t * t;
            points.Add(newPoint);

            if (Physics.OverlapSphere(newPoint, 2, 8).Length > 0)
            {
                lineRender.positionCount = points.Count;
                break;
            }
        }

        lineRender.SetPositions(points.ToArray());
    }
}
