using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserObstacle : MonoBehaviour
{
    //LineRenderer laser;

    [Header("Dont touch")]
    [SerializeField] private Transform[] points;
    [SerializeField] private GameObject laser;

    [Header("General options")]
    [SerializeField] private float laserLength = 1.5f;
    [SerializeField] private float rotationSpeed;

    //[SerializeField] MeshCollider col;

    void Start()
    {
        //points = gameObject.GetComponentsInChildren<Transform>();
        //laser = gameObject.GetComponent<LineRenderer>();
        //laser.positionCount = 2;
        //Invoke("GenCollider",0.1f);
        //GenCollider();
        points[0].position += new Vector3(0, 0, laserLength);
        points[1].position -= new Vector3(0, 0, laserLength);


        float dist = Vector3.Distance(points[0].position, points[1].position);

        //laser.transform.position = new Vector3(dist / 2, dist / 2, dist / 2);

        laser.transform.localScale = new Vector3(laser.transform.localScale.x, laser.transform.localScale.y,dist);
    }


    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }

    /*
    void GenCollider()
    {

        laser.SetPosition(0, points[1].position);
        laser.SetPosition(1, points[2].position);
        col = gameObject.GetComponent<MeshCollider>();

        
        if (col == null)
        {
            col = gameObject.AddComponent<MeshCollider>();
        }
        
        Mesh mesh = new Mesh();
        laser.BakeMesh(mesh);
        col.sharedMesh = mesh;
    }
    */
}
