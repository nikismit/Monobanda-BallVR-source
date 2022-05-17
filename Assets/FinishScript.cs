using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishScript : MonoBehaviour
{
  public int Count;
  public bool hasStarted;
  public bool theEnd;
  public FinishScriptChecker Checker;
  public float timer;

    void Start()
    {
      Count = 0;
      hasStarted = false;
      theEnd = false;
    }


    void FixedUpdate()
    {
      // it needs to be two because two colliders from the hoverer are going through the trigger
      if (Count > 1 && Checker.canFinish == true){
        Debug.Log("You're DONE");
        theEnd = true;
        timer += Time.fixedDeltaTime;
      }
    }

    private void OnTriggerEnter(Collider other)
    {
      if (other.gameObject.tag == "Player"){
        Count += 1;
        if (hasStarted == false && Count%2==1){ //is Odd
          hasStarted = true;
        }
        else if (hasStarted == true && Count%2==0){ //is Even
          hasStarted = false;
        }
      }
    }
}
