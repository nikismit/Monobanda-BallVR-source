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

    // Start is called before the first frame update
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

        Slider.value = Mathf.RoundToInt(player.minimumAmp);

        soundText.text = "Minimum soundInput: " + Mathf.RoundToInt(player.minimumAmp).ToString();
    }

    public void OnSliderChange(float sliderVal)
    {
        soundText.text = "Minimum soundInput: " + Mathf.RoundToInt(sliderVal).ToString();
        player.minimumAmp = Mathf.RoundToInt(sliderVal);
    }
}
