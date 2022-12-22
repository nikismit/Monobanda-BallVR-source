using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileManager : MonoBehaviour
{
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Invoke("TargetFrameRate",0.1f);
    }


    void TargetFrameRate()
    {
        Application.targetFrameRate = 60;
    }
}
