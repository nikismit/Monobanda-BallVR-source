using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pathScript : MonoBehaviour
{
  public List<GameObject> points;
    // Start is called before the first frame update
    void Start()
    {
          //This has been removed because it caused a weird bug when building ----> the points were loaded in in the wrong order.

      // foreach(GameObject fooObj in GameObject.FindGameObjectsWithTag("Point")) {
      //
      //        points.Add(fooObj);
      //    }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
