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

            float ease = Mathf.Lerp(0, easeOutlength / 2, timer * 1.2f);

            ringModels[0].transform.Rotate(new Vector3(0, 0, rotateSpeed * (1 + easeOutQuint(ease))));
            transform.localScale = sizeRef * (1 + (0.1f * easeOutElastic(ease)));
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

    float easeOutQuint(float x) 
    {
        return 1 - Mathf.Pow(1 - x, 5);
    }

    float easeOutElastic(float x)
    {
    float c4 = (2 * Mathf.PI) / 3;

    return 
            x == 0
  ? 0
  : x == 1
  ? 1
  : Mathf.Pow(2, -10 * x) * Mathf.Sin((x* 10 - 0.75f) * c4) + 1;
}

    float easeOutBack(float x)
    {
        //float c1 = 15.70158f;
        float c1 = 1.70158f;
        float c3 = c1 + 1;

        return 1 + c3* Mathf.Pow(x - 1, 3) + c1* Mathf.Pow(x - 1, 2);
    }

    float easeInCirc(float x)
    {
        return 1 - Mathf.Sqrt(1 - Mathf.Pow(x, 2));
    }
}
