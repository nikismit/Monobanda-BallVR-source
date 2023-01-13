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
        if (!playerPitchOneDone)
            PlayerPitchInput(0);
        else if (!playerPitchTwoDone && demo.highCount == 2 && !demo.tutHandler.androidDebug || !playerPitchTwoDone && demo.tutHandler.androidDebug)
            PlayerPitchInput(1);

        if (player.hasStarted)
            gameObject.SetActive(false);
    }

    bool invokeOnce = false;
    float velocity;
    [SerializeField] float pitchVal;

    bool particlePlays = false;

    void PlayerPitchInput(int minMaxPitch)
    {
        if(player.currentPitch >= 7)
        pitchVal = Mathf.Lerp(velocity, player.currentPitch, Time.deltaTime);

        velocity = pitchVal;

        if (!playerPitchOneDone && player.pitch._currentPublicAmplitude >= -30 && pitchVal > 15 && elapsedTime <= 4.5f && allowCountdown
            || playerPitchOneDone && player.pitch._currentPublicAmplitude >= -30 && pitchVal < player.maximumPitch && elapsedTime <= 4.5f && pitchVal > 0 && allowCountdown)
        {
            if (minMaxPitch == 0)
                demo.HighHoldEvent(true);
            else
                demo.LowHoldEvent(true);

            ringParticle.startSize = player.currentPitch;

            if (!particlePlays)
            {
                particlePlays = true;
                ringParticle.Play();
            }

            elapsedTime += Time.deltaTime;

            if (elapsedTime < 0.5f)
            {
                countDownImages[0].SetActive(false);
            }
            else if (elapsedTime < 1.5f)
                countDownImages[0].SetActive(true);
            else if (elapsedTime < 2.5f)
            {
                countDownImages[0].SetActive(false);
                countDownImages[1].SetActive(true);
            }
            else if (elapsedTime < 3.5f)
            {
                countDownImages[1].SetActive(false);
                countDownImages[2].SetActive(true);
            }
            else if (elapsedTime < 4.5f)
            {
                countDownImages[2].SetActive(false);
                countDownImages[3].SetActive(true);

                if (!invokeOnce)
                {
                    invokeOnce = true;
                    if (minMaxPitch == 0)
                    {
                        Invoke("ResetCountDownOne", 1);
                    }
                    else if (minMaxPitch > 0)
                    {
                        Invoke("ResetCountDownTwo", 1);
                    }
                }
            }
            else if(elapsedTime > 4.5f)
            {
                if (minMaxPitch == 0)
                    demo.HighHoldEvent(false);
                else
                    demo.LowHoldEvent(false);

                particlePlays = false;
                ringParticle.Stop();
            }

        }
        else
        {
            if (minMaxPitch == 0)
                demo.HighHoldEvent(false);
            else
                demo.LowHoldEvent(false);

            if (particlePlays)
            {
                particlePlays = false;
                ringParticle.Stop();
            }

            //ringParticle.Stop();
            countDownImages[0].SetActive(false);
            countDownImages[1].SetActive(false);
            countDownImages[2].SetActive(false);
            countDownImages[3].SetActive(false);
            elapsedTime = 0;
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

        elapsedTime = 0;
        countDownImages[3].SetActive(false);
        allowCountdown = true;
        invokeOnce = false;
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
        countDownImages[3].SetActive(false);
        allowCountdown = true;
        invokeOnce = false;


        ringParticle.loop = false;
        ringParticle.Stop();

        //demoUI.RemoveDemoUIEvent();

        gameObject.SetActive(false);
    }
}
