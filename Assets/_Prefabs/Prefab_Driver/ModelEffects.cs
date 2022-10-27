using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelEffects : MonoBehaviour
{
    [SerializeField] Transform playerModel;
    [HideInInspector] public float dir;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //playerModel.Rotate(Vector3.forward, 10 * Time.deltaTime);
        //if(dir == 0)
        //playerModel.localRotation = Quaternion.Slerp(transform.localRotation, new Quaternion(0,0,0,0), 20 * Time.deltaTime);


        //Debug.Log(playerModel.rotation.eulerAngles.z);
        //Debug.Log();


        Vector3 rotateVec = new Vector3(dir, 0, 0).normalized;

        float rotateAmount = dir * 12;

        Quaternion rotationZ = Quaternion.AngleAxis(-rotateAmount, Vector3.forward);

        playerModel.localRotation = Quaternion.Slerp(playerModel.localRotation, rotationZ, 10 * Time.deltaTime);

    }

    public void RotateModel(float dirInput)
    {
        /*
        dir = dirInput;

        Vector3 rotateVec = new Vector3(dirInput, 0, 0).normalized;
        //Debug.Log("Rotation = " + playerModel.rotation.eulerAngles.z);

        if (playerModel.rotation.eulerAngles.z < 50 && dirInput <= 0)
            playerModel.Rotate(Vector3.forward, -rotateVec.x * 150 * Time.deltaTime);

        
        if (playerModel.rotation.eulerAngles.z > 300 && playerModel.rotation.eulerAngles.z < 50  && dirInput >= 0)
            playerModel.Rotate(Vector3.forward, -rotateVec.x * 150 * Time.deltaTime);
        */

        Vector3 rotateVec = new Vector3(dirInput, 0, 0).normalized;

        float rotateAmount = rotateVec.x * 2;

        Quaternion rotationZ = Quaternion.AngleAxis(rotateAmount, Vector3.forward);

        playerModel.localRotation = Quaternion.Slerp(transform.localRotation, rotationZ, 20 * Time.deltaTime);

    }
}
