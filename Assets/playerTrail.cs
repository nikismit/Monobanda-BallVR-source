using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerTrail : MonoBehaviour
{
    TrailRenderer trail;
    ParticleSystem particle;

    [SerializeField] AnimationCurve[] trailCurves = new AnimationCurve[4];

    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        
    }

    void UpdateTrail()
    {

    }
}
