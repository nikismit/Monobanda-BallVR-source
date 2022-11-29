using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BoomSound : MonoBehaviour
{
    private void OnEnable()
    {
        AudioSource sound = GetComponent<AudioSource>();
        sound.Play();
    }
}
