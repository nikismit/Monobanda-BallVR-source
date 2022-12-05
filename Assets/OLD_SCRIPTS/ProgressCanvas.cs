using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressCanvas : MonoBehaviour
{
    [SerializeField] Transform followPlayer;

    // Update is called once per frame
    void LateUpdate()
    {
        //Debug.Log("FollowPlayer");
        transform.position = new Vector3(transform.position.x, transform.position.y, followPlayer.position.z);
        //transform.Translate((transform.right * followPlayer.transform.position.x) * Time.fixedDeltaTime, Space.World);
    }
}
