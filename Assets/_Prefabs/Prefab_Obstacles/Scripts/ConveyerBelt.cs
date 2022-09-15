using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerBelt : MonoBehaviour
{
    [SerializeField] float speed;
    private Vector3 dir = Vector3.right;

    private List<GameObject> onBelt = new List<GameObject>();

    void LateUpdate()
    {
        if (onBelt.Count == 0)
            return;

        for (int i = 0; i < onBelt.Count; i++)
        {
            onBelt[i].GetComponent<Rigidbody>().velocity = speed * gameObject.transform.forward * Time.deltaTime;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            onBelt.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            onBelt.Remove(other.gameObject);
    }
}
