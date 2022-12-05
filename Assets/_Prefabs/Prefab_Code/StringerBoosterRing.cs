using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringerBoosterRing : MonoBehaviour
{
    [SerializeField] GameObject[] ringModels;
    [SerializeField] GameObject cam;
    private Projector shadow;

    float rotateSpeed = 10f;
    float easeOutlength = 2;

    private AudioSource boostSound;

    private Vector3 sizeRef;

    public float BoostAmount = 10;
    public AnimationCurve curve;

    void Start()
    {
        shadow = GetComponentInChildren<Projector>();
        sizeRef = transform.localScale;
        //timer = easeOutlenght;
        boostSound = gameObject.GetComponent<AudioSource>();
    }

    
    void Update()
    {
        ringModels[0].transform.Rotate(new Vector3(0, 0, 2));
    }
    

    IEnumerator StartBoost()
    {
        float elapsedTime = 0;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime;

            float ease = Mathf.Lerp(0, 1, elapsedTime);

            ringModels[0].transform.Rotate(new Vector3(0, 0, rotateSpeed * (1 + easeOutQuint(ease))));
            transform.localScale = sizeRef * (1 + (0.1f * curve.Evaluate(ease)));
            shadow.orthographicSize = 2 * (1 + (0.1f * curve.Evaluate(ease)));

            yield return null;
        }

        ringModels[0].transform.Rotate(new Vector3(0, 0, 2));
        transform.localScale = sizeRef;
        shadow.orthographicSize = 2;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //timer = 0;
            StartCoroutine(StartBoost());
            boostSound.Play();
        }
    }

    float easeOutQuint(float x)
    {
        return 1 - Mathf.Pow(1 - x, 5);
    }
}
