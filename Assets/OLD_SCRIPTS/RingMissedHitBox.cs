using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingMissedHitBox : MonoBehaviour
{
    [SerializeField] AudioMovement player1;

    Collider col;

    private bool hasEntered = false;

    void Start()
    {
        col = gameObject.GetComponent<Collider>();
    }

    public void PlayerHasEntered()
    {
        hasEntered = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ring")
        {
            if (!hasEntered)
            {
                if (player1 != null)
                    player1.RemoveRing();
            }
            else
                hasEntered = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Physics.IgnoreCollision(collision.collider, col, true);
    }
}
