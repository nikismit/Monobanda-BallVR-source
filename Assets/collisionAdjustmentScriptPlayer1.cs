using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionAdjustmentScriptPlayer1 : MonoBehaviour
{
  public pathScript path;
  public GameObject car;
  public Transform vehicle;
  public List<Vector3> posDiff;
  public Vector3 minVector;
  public Rigidbody m_Rigidbody;
  public GameObject closest = null;
  private Transform closestTransform;

  public int index = 0;
  public float P1Pos = 0f;
    List<GameObject> point;

    public void playerPosition(){
        /*

        //point = path.points;
        var leng = path.points.Count;
        //var leng = point.Count;
      index = path.points.FindIndex(gameObject => string.Equals(closest.name, gameObject.name));
        //Debug.LogWarning(leng);
      P1Pos = leng-index;
      P1Pos = 1f-(P1Pos/leng);

        */
    }


    public GameObject FindClosestEnemy()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Point");
        // GameObject closest = null;
        closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = vehicle.transform.position;
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
      playerPosition();

        //if (knockingBack)
            //CrashKnockBack();
    }

    public void onCollisionCorrection(){
      posDiff.Clear();
      var pathLength = path.points.Count;

        if (pathLength == 0)
            pathLength = path.points.Count;

      for (int i=0; i < pathLength; i++){
          var pointTransform = path.points[i].transform.position;
          var posDifference = pointTransform - vehicle.transform.position;
          posDiff.Add(posDifference);
      }
      Vector3 minVector = Vector3.positiveInfinity;
        //Vector3 minVector = new Vector3(transform.position.x + 50, transform.position.y + 50, transform.position.z + 50);
        //Vector3 minVector = Vector3.forward;
        Vector3 maxVector = Vector3.zero;

        //Debug.Log(posDiff[0]);
        for (int i = 0; i < posDiff.Count; i++){


            minVector = (posDiff[i].magnitude < minVector.magnitude) ?  posDiff[i] : minVector;
        maxVector = (posDiff[i].magnitude > maxVector.magnitude) ?  posDiff[i] : maxVector;
        }
        //m_Rigidbody.AddForce(new Vector3(minVector.x * 20f, 0f, minVector.z * 20f), ForceMode.Force);
        m_Rigidbody.AddForce(new Vector3(closest.transform.position.x * 20, 0f, closest.transform.position.z * 20), ForceMode.Force);

        //CrashKnockBack();

        StartCoroutine(CrashKnockBack());


        car.transform.eulerAngles = closest.transform.eulerAngles;
    }

    IEnumerator CrashKnockBack()
    {
        m_Rigidbody.drag = 2;

        yield return new WaitForSeconds(1);

        m_Rigidbody.drag = 0;

        yield return null;
    }
}
