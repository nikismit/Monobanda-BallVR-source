using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinState : MonoBehaviour
{
    public GameObject[] winUI;

    public void PlayerOneWins()
    {
        winUI[0].gameObject.SetActive(true);
        //Time.timeScale = 0;
        StartCoroutine(ResetScene(2));
        Invoke("ResetScene", 2);
    }

    public void PlayerTwoWins()
    {
        winUI[1].gameObject.SetActive(true);
        //Time.timeScale = 0;
        StartCoroutine(ResetScene(2));
        Invoke("ResetScene", 2);
    }
    IEnumerator ResetScene(float waitTime)
    {
        Time.timeScale = 0;
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
