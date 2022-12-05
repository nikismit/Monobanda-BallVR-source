using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerTrail : MonoBehaviour
{
    TrailRenderer trail;
    ParticleSystem particle;

    [SerializeField] AnimationCurve[] trailCurves = new AnimationCurve[2];

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTrail();
    }

    bool isActive;

    void UpdateTrail()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (isActive)
            {
                isActive = false;
                Debug.Log("SETCURVE " + isActive);
                trail.widthCurve = trailCurves[0];
            }
            else
            {
                isActive = true;
                Debug.Log("SETCURVE " + isActive);
                trail.widthCurve = trailCurves[1];
            }
        }
    }
}
