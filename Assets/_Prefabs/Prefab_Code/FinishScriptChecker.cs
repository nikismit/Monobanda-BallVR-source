using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishScriptChecker : MonoBehaviour
{
  public int Count;
  public bool canFinish;
  public FinishScript script;
    void Start()
    {
      Count = 0;
      canFinish = false;
    }


    void FixedUpdate()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        Count += 1;
        if (Count > 1 && script.hasStarted == true){
          canFinish = true;
        }
    }
}
