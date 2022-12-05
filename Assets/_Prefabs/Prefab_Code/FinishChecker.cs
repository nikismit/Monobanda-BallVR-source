using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class FinishChecker : MonoBehaviour
{
  public bool isDone;
  private float counter;
  public AudioMovement[] players;
  public float beforeReload = 3f;
  // public Camera cam1,cam2;
  public SmoothFollow cam1;
    // Start is called before the first frame update
  public float timer = 0f;

  public GameObject Player;
  public GameObject Winner;
  public int FinalTime;

    [SerializeField] RectTransform uiTransform;
    [SerializeField] AnimationCurve curve;
    [SerializeField] WinState winState;

    void Start()
    {
        isDone = false;
        uiTransform.sizeDelta = new Vector2(scale, scale);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Finish")
        {
            Winner = Player;
            float f = timer - 5f;
            FinalTime = (int)Mathf.Round(f);
            Finnish();
        }
    }

    float scale = 0;

    IEnumerator Finnish()
    {
        float elapsedTime = 0;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime;

            timer += Time.unscaledDeltaTime;
            players[0].currentAmp = 0f;
            players[1].currentAmp = 0f;
            cam1.enabled = false;
            float ease = Mathf.Clamp(elapsedTime, 0, 1);
            uiTransform.sizeDelta = new Vector2(curve.Evaluate(ease) * 6047, curve.Evaluate(ease) * 1588.518f);

            yield return null;
        }

        Invoke("ReloadScene", 2);
    }

    /*
    private void Update()
    {
        if (timer <= 1 && isDone)
        {
            timer += Time.unscaledDeltaTime;
            Debug.Log("Timer = " + timer);
            players[0].currentAmp = 0f;
            players[1].currentAmp = 0f;
            //player2.currentAmp = 0f;
            // var cam1Component = cam1.GetComponent<>();
            cam1.enabled = false;
            //float ease = Mathf.Lerp(0, easeOutlength / 2, timer * 1.2f);
            //float ease = Mathf.Lerp(0, 3, timer);
            float ease = Mathf.Clamp(timer , 0, 1);
            uiTransform.sizeDelta = new Vector2(curve.Evaluate(ease) * 6047, curve.Evaluate(ease) * 1588.518f);

            if (ease == 1)
            {
                //demoUI.RemoveDemoUIEvent();
                //StartCoroutine(StartTransition());
                Invoke("ReloadScene", 2);
            }

        }
    }
    */

    void ReloadScene()
    {
        //SceneManager.LoadScene("EndlessRunnerTEST_MainMultiplayer");
        winState.ScoreWinner();
    }
}
