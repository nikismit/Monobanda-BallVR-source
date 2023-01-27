using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathOptions : MonoBehaviour
{
    public void QuitApplication()
    {
        Application.Quit();
    }

    public void RestartScene()
    {
        Time.timeScale = 1;
    }
}
