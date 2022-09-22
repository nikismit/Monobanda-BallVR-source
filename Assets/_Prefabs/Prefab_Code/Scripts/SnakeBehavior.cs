using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class SnakeBehavior : MonoBehaviour
{

    public List<Transform> bodyParts = new List<Transform>();
    //public Transform car;

    public float minDistance;

    [Header("General settings")]
    public float rotationSpeed = 50;

    private int beginSize;
    private Transform curBodyPart;
    private Transform PrevBodyPart;


    private float lineDist = 0;


    void Start()
    {
        /*
        beginSize = bodyParts.Count + 1;

        //lineDist = lineDist + minDistance;

        
        if (minDistances.Length != bodyParts.Count)
            Debug.LogError("The Array minDistance: " + minDistances.Length + " isnt equal to bodyParts: " + bodyParts.Count + "!");
        */
        /*
        for (int i = 0; i < beginSize - 1; i++)
        {
            lineDist = lineDist + minDistance;

            bodyParts[i].position = gameObject.transform.position;
            bodyParts[i].rotation = gameObject.transform.rotation;

            AddBodyPart(i, lineDist);
        }
        bodyParts.RemoveRange(0, beginSize - 1);
        bodyParts[0].gameObject.SetActive(false);
        */

    }

    void LateUpdate()
    {
        Move();

        /*
        if (Input.GetKey(KeyCode.Tab))
        {
            AddBodyPart(1, lineDist);
        }
        */
    }

    public void Move()
    {
        //bodyParts[0].Translate(bodyParts[0].forward * speed * Time.smoothDeltaTime, Space.World);


        /*
        if (Input.GetAxis("Horizontal") != 0)
            bodyParts[0].Rotate(Vector3.up * rotationSpeed * Time.deltaTime * Input.GetAxis("Horizontal"));
        */

        //bodyParts[0].position = Vector3.Slerp(bodyParts[0].position, pointer.position, 0.5f);

        //Quaternion ToRotMain = Quaternion.LookRotation(pointer.position - bodyParts[0].position);

        //bodyParts[0].rotation = Quaternion.RotateTowards(bodyParts[0].rotation, ToRotMain, rotationSpeed * Time.deltaTime);



        for (int i = 1; i < bodyParts.Count; i++)
        {
            curBodyPart = bodyParts[i];
            //PrevBodyPart = bodyParts[i - 1];
            PrevBodyPart = bodyParts[i - 1];


            if (i == 1)
            {
                PrevBodyPart = gameObject.transform;
                //bodyParts.RemoveAt(0);
            }



            curBodyPart.position = (curBodyPart.position - PrevBodyPart.transform.position).normalized * minDistance + PrevBodyPart.transform.position;//Keeps distance objects

            Quaternion  ToRot = Quaternion.LookRotation(PrevBodyPart.position - curBodyPart.position);
            Quaternion lookDir = Quaternion.RotateTowards(curBodyPart.rotation, ToRot, rotationSpeed * Time.deltaTime);
            Quaternion lookAt = Quaternion.Euler(lookDir.eulerAngles.x, lookDir.eulerAngles.y, PrevBodyPart.rotation.eulerAngles.z);

            curBodyPart.rotation = lookAt;
        }
    }

    private bool firstSpawn = false;

    public void AddBodyPart(int _count, float _lineDist)
    {
        /*
        if (firstSpawn)
        {
            Transform newpart = (Instantiate(bodyParts[1].gameObject,
    bodyParts[bodyParts.Count - 1].position = new Vector3(-minDistance + bodyParts[bodyParts.Count - 1].transform.position.x,
    bodyParts[bodyParts.Count - 1].transform.position.y,
    bodyParts[bodyParts.Count - 1].transform.position.z),
    bodyParts[bodyParts.Count - 1].rotation = gameObject.transform.rotation) as GameObject).transform;
        }
        else
        {
            Transform newpart = (Instantiate(bodyParts[1].gameObject,
    bodyParts[bodyParts.Count - 1].position = new Vector3(-minDistance + bodyParts[bodyParts.Count - 1].transform.position.x,
    bodyParts[bodyParts.Count - 1].transform.position.y,
    bodyParts[bodyParts.Count - 1].transform.position.z),
    bodyParts[bodyParts.Count - 1].rotation = gameObject.transform.rotation) as GameObject).transform;
        }

        //newpart.SetParent(transform);

        bodyParts.Add(newpart);
        */
    }
}