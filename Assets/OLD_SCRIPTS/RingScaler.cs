using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingScaler : MonoBehaviour
{
    public List<GameObject> rings;
    public AudioMovement P1;
    public float scale = 1f;
    private float multiplier = 60f;
    // Start is called before the first frame update
    void Start()
    {
      foreach(GameObject fooObj in GameObject.FindGameObjectsWithTag("Ring")) {

             rings.Add(fooObj);
         }
    }



    void Update()
    {
      if (scale>1.1f && scale<2f){
        for (int i=0; i<rings.Count; i++){
          rings[i].transform.localScale = new Vector3(scale,scale*1.2f,scale);
        }
      }
      else{
        scale = 1f;
      }
    }
}
