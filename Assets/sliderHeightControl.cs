using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class sliderHeightControl : MonoBehaviour
{
  public AudioPitch_MenuPlayer2 pitch;
  public Slider slider;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      slider.value = pitch._currentpublicpitch;
    }
}
