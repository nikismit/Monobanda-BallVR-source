using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PitchCalibrateCountDown : MonoBehaviour
{
    [SerializeField] Sprite[] countDownImages;
    [SerializeField] Image image;
    float timer = 10;

    IEnumerator HoldPitchCountdown()
    {
        image.sprite = countDownImages[2];
        yield return new WaitForSeconds(1);
        image.sprite = countDownImages[1];
        yield return new WaitForSeconds(1);
        image.sprite = countDownImages[0];
        yield return new WaitForSeconds(1);
        //Invoke("DisableSelf", 0);
        gameObject.SetActive(false);
        //return null;
    }

    private void OnEnable()
    {
        //timer = 0;
        StartCoroutine(HoldPitchCountdown());
    }
}
