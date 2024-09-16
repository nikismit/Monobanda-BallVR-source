using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MicSelectOptionControl : MonoBehaviour
{
  // public Text TextBox;
  public Dropdown _mDropdown;
    // Start is called before the first frame update
    void Start()
    {
      _mDropdown.options.Clear();
      List<string> items =  new List<string>();
      items.Add(Microphone.devices[0].ToString());
      items.Add(Microphone.devices[1].ToString());
      items.Add(Microphone.devices[2].ToString());
      items.Add(Microphone.devices[3].ToString());
      items.Add(Microphone.devices[4].ToString());
      // items.Add(Microphone.devices[5].ToString());
      foreach(var item in items){
        _mDropdown.options.Add(new Dropdown.OptionData() {text = item});
      }
    }


    // void Update()
    // {
    //
    // }
}
