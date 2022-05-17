using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionAdjustmentScriptPlayer1 : MonoBehaviour
{
  public pathScript path;
  public Transform vehicle;
  public List<Vector3> posDiff;
  public Vector3 minVector;
  public Rigidbody m_Rigidbody;

// ForTesting!
    // void Update()
    // {
    //   onCollisionCorrection();
    // }

    public void onCollisionCorrection(){
      posDiff.Clear();
      var pathLength = path.points.Count;
      for (int i=0; i < pathLength; i++){
          var pointTransform = path.points[i].transform.position;
          var posDifference = pointTransform - vehicle.transform.position;
          posDiff.Add(posDifference);
      }
      Vector3 minVector = Vector3.positiveInfinity;
      Vector3 maxVector = Vector3.zero;
      for(int i = 0; i < posDiff.Count; i++){
        minVector = (posDiff[i].magnitude < minVector.magnitude) ?  posDiff[i] : minVector;
        maxVector = (posDiff[i].magnitude > maxVector.magnitude) ?  posDiff[i] : maxVector;
      }
      Debug.Log(minVector);
      m_Rigidbody.AddForce(new Vector3(minVector.x*30f,0f,minVector.z*30f), ForceMode.Force);

    }
}
