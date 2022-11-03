using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBoundshandler : MonoBehaviour
{
    private Transform camPos;

    private void Start()
    {
        camPos = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    private void Update()
    {
        transform.position = camPos.position;
        //transform.Translate(transform.forward * 30 * Time.fixedDeltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        other.transform.position = new Vector3(transform.position.x + 40, 0.5f, 0);
        other.transform.rotation = new Quaternion(0,0,0,0);
    }
}
