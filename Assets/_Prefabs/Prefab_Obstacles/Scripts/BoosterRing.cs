using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BoosterRing : MonoBehaviour
{
    [SerializeField] GameObject[] ringModels;
    [SerializeField] GameObject cam;

    [SerializeField] float rotateSpeed = 1;
    float easeOutlength = 2;

    private float timer;
    private AudioSource boostSound;

    private Vector3 sizeRef;

    void Start()
    {
        sizeRef = transform.localScale;
        //timer = easeOutlenght;
        timer = easeOutlength + 1;
        boostSound = gameObject.GetComponent<AudioSource>();
    }

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
        if (other.gameObject.tag == "Player")
        {
            timer = 0;
            boostSound.Play();
        }
    }
}
