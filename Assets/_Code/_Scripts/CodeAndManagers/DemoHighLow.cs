using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class DemoHighLow : MonoBehaviour
{
    [SerializeField] GameObject[] highLowObj;
    [SerializeField] public TutorialHandler tutHandler;

    [HideInInspector] public int lowCount = 0;
    [HideInInspector] public int highCount = 0;

    public RectTransform[] highLowRect;
    float timer = 0;


    [SerializeField] private bool floatUp;
    private bool lockInvoke;
    [SerializeField] float floatAmount;

    [SerializeField] AnimationCurve curve;

    Vector2 scaleRef = new Vector2(420, 220);

    AudioSource blipSound;

    [SerializeField] ParticleSystem[] holdParticle;
    [SerializeField] Image[] holdImage;
    [SerializeField] private Sprite[] holdWhite;
    private Sprite[] holdRef = new Sprite[2];


    void Start()
    {
        holdRef[0] = holdImage[0].sprite;
        holdRef[1] = holdImage[1].sprite;

        blipSound = GetComponent<AudioSource>();

        highLowObj[0].SetActive(true);
        if (floatUp)
            lockInvoke = true;
    }

    void Update()
    {
        if (floatUp)
            FloatingUp();
        else
            FloatingDown();
    }

    void FloatingUp()
    {
        if (lockInvoke)
        {
            lockInvoke = false;
            StartCoroutine(SquashStretch(new Vector2(420 * 1.2f, 220 * 1.2f), true));
            Invoke("SetBool", 1);
        }
    }

    void FloatingDown()
    {
        if (!lockInvoke)
        {
            lockInvoke = true;
            StartCoroutine(SquashStretch(new Vector2(420 * 1.2f, 220 * 1.2f), false));
            Invoke("SetBool", 1);
        }
    }

    void SetBool()
    {
        if (floatUp)
            floatUp = false;
        else
            floatUp = true;
    }

    public void HighEvent()
    {
        blipSound.Play();

        if (tutHandler.androidDebug)
        {
            highLowObj[0].SetActive(false);
            highLowObj[2].SetActive(false);
            highLowObj[1].SetActive(true);
            return;
        }

        highCount++;

        if (highCount > 1)
        {
            highLowObj[0].SetActive(false);
            highLowObj[2].SetActive(false);
            highLowObj[1].SetActive(true);
        }
    }

    bool revertHold = false;

    public void HighHoldEvent(bool changeUI)
    {
        if (changeUI)
        {
            if (!revertHold)
            {
                Debug.Log("LNJWVBEWBVEBVEBVEBJKVEWBKJVVBEKJW");
                revertHold = true;
                holdImage[0].sprite = holdWhite[0];
                Invoke("RevertHigh", 0.1f);
            }

            //holdParticle[0].Play();
            highLowObj[0].SetActive(false);
            highLowObj[2].SetActive(true);
        }
        else if (!changeUI)
        {
            revertHold = false;
            //holdParticle[0].Stop();
            highLowObj[0].SetActive(true);
            highLowObj[2].SetActive(false);
        }
    }

    void RevertHigh()
    {
        holdImage[0].sprite = holdRef[0];
    }

    public void LowHoldEvent(bool changeUI)
    {
        if (changeUI)
        {
            if (!revertHold)
            {
                revertHold = true;
                holdImage[1].sprite = holdWhite[1];
                Invoke("RevertLow", 0.1f);
            }
            //holdParticle[1].Play();
            highLowObj[1].SetActive(false);
            highLowObj[3].SetActive(true);
        }
        else if (!changeUI)
        {
            revertHold = false;
            //holdParticle[1].Stop();
            highLowObj[1].SetActive(true);
            highLowObj[3].SetActive(false);
        }
    }

    void RevertLow()
    {
        holdImage[1].sprite = holdRef[1];
    }

    public void LowEvent()
    {
        blipSound.Play();

        if (tutHandler.androidDebug)
        {
            highLowObj[1].SetActive(false);
            highLowObj[3].SetActive(false);
            tutHandler.SpawnPlane();
            return;
        }

        lowCount++;

        if (lowCount > 1)
        {
            highLowObj[1].SetActive(false);
            highLowObj[3].SetActive(false);
            tutHandler.SpawnPlane();
        }
    }

    IEnumerator SquashStretch(Vector2 scaleMultiplier, bool scale)
    {
        float elapsedTime = 0;

        while (elapsedTime < 1)
        {
            elapsedTime += Time.deltaTime;

            //float ease = Mathf.Clamp(elapsedTime, 0, 1);
            float ease = Mathf.Clamp(elapsedTime, 0, 1);

            if (scale)
            {
                Vector3 easeVector = Vector3.Lerp(scaleRef, scaleMultiplier, curve.Evaluate(ease));

                //playerModel.localScale = new Vector3(curve.Evaluate(ease), curve.Evaluate(ease), curve.Evaluate(ease));
                highLowRect[0].sizeDelta = easeVector;
                highLowRect[1].sizeDelta = easeVector;
                highLowRect[2].sizeDelta = easeVector;
                highLowRect[3].sizeDelta = easeVector;
            }
            else
            {
                Vector3 easeVector = Vector3.Lerp(scaleMultiplier, scaleRef, curve.Evaluate(ease));

                //playerModel.localScale = new Vector3(curve.Evaluate(ease), curve.Evaluate(ease), curve.Evaluate(ease));
                highLowRect[0].sizeDelta = easeVector;
                highLowRect[1].sizeDelta = easeVector;
                highLowRect[2].sizeDelta = easeVector;
                highLowRect[3].sizeDelta = easeVector;
            }


            //Time.timeScale = curve.Evaluate(ease);

            //highScoreUI.alpha = elapsedTime;

            yield return null;

        }
        //highLowRect[0].sizeDelta = scaleMultiplier;
    }

}
