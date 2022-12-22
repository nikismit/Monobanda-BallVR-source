using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinnerScreenCode : MonoBehaviour
{
  public string winner;
  public int time;
  public GameObject WinnerP1;
  public GameObject WinnerP2;
  public GameObject LoserP1;
  public GameObject LoserP2;

  public GameObject P1Image;
  public GameObject P2Image;
  public Text timeDisplay;


    // Start is called before the first frame update
    void Start()
    {
      winner = PlayerPrefs.GetString("Winner");
      time = PlayerPrefs.GetInt("Time");
      string display = time.ToString();
      timeDisplay.text = display + " Seconds";
      if (winner == "Player1"){
        WinnerP1.SetActive(true);
        WinnerP2.SetActive(false);
        LoserP1.SetActive(false);
        LoserP2.SetActive(true);
        P1Image.SetActive(true);
        P2Image.SetActive(false);
      }
      else if (winner == "Player2"){
        WinnerP1.SetActive(false);
        WinnerP2.SetActive(true);
        LoserP1.SetActive(true);
        LoserP2.SetActive(false);
        P1Image.SetActive(false);
        P2Image.SetActive(true);
      }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
