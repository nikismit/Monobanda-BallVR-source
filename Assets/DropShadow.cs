using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropShadow : MonoBehaviour
{
    private Transform playerTrans;
    private Projector proj;

    private RaycastHit hit;

    void Start()
    {
        proj = gameObject.GetComponent<Projector>();
        playerTrans = transform.parent;
        transform.parent = null;
    }

    void LateUpdate()
    {
        Ray ray = new Ray(playerTrans.position, Vector3.down);

        if (Physics.Raycast(ray, out hit))
        {
            float shadowSize = Mathf.Lerp(1, 0.15f, hit.distance / 10);
            proj.orthographicSize = shadowSize;
        }

        transform.position = playerTrans.position;
    }
}
