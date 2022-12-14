using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    [SerializeField] Transform[] movePoints;
    [SerializeField] float speed = 5;

    private int num;

    void Update()
    {
        if (Vector3.Distance(transform.position, movePoints[num].position) < 1)
        {
            num++;
            if (num >= movePoints.Length)
                num = 0;
        }
        transform.position = Vector3.MoveTowards(transform.position, movePoints[num].position, Time.deltaTime * speed);
    }
}
