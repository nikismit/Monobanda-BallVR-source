using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinimumSound : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI soundText;
    [SerializeField] Slider Slider;
    AudioMovement player;

    void Start()
    {
        GameObject playerRef = GameObject.FindGameObjectWithTag("Player");
        player = playerRef.GetComponent<AudioMovement>();
    }

    private void OnEnable()
    {
        if(player == null)
        {
            GameObject playerRef = GameObject.FindGameObjectWithTag("Player");
            player = playerRef.GetComponent<AudioMovement>();
        }

        if (PlayerPrefs.GetFloat("minimumSound", 0) != 0)
        {
            player.minimumAmp = PlayerPrefs.GetFloat("minimumSound", 0);
        }

        Slider.value = Mathf.RoundToInt(player.minimumAmp);
        soundText.text = "Minimum soundInput: " + Mathf.RoundToInt(player.minimumAmp).ToString();
    }

    public void OnSliderChange(float sliderVal)
    {
        soundText.text = "Minimum soundInput: " + Mathf.RoundToInt(sliderVal).ToString();
        player.minimumAmp = Mathf.RoundToInt(sliderVal);
        PlayerPrefs.SetFloat("minimumSound", player.minimumAmp);
    }
}
