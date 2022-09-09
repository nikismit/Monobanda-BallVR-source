using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roller : MonoBehaviour
{
    //public List<GameObject> cars;

    [SerializeField] float rollSpeed;

    void Update()
    {
        transform.Rotate(0, rollSpeed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            //cars.Add(other.gameObject);
            other.transform.parent = transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //cars.Remove(other.gameObject);
            other.transform.parent = null;
        }
    }
}
