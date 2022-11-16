using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowFloat : MonoBehaviour
{
    private bool floatUp;
    private bool lockInvoke;
    [SerializeField] float floatAmount;

    void Update()
    {
        if (floatUp)
            FloatingUp();
        else
            FloatingDown();
    }

    void FloatingUp()
    {
        transform.position += Vector3.up * floatAmount * Time.deltaTime;
        if (lockInvoke)
        {
            lockInvoke = false;
            Invoke("SetBool", 1);
        }

    }

    void FloatingDown()
    {
        transform.position -= Vector3.up * floatAmount * Time.deltaTime;
        if (!lockInvoke)
        {
            lockInvoke = true;
            Invoke("SetBool", 1);
        }
    }

    void SetBool()
    {
        if (floatUp)
            floatUp = false;
        else
            floatUp = true;
    }
}
