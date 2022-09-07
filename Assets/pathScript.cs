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

        if(points.Count != 0)
        {
            GameObject spline = GameObject.FindGameObjectWithTag("PointSpline");
            int child = spline.transform.childCount;

            for (int i = 0; i < child; i++)
            {
                points.Add(spline.transform.GetChild(i).gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
