using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelEffects : MonoBehaviour
{
    [SerializeField] Transform playerModel;
    Transform emptyTrans;
    private Vector3 playerScaleRef;
    [HideInInspector] public float dir;

    [SerializeField] AnimationCurve curve;

    // Start is called before the first frame update
    void Start()
    {
        emptyTrans = GetComponent<Transform>();
        playerScaleRef = emptyTrans.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rotateVec = new Vector3(dir, 0, 0).normalized;

        float rotateAmount = dir * 12;
        Quaternion rotationZ = Quaternion.AngleAxis(-rotateAmount, Vector3.forward);
        playerModel.localRotation = Quaternion.Slerp(playerModel.localRotation, rotationZ, 10 * Time.deltaTime);
    }

    bool floating = false;
    float floatAmount = 0.5f;

    private void LateUpdate()
    {
        /*
        if (!floating)
        {
            emptyTrans.localPosition = new Vector3(0, Time.deltaTime * -floatAmount, 0);

            if (emptyTrans.localPosition.y <= -1f)
            {
                Debug.Log("UP");
                floating = true;
            }
        }

        if (floating)
        {
            emptyTrans.localPosition = new Vector3(0, Time.deltaTime * floatAmount, 0);

            if (emptyTrans.localPosition.y >= 1f)
            {
                Debug.Log("Down");
                floating = false;
            }
        }
        */
    }

    public void RotateModel(float dirInput)
    {
        /*
        dir = dirInput;

        Vector3 rotateVec = new Vector3(dirInput, 0, 0).normalized;
        //Debug.Log("Rotation = " + playerModel.rotation.eulerAngles.z);

        if (playerModel.rotation.eulerAngles.z < 50 && dirInput <= 0)
            playerModel.Rotate(Vector3.forward, -rotateVec.x * 150 * Time.deltaTime);

        
        if (playerModel.rotation.eulerAngles.z > 300 && playerModel.rotation.eulerAngles.z < 50  && dirInput >= 0)
            playerModel.Rotate(Vector3.forward, -rotateVec.x * 150 * Time.deltaTime);
        */

        Vector3 rotateVec = new Vector3(dirInput, 0, 0).normalized;

        float rotateAmount = rotateVec.x * 2;

        Quaternion rotationZ = Quaternion.AngleAxis(rotateAmount, Vector3.forward);

        playerModel.localRotation = Quaternion.Slerp(transform.localRotation, rotationZ, 20 * Time.deltaTime);

    }

    public void Squash(Vector3 scaleMultiplier)
    {
        StopAllCoroutines();
        StartCoroutine(SquashStretch(scaleMultiplier));
    }

    IEnumerator SquashStretch(Vector3 scaleMultiplier)
    {
        float elapsedTime = 0;

        while (elapsedTime < 1)
        {
            elapsedTime += Time.deltaTime;

            //float ease = Mathf.Clamp(elapsedTime, 0, 1);
            float ease = Mathf.Clamp(elapsedTime, 0, 1);
            Vector3 easeVector = Vector3.Lerp(playerScaleRef, scaleMultiplier, curve.Evaluate(elapsedTime));

            //playerModel.localScale = new Vector3(curve.Evaluate(ease), curve.Evaluate(ease), curve.Evaluate(ease));
            emptyTrans.localScale = easeVector;

            //Time.timeScale = curve.Evaluate(ease);

            //highScoreUI.alpha = elapsedTime;

            yield return null;

        }
        emptyTrans.localScale = playerScaleRef;
    }
}
