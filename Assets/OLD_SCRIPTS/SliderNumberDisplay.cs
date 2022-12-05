using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SliderNumberDisplay : MonoBehaviour
{

  public Slider slider;
  public Text display;
  public GameObject parent;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      slider.value = Mathf.RoundToInt(slider.value);
      display.text = (slider.value).ToString();
      if (parent.name == "Player1Highest"){
        Debug.Log("Player1Highest");
        PlayerPrefs.SetFloat("Player1Highest",slider.value);
      }
      else if (parent.name == "Player1Lowest"){
        Debug.Log("Player1Lowest");
        PlayerPrefs.SetFloat("Player1Lowest",slider.value);
      }
      else if (parent.name == "Player2Highest"){
        Debug.Log("Player2Highest");
        PlayerPrefs.SetFloat("Player2Highest",slider.value);
      }
      else if (parent.name == "Player2Lowest"){
        Debug.Log("Player2Lowest");
        PlayerPrefs.SetFloat("Player2Lowest",slider.value);
      }
    }
}
