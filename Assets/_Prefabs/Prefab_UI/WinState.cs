using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinState : MonoBehaviour
{
    public GameObject[] winUI;
    DemoUI demoUI;
    public AnimationCurve curve;
    private bool initialized = false;
    private float timer = 0;
    private float timeLength = 3;

    private int winner;

    private void Start()
    {
        demoUI = GetComponent<DemoUI>();
    }

    private void Update()
    {
        if (timer <= timeLength && initialized)
        {
            timer += Time.unscaledDeltaTime;
            //float ease = Mathf.Lerp(0, easeOutlength / 2, timer * 1.2f);
            //float ease = Mathf.Lerp(0, 3, timer);
            float ease = Mathf.Clamp(timer / 3, 0, 1);
            Time.timeScale = curve.Evaluate(ease);

            if (ease == 1)
            {
                //demoUI.RemoveDemoUIEvent();
                StartCoroutine(StartTransition());
            }

        }
    }

    IEnumerator StartTransition()
    {
        //yield return new WaitForSeconds(1.5f);
        yield return new WaitForSecondsRealtime(1.5f);
        demoUI.RemoveDemoUIEvent();
    }

    public void PlayerOneWins()
    {
        if (!initialized)
        {
            initialized = true;
            winner = 0;
            //Time.timeScale = 0;
            //StartCoroutine(ResetScene(2, 0));
            //Invoke("ResetScene", 2);
        }
    }

    public void PlayerTwoWins()
    {
        if (!initialized)
        {
            initialized = true;
            //winUI[1].gameObject.SetActive(true);
            //Time.timeScale = 0;
            //StartCoroutine(ResetScene(2, 1));

            winner = 1;
            //Invoke("ResetScene", 2);
        }
    }
    public IEnumerator ResetScene(float waitTime)
    {
        //Time.timeScale = 0;
        //Time.timeScale = 0;
        //Time.fixedDeltaTime = Time.timeScale * 0.02f;

        //yield return new WaitForSecondsRealtime(5);
        winUI[winner].gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(waitTime);
        Time.timeScale = 1;
        SceneManager.LoadScene("EndlessRunnerTEST_MainMultiplayer");
        //DO STUFF
    }
    /*
    void ResetScene()
    {
        SceneManager.LoadScene("EndlessRunnerTEST_MainMultiplayer");
    }
    */
}
