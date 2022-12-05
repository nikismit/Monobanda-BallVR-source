using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class JumpPad : MonoBehaviour
{
    public float jumpStrength;
    public AnimationCurve curve;
    [HideInInspector] public AudioSource source;

    private void Start()
    {
        source = gameObject.GetComponent<AudioSource>();
    }
}
