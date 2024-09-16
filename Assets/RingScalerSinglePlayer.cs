using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingScalerSinglePlayer : MonoBehaviour
{
    public List<GameObject> rings;
    public AudioMovement P1;
    //public AudioMovementPlayer2 P2;
    public float scale = 1f;
    private float multiplier = 60f;
    // Start is called before the first frame update
    void Start()
    {
      foreach(GameObject fooObj in GameObject.FindGameObjectsWithTag("Ring")) {

             rings.Add(fooObj);
         }
    }

    public void averageVolume(){
      scale = 2+((P1.currentAmp)/multiplier);
      scale = Mathf.Abs(scale);
    }


    void Update()
    {
      averageVolume();
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
