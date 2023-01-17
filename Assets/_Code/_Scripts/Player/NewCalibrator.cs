using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCalibrator : MonoBehaviour
{
    [SerializeField] GameObject[] countDownImages;
    [SerializeField] DemoHighLow demo;
    [SerializeField] DemoUI demoUI;
    private AudioMovement player;

    bool playerPitchOneDone, playerPitchTwoDone;
    private bool allowCountdown = true;

    float elapsedTime = 0;
    [SerializeField] float pitchSmooth;

    [SerializeField] ParticleSystem ringParticle;

    bool doOnce = false;

    [SerializeField] int playerID;

    void Start()
    {
        player = GetComponentInParent<AudioMovement>();
        ringParticle.loop = true;

        if (player.debugKeyControl)
        {
            ringParticle.loop = false;
            demo.tutHandler.SpawnPlane();
        }
    }

    void Update()
    {

        if (demo.highCount == 2 && playerPitchOneDone && demo.lowCount == 0)
            countDownImages[3].SetActive(false);
        else if (demo.lowCount == 2)
            countDownImages[3].SetActive(false);

        if (playerPitchTwoDone && demo.lowCount == 1 && !demo.tutHandler.androidDebug)
            countDownImages[3].SetActive(true);
        else if (playerPitchTwoDone && demo.lowCount == 2)
            gameObject.SetActive(false);

        Debug.Log("demoLow = " + demo.lowCount);

        if (!playerPitchOneDone)
            PlayerPitchInput(0);
        else if (!playerPitchTwoDone && demo.highCount == 2 && !demo.tutHandler.androidDebug || !playerPitchTwoDone && demo.tutHandler.androidDebug)
        {
            PlayerPitchInput(1);
        }


        if (!particlePlays && player.pitch._currentPublicAmplitude >= -80)
        {
            particlePlays = true;
            ringParticle.Play();
        }
        else if (particlePlays && player.pitch._currentPublicAmplitude <= -80)
        {
            particlePlays = false;
            ringParticle.Stop();
        }

        if (particlePlays)
            ringParticle.startSize = player.currentPitch;


        if (player.hasStarted)
            gameObject.SetActive(false);
    }
    float velocity;
    [SerializeField] float pitchVal;

    bool particlePlays = false;

    void PlayerPitchInput(int minMaxPitch)
    {
        if(player.currentPitch >= 7)
        pitchVal = Mathf.Lerp(velocity, player.currentPitch, Time.deltaTime);

        velocity = pitchVal;




        if (!playerPitchOneDone && player.pitch._currentPublicAmplitude >= -30 && pitchVal > 15 && elapsedTime <= 2.5f && allowCountdown
            || playerPitchOneDone && player.pitch._currentPublicAmplitude >= -30 && pitchVal < player.maximumPitch && elapsedTime <= 2.5f && pitchVal > 0 && allowCountdown)
        {




            elapsedTime += Time.deltaTime;

            if (elapsedTime < 0.5f)
            {
                countDownImages[0].SetActive(false);
            }
            else if (elapsedTime < 1f)
                countDownImages[0].SetActive(true);
            else if (elapsedTime < 1.5f)
            {
                countDownImages[0].SetActive(false);
                countDownImages[1].SetActive(true);
            }
            else if (elapsedTime < 2f)
            {
                countDownImages[1].SetActive(false);
                countDownImages[2].SetActive(true);
            }
            else if (elapsedTime < 2.5f)
            {
                countDownImages[2].SetActive(false);
                countDownImages[3].SetActive(true);

                if (minMaxPitch == 0 && !doOnce)
                {
                    doOnce = true;
                    Invoke("ResetCountDownOne", 1);
                    ringParticle.Stop();
                }
                else if (minMaxPitch > 0 && !doOnce)
                {
                    doOnce = true;
                    Invoke("ResetCountDownTwo", 1);
                    ringParticle.Stop();
                }
            }
            else if(elapsedTime > 2.5f)
            {
                doOnce = false;
                particlePlays = false;
                ringParticle.Stop();
            }

        }
        else
        {


            //ringParticle.Stop();
            countDownImages[0].SetActive(false);
            countDownImages[1].SetActive(false);
            countDownImages[2].SetActive(false);
            //countDownImages[3].SetActive(false);
            elapsedTime = 0;
            doOnce = false;
        }
    }

    void ResetCountDownOne()
    {
        playerPitchOneDone = true;

        //if (pitchSmooth > 7)
            player.maximumPitch = pitchVal;
        //else
            //player.maximumPitch = player.lastValidPitch;

        demo.HighEvent();
        ringParticle.Stop();
        elapsedTime = 0;
        if(demo.tutHandler.androidDebug)
            countDownImages[3].SetActive(false);
        allowCountdown = true;
    }

    void ResetCountDownTwo()
    {
        playerPitchTwoDone = true;

       // if (pitchSmooth > 7)
            player.minimumPitch = pitchVal;
        //else
            //player.minimumPitch = player.lastValidPitch;

        demo.LowEvent();

        elapsedTime = 0;
        if (demo.tutHandler.androidDebug)
            countDownImages[3].SetActive(false);
        allowCountdown = true;


        ringParticle.loop = false;
        ringParticle.Stop();

        //demoUI.RemoveDemoUIEvent();


    }
}
