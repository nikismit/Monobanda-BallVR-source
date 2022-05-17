using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pathScript : MonoBehaviour
{
  public List<GameObject> points;
    // Start is called before the first frame update
    void Start()
    {
      // for (int i = 0; i < 6; i++){
      //
      // }
      foreach(GameObject fooObj in GameObject.FindGameObjectsWithTag("Point")) {

             points.Add(fooObj);
         }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
