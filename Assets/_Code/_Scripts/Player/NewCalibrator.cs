using System.Collections;
using UnityEngine;

public class NewCalibrator : MonoBehaviour
{
    [SerializeField] private GameObject[] countDownImages;
    [SerializeField] private DemoHighLow demo;
    [SerializeField] private DemoUI demoUI;
    
    [SerializeField] private Transform playerTrans;
    [SerializeField] private float pitchSmooth;
    [SerializeField] private ParticleSystem ringParticle;
    [SerializeField] private int playerID;
    [SerializeField] private float pitchVal;

    [SerializeField]
    private bool UpdateUILocation;
    [SerializeField]
    private float offset;

    private RectTransform transform;

    private bool playerPitchOneDone, playerPitchTwoDone;
    private bool allowCountdown = true;
    private bool returnToStartLoc = false;
    private float elapsedTime = 0;
    private bool doOnce = false;
    private float velocity;
    private bool particlePlays = false;
    private bool revertToStartLoc = false;
    private AudioMovement player;
    private Vector3 refPos;
    private float refvelocity;

    void Start()
    {
        transform = base.transform as RectTransform;

        refPos = playerTrans.position;
        player = playerTrans.GetComponentInParent<AudioMovement>();
        ringParticle.loop = true;

        if (player.debugKeyControl)
        {
            ringParticle.loop = false;
            demo.tutHandler.SpawnPlane();
        }

        if (demo.tutHandler.androidDebug)
        {
            player.minimumAmp = -25f;
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

    void LateUpdate()
    {
        if( ! UpdateUILocation) 
            return;

        Vector3 targetPosition = Camera.main.WorldToScreenPoint( playerTrans.position );
        transform.position = targetPosition + Vector3.up * offset;

        Vector3 position =transform.localPosition;
        position.z = 0;
        transform.localPosition = position;
    }
    
    void PlayerPitchInput(int minMaxPitch)
    {
        if(player.currentPitch >= 7)
        pitchVal = Mathf.Lerp(velocity, player.currentPitch, Time.deltaTime);

        velocity = pitchVal;

        if (!playerPitchOneDone && player.pitch._currentPublicAmplitude >= -30 && pitchVal > 15 && elapsedTime <= 2.5f && allowCountdown
            || playerPitchOneDone && player.pitch._currentPublicAmplitude >= -30 && pitchVal < player.maximumPitch && elapsedTime <= 2.5f && pitchVal > 0 && allowCountdown)
        {
            elapsedTime += Time.deltaTime;

            if (revertToStartLoc)
            {
                Debug.Log("Revert to location false");
                revertToStartLoc = false;
                returnToStartLoc = false;
                player.isInPipe = false;
                StopAllCoroutines();
            }


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
            if (!revertToStartLoc)
            {
                Debug.Log("Revert to start location true");
                revertToStartLoc = true;
                StartCoroutine(RevertToStartLoc());
            }


            countDownImages[0].SetActive(false);
            countDownImages[1].SetActive(false);
            countDownImages[2].SetActive(false);

            elapsedTime = 0;
            doOnce = false;
        }

        if (returnToStartLoc)
        {
            player.isInPipe = true;
            playerTrans.position = new Vector3(0, 0.5f, Mathf.SmoothDamp(playerTrans.position.z, refPos.z, ref refvelocity, 1, 200 * Time.deltaTime));
        }
    }

    void ResetCountDownOne()
    {
        playerPitchOneDone = true;

        player.maximumPitch = pitchVal;

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
        player.minimumPitch = pitchVal;

        demo.LowEvent();

        elapsedTime = 0;
        if (demo.tutHandler.androidDebug)
            countDownImages[3].SetActive(false);
        allowCountdown = true;

        ringParticle.loop = false;
        ringParticle.Stop();
    }

    IEnumerator RevertToStartLoc()
    {
        yield return new WaitForSeconds(2);
        returnToStartLoc = true;
    }
}
