using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class loadScene : MonoBehaviour
{

    public GameObject MainMenu;
    public GameObject CalibrationMenu;
    public GameObject LevelsMenu;
    public void loadNewScene(){
      SceneManager.LoadScene("CityDriver - 2nd Prototype");
    }
    public void loadCoopScene(){
      SceneManager.LoadScene("CityDriver - 2nd Prototype - 2 Player");
    }
    public void loadMenuScene(){
      SceneManager.LoadScene("CityDriver - 2nd Prototype - MENU");
    }
    public void loadFlyingScene(){
      SceneManager.LoadScene("FlyingFreeRoamDemo");
    }
    public void load2ndCoopScene(){
      SceneManager.LoadScene("CityDriver - 2nd Prototype - 2 Player - StraghtMap Turning Limit");
    }
    public void moveToCalibration(){
      MainMenu.SetActive(false);
      CalibrationMenu.SetActive(true);
    }
    public void moveToLevelsMenu(){
      MainMenu.SetActive(false);
      CalibrationMenu.SetActive(false);
      LevelsMenu.SetActive(true);
    }
    public void ListenPlayer1HighestTone(){
      Debug.Log("ListenPlayer1HighestTone");
    }
}
