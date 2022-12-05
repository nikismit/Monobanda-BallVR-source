using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionAdjustmentScriptPlayer2 : MonoBehaviour
{
  public pathScript path;
  public GameObject car;
  public Transform vehicle;
  public List<Vector3> posDiff;
  public Vector3 minVector;
  public Rigidbody m_Rigidbody;
  public GameObject closest = null;
  private Transform closestTransform;
  private bool timerSwitch = false;
  private float timer = 0f;


  public int index = 0;
  public float P2Pos = 0f;

    Vector3 position;

    public void playerPosition(){
      var leng = path.points.Count;
      index = path.points.FindIndex(gameObject => string.Equals(closest.name, gameObject.name));

      P2Pos = leng-index;
      P2Pos = 1f-(P2Pos/leng);
    }

    public GameObject FindClosestEnemy()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Point");
        // GameObject closest = null;
        closest = null;
        float distance = Mathf.Infinity;
        if(vehicle != null)
        position = vehicle.transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
        Debug.Log(closest);
    }
    void FixedUpdate(){
      FindClosestEnemy();
     //playerPosition();
    }

    public void onCollisionCorrection(Vector3 wallCol)
    {
      
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
      //m_Rigidbody.AddForce(new Vector3(minVector.x*20f,0f,minVector.z*20f), ForceMode.Force);
     //car.transform.eulerAngles = closest.transform.eulerAngles;

        m_Rigidbody.AddForce(wallCol * 10, ForceMode.Impulse);
    }
}
