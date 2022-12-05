using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePopup : MonoBehaviour
{
    [HideInInspector] public int player;
    //[SerializeField] Vector3 TargetPos;
    [SerializeField] AnimationCurve curve;

    void OnEnable()
    {
        StartCoroutine(PopUp(player));
    }
    
    
    IEnumerator PopUp(int player)
    {
        float playerDir = 0;

        if (player == 0)
            playerDir = -5;
        else
            playerDir = 5;

        float elapsedTime = 0;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime;
            //playerTrans.position = new Vector3(pipeEnter.position.x + elapsedTime * 30, pipeEnter.position.y, pipeEnter.position.z);
            transform.Translate(transform.up * curve.Evaluate(elapsedTime) + transform.right * playerDir * Time.deltaTime, Space.World);
            yield return null;

        }

        //yield return new WaitForSeconds(1);
    }
}
