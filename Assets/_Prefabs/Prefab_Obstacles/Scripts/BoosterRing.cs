﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BoosterRing : MonoBehaviour
{
    [SerializeField] GameObject[] ringModels;


    [SerializeField] float rotateSpeed = 1;

    float easeOutlength = 2;

    private float timer;
    private AudioSource boostSound;

    private Vector3 sizeRef;

    // Start is called before the first frame update
    void Start()
    {
        sizeRef = transform.localScale;
        //timer = easeOutlenght;
        timer = easeOutlength + 1;
        boostSound = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < easeOutlength)
        {
            timer += Time.deltaTime;

            float ease = Mathf.InverseLerp(0, easeOutlength, timer);

            ringModels[0].transform.Rotate(new Vector3(0, 0, 200));
            
            transform.localScale = sizeRef * 1.2f;

        }
        else
        {
            ringModels[0].transform.Rotate(new Vector3(0, 0, 1));
            transform.localScale = sizeRef;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        timer = 0;
        boostSound.Play();
    }

    float easeOutQuart(float x){
        return 1 - Mathf.Pow(1 - x, 4);
    }
}
