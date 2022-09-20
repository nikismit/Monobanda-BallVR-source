using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPathFollowDriver : MonoBehaviour
{
  public pathScript path;
  public GameObject car;
  public Transform vehicle;
  public List<Vector3> posDiff;
  public Vector3 minVector;
  public Rigidbody m_Rigidbody;
  public GameObject closest = null;
  private Transform closestTransform;
  public AudioMovement Player1;
  public AudioPitch_Player1 pitch;
  public SFXManager sfx;
  private LayerMask layer;

    private AudioMovement player;//Debug purpose


    [Range(0.01f,1f)]
  public float AiDisadvantage = 1f;

  public float currentSpeed = 0f;

  public int index = 0;
  public float P1Pos = 0f;

  public float turningRate = 10f;

    private bool ignorePlayerCol;
    private Collider col;

    //private Quaternion _targetRotation = Quaternion.identity;

    private void Start()
    {
        ignorePlayerCol = Player1.ignorePlayerCol;
        col = gameObject.GetComponent<Collider>();

        layer = 8;
        player = GameObject.FindObjectOfType<AudioMovement>();
    }

    void OnCollisionEnter(Collision collision)
{
        if (collision.gameObject.tag == "Player")
        {
            Physics.IgnoreCollision(collision.collider, col, ignorePlayerCol);
        }

        if (collision.gameObject.tag == "Box"){
    currentSpeed = currentSpeed;
  }
  else if(collision.gameObject.tag == "Track" || player.setCrashCol && collision.gameObject.tag == "Player" && !ignorePlayerCol)
        {

            Vector3 col = Vector3.Reflect(transform.forward, collision.contacts[0].normal);


            if (player.testRailControl)//TODO: only works on right wall
            {
                Vector3 reflect = Vector3.zero;

                if (player.currentTurn > 0)
                    reflect = Vector3.Reflect(-transform.right, collision.contacts[0].normal);
                else
                    reflect = Vector3.Reflect(transform.right, collision.contacts[0].normal);
                transform.Translate(reflect, Space.World);
            }
            else
            onCollisionCorrection(col);

    sfx.crashIntoTrack();
    if(currentSpeed> Player1.maximumForwardSpeed){
        currentSpeed = 0.50f * currentSpeed;
    }
    else if (currentSpeed< 0.75f*Player1.maximumForwardSpeed){
      currentSpeed = 0.80f * currentSpeed;
    }
    else{
      currentSpeed = 0.70f * currentSpeed;
    }
  }
  else{
    currentSpeed = 0.75f * currentSpeed;
  }
}
private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.tag == "Ring"){
      currentSpeed = Player1.speedBoost * 0.75f * currentSpeed;
    }

        if (other.gameObject.GetComponent<JumpPad>())
            JumpBoost(other.gameObject.GetComponent<JumpPad>().jumpStrength);
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
        m_Rigidbody.AddForce(wallCol * 10, ForceMode.Impulse);

        car.transform.eulerAngles = closest.transform.eulerAngles;
    }

    // public void SetBlendedEulerAngles(Vector3 angles)
    // {
    //   _targetRotation = Quaternion.Euler(angles);
    // }

    void FixedUpdate(){
      FindClosestEnemy();
      var randomVolume = Random.Range(-60.0f, 0.0f);

      //this.transform.eulerAngles = closest.transform.eulerAngles;
      Vector3 targetDirection = closest.transform.forward;
       Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, turningRate * Time.deltaTime,0.0f);
       transform.rotation = Quaternion.LookRotation(newDirection);

      if (randomVolume>pitch.minVolumeDB){
        if(currentSpeed < Player1.maximumForwardSpeed){
  				currentSpeed += Player1.forwardAccelaration * Time.fixedDeltaTime * AiDisadvantage;
  			}
        else{
  				currentSpeed -= Player1.forwardDecelerationSinglePlayer * Time.fixedDeltaTime * AiDisadvantage;
  			}
        this.transform.Translate(transform.forward * currentSpeed * Time.fixedDeltaTime, Space.World);
      }
      else{
        currentSpeed -= Player1.forwardDecelerationSinglePlayer * Time.fixedDeltaTime;
        this.transform.Translate(transform.forward * currentSpeed * Time.fixedDeltaTime, Space.World);
      }

        if (!IsGrounded())
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0), 1 * Time.deltaTime);
    }

    public bool IsGrounded()
    {
        if (Physics.Raycast(transform.position, Vector3.down + Vector3.forward, 5, layer) ||
            Physics.Raycast(transform.position, Vector3.down + Vector3.back, 5, layer) ||
            Physics.Raycast(transform.position, Vector3.down + Vector3.left, 5, layer) ||
            Physics.Raycast(transform.position, Vector3.down + Vector3.right, 5, layer))
            return true;
        else
            return false;
    }

    public void JumpBoost(float jumpBoost)
    {
        //Debug.LogError("JAJA");
        m_Rigidbody.AddForce(transform.up * jumpBoost, ForceMode.Impulse);
        //this.transform.Translate(transform.up * jumpBoost * Time.fixedDeltaTime, Space.World);
    }
}
