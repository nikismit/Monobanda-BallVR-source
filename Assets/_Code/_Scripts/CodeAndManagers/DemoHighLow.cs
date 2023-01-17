using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class DemoHighLow : MonoBehaviour
{
    [SerializeField] GameObject[] highLowObj;
    [SerializeField] public TutorialHandler tutHandler;
    [SerializeField] private AudioMovement[] playerMic;

    [HideInInspector] public int lowCount = 0;
    [HideInInspector] public int highCount = 0;

    public RectTransform[] highLowRect;

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

    private bool highDone = false;
    void Start()
    {
        holdRef[0] = holdImage[0].sprite;
        holdRef[1] = holdImage[1].sprite;

        blipSound = GetComponent<AudioSource>();

        highLowObj[0].SetActive(true);
        if (floatUp)
            lockInvoke = true;
    }

    bool changeUI = true;
    bool lockUI = true;

    void Update()
    {
        if (floatUp)
            FloatingUp();
        else
            FloatingDown();

        if (changeUI)
            MakeSoundUI();
        else
            HoldSoundUI();
    }


    void MakeSoundUI()
    {
        if (lockUI)
        {
            lockUI = false;
            if (!highDone)
            {
                highLowObj[0].SetActive(true);
                highLowObj[2].SetActive(false);
            }
            else
            {
                highLowObj[1].SetActive(true);
                highLowObj[3].SetActive(false);
            }
            Invoke("SetUI", 4);
        }
    }

    void HoldSoundUI()
    {
        if (!lockUI)
        {
            lockUI = true;
            if (!highDone)
                RevertHigh();
            else
                RevertLow();

            Invoke("SetUI", 4);
        }
    }


    void SetUI()
    {
        if (changeUI)
            changeUI = false;
        else
            changeUI = true;
    }

    void RevertHigh()
    {
        highLowObj[0].SetActive(false);
        highLowObj[2].SetActive(true);
        holdImage[0].sprite = holdWhite[0];
        Invoke("RevertHighEnd", 0.1f);
    }

    void RevertHighEnd()
    {
        holdImage[0].sprite = holdRef[0];
    }

    void RevertLow()
    {
        highLowObj[1].SetActive(false);
        highLowObj[3].SetActive(true);

        holdImage[1].sprite = holdWhite[1];
        Invoke("RevertLowEnd", 0.1f);
    }

    void RevertLowEnd()
    {
        holdImage[1].sprite = holdRef[1];
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
            highDone = true;
            highLowObj[0].SetActive(false);
            highLowObj[2].SetActive(false);
            highLowObj[1].SetActive(true);
            return;
        }

        highCount++;

        if (highCount > 1)
        {
            highDone = true;
            highLowObj[0].SetActive(false);
            highLowObj[2].SetActive(false);
            highLowObj[1].SetActive(true);
            return;
        }
    }

    public void LowHoldEvent(bool changeUI)
    {
        if (changeUI)
        {
            Invoke("RevertLow", 1f);

        }
        else if (!changeUI)
        {
            highLowObj[1].SetActive(true);
            highLowObj[3].SetActive(false);
        }
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
            return;
        }
    }

    IEnumerator SquashStretch(Vector2 scaleMultiplier, bool scale)
    {
        float elapsedTime = 0;

        while (elapsedTime < 1)
        {
            elapsedTime += Time.deltaTime;
            float ease = Mathf.Clamp(elapsedTime, 0, 1);

            if (scale)
            {
                Vector3 easeVector = Vector3.Lerp(scaleRef, scaleMultiplier, curve.Evaluate(ease));
                highLowRect[0].sizeDelta = easeVector;
                highLowRect[1].sizeDelta = easeVector;
                highLowRect[2].sizeDelta = easeVector;
                highLowRect[3].sizeDelta = easeVector;
            }
            else
            {
                Vector3 easeVector = Vector3.Lerp(scaleMultiplier, scaleRef, curve.Evaluate(ease));
                highLowRect[0].sizeDelta = easeVector;
                highLowRect[1].sizeDelta = easeVector;
                highLowRect[2].sizeDelta = easeVector;
                highLowRect[3].sizeDelta = easeVector;
            }

            yield return null;
        }
    }
}
