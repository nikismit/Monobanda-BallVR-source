using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropShadow : MonoBehaviour
{
    private Transform playerTrans;
    private Projector proj;

    private GameObject playerObj;

    private RaycastHit hit;

    void Start()
    {
        proj = gameObject.GetComponent<Projector>();
        playerTrans = transform.parent;

        playerObj = transform.parent.gameObject;
        transform.parent = null;

        Invoke("CheckPlayer", 0.1f);
    }

    void CheckPlayer()
    {
        if (playerTrans == null || !playerObj.activeSelf)
            gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        transform.position = playerTrans.position;
    }
}
