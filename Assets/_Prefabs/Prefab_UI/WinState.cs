using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinState : MonoBehaviour
{
    public GameObject[] winUI;

    public void PlayerOneWins()
    {
        winUI[0].gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void PlayerTwoWins()
    {
        winUI[1].gameObject.SetActive(true);
        Time.timeScale = 0;
    }
}
