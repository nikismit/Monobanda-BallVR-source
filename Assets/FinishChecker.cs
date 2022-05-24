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
  public float beforeReload = 20f;
  // public Camera cam1,cam2;
  public SmoothFollow cam1,cam2;
    // Start is called before the first frame update
    void Start()
    {
        isDone = false;
    }

    private void OnTriggerEnter(Collider other)
        {
    		  if (other.gameObject.tag == "Finish"){
    				isDone = true;
    			}
        }

    void Update()
    {
        if(isDone == true){
          counter += 0.01f;
          player1.currentAmp=0f;
          player2.currentAmp=0f;
          // var cam1Component = cam1.GetComponent<>();
          cam1.enabled = false;
          cam2.enabled = false;

          if (counter>=beforeReload){
            SceneManager.LoadScene("CityDriver - 2nd Prototype - SwervyCoopMap");
          }

        }
    }
}
