using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class FinishChecker : MonoBehaviour
{
  public bool isDone;
  private float counter;
  public AudioMovement player1;
  public AudioMovementPlayer2 player2;
  public FinishChecker otherPlayerFinishChecker;
  public float beforeReload = 3f;
  // public Camera cam1,cam2;
  public SmoothFollow cam1,cam2;
    // Start is called before the first frame update
  public float timer = 0f;

  public GameObject Player;
  public GameObject Winner;
  public int FinalTime;

    void Start()
    {
        isDone = false;
    }

    private void OnTriggerEnter(Collider other)
        {
    		  if (other.gameObject.tag == "Finish" && otherPlayerFinishChecker.isDone == false){
            Winner = Player;
            float f = timer - 5f;
            FinalTime = (int) Mathf.Round(f);
            Debug.Log(Winner);
            Debug.Log(FinalTime);
            PlayerPrefs.SetString("Winner",Winner.name);
            PlayerPrefs.SetInt("Time",FinalTime);
    				isDone = true;
    			}
        }

    void FixedUpdate()
    {
      timer += 0.02f;
        if(isDone == true){
          counter += 0.02f;
          player1.currentAmp=0f;
          player2.currentAmp=0f;
          // var cam1Component = cam1.GetComponent<>();
          cam1.enabled = false;
          cam2.enabled = false;

          if (counter>=beforeReload){
            //SceneManager.LoadScene("CityDriver - 2nd Prototype - SwervyCoopMap");
            //Now we can send the players to a winner screen!! and display their character, the loser and the time to beat the character

            SceneManager.LoadScene("CityDriver - 2nd Prototype - SwervyCoopMap - WinnerScreen");
          }

        }
    }
}
