using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class JumpPad : MonoBehaviour
{
    public float jumpStrength;
    public float jumpLength;
    public AudioSource source;

    public AnimationCurve curve;

    private void Start()
    {
        source = gameObject.GetComponent<AudioSource>();
    }
}
