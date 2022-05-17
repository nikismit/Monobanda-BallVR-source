using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeting : MonoBehaviour
{
  [Range(0f,1f)]
  public float orbSpeed;
  public GameObject targ;

    void FixedUpdate()
    {
    transform.position = Vector3.MoveTowards(transform.position, targ.transform.position, orbSpeed);
    }
}
