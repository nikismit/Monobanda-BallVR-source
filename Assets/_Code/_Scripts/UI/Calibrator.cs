using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calibrator : MonoBehaviour
{
    [SerializeField] int playerInt;
    [Space(20)]
    private AudioMovement[] players = new AudioMovement[2];
    GameObject[] playerObj;

    float minPitch;
    float maxPitch;

    private float timer = 0;
    private float maxTime = 1.5f;
    private bool stopCalPitch = false;

    private int[] pitchCount = new int[2];
    private float currentVel = 0;

    [SerializeField] Slider[] loadSliders;

    [SerializeField] RectTransform barScaler;
    [SerializeField] Slider pitchSlider;

    [SerializeField] Image backgroundImage;
    [SerializeField] GameObject makeSoundObj;

    bool noInput = true;

    private Camera cam;
    public RectTransform canvasRect;
    public RectTransform pointEnd;
    public Transform line;

    public RectTransform pointBegin;
    [SerializeField] private TutorialHandler tutHandler;

    void Start()
    {
        if (tutHandler.androidDebug && playerInt == 1)
        {
            gameObject.SetActive(false);
        }
        //if (Application.platform == RuntimePlatform.Android && playerInt == 1)
            //Destroy(gameObject);

        playerObj = GameObject.FindGameObjectsWithTag("Player");
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        //rectTransform = transform.GetChild(1).GetComponent<RectTransform>();

        //playerTrans = player[playerInt].transform;

        loadSliders[0].maxValue = maxTime;
        loadSliders[1].maxValue = maxTime;

        for (int i = 0; i < playerObj.Length; i++)
        {
            players[i] = playerObj[i].GetComponent<AudioMovement>();

            if (players[i].debugKeyControl)
                gameObject.SetActive(false);
        }

        pitchSlider.maxValue = players[0].maximumPitch;
    }

    void Update()
    {
        if(pitchCount[0] == 0 && !stopCalPitch)
            CalibratePitch(playerInt, 0);
        if (pitchCount[0] == 1 && !stopCalPitch)
            CalibratePitch(playerInt, 1);
        if (pitchCount[0] == 2 && !stopCalPitch)
        {
            //StartGame
            players[playerInt].removeControl = false;
            tutHandler.calibratorsComplete++;
            tutHandler.SpawnPlane();
            gameObject.SetActive(false);
        }





        //var lookAtPoint = point.position;
        //lookAtPoint.x = transform.position.z;
        //line.transform.LookAt(lookAtPoint);
        //line.rotation = Quaternion.LookRotation(point.position, Vector3.right);
        Vector2 direction = pointEnd.position - line.position;
        //line.right = direction;
        line.right = Vector3.SmoothDamp(line.right, direction, ref velocity, 0.5f, 1500);

    }

    Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        Vector2 ViewportPosition = cam.WorldToViewportPoint(playerObj[playerInt].transform.position);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)) + 30);

        //now you can set the position of the ui element
        pointEnd.anchoredPosition = WorldObject_ScreenPosition;



        float distance = Vector3.Distance(pointBegin.transform.position, pointEnd.transform.position);
        line.localScale = new Vector2(distance / 10, 0.5f);
        line.position = (pointBegin.position + pointEnd.position) / 2;
    }
    void CalibratePitch(int player, int minMax)
    {
        loadSliders[pitchCount[player]].value = timer;

        if (minMax == 0 && players[player].pitch._currentPublicAmplitude >= -25 && timer <= 5 && players[player].currentPitch > 15
            || minMax == 1 && players[player].pitch._currentPublicAmplitude >= -25 && timer <= 5 && players[player].pitch._currentPublicAmplitude < players[player].maximumPitch)
        {
            StopAllCoroutines();
            makeSoundObj.SetActive(false);
            noInput = false;
            pitchSlider.value = SmoothPitch(players[player].currentPitch);

            if (players[player].currentPitch > minPitch 
                && players[player].currentPitch < maxPitch
                && players[player].currentPitch > 0
                && players[player].currentPitch < 40)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = 0;
                minPitch = players[player].currentPitch - 5;
                maxPitch = players[player].currentPitch + 5;
            }
        }
        else if (!noInput && players[player].pitch._currentPublicAmplitude <= -45 && !stopCalPitch)
        {
            noInput = true;
            pitchSlider.value = SmoothPitch(0);
            StartCoroutine(ActivateMakeSoundObj());
        }


        if (timer >= maxTime)
        {
            Debug.Log("PitchCount = " + pitchCount[0] + "PitchSet = " + players[player].currentPitch);

            stopCalPitch = true;
            timer = 0;
            players[player].SetPitchVal(minMax);
            StartCoroutine(NextPitch(player));
        }
    }

    IEnumerator ActivateMakeSoundObj()
    {
        yield return new WaitForSeconds(5);
        makeSoundObj.SetActive(true);
    }

    IEnumerator NextPitch(int player)
    {
        backgroundImage.color = Color.green;
        yield return new WaitForSeconds(4);
        backgroundImage.color = Color.white;
        pitchCount[player]++;

        stopCalPitch = false;
    }

    float SmoothPitch(float val)
    {
        float currentPitch = Mathf.SmoothDamp(pitchSlider.value, val, ref currentVel, 5 * Time.deltaTime);
        return currentPitch;
    }
}
