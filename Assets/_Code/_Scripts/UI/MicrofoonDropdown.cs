using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;
using System;

public class MicrofoonDropdown : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown dropdown;

    [SerializeField]
    private AudioPitch_Player1 player;

    [SerializeField]
    private AudioPitch_Player1 otherPlayer;

    private int oldIndex = 0;

    void OnValidate()
    {
        dropdown = GetComponent<TMP_Dropdown>();
    }

    // Start is called before the first frame update
    void Start()
    {
        dropdown.enabled = false;

        dropdown.ClearOptions();
        foreach( string micName in Microphone.devices )
        {
            dropdown.options.Add( new TMP_Dropdown.OptionData( micName ) );
        }  
        
        dropdown.enabled = true;
        dropdown.SetValueWithoutNotify( player.MicInput );

        dropdown.onValueChanged.AddListener( onMicrofoonSelected );
    }

    private void onMicrofoonSelected(int arg0)
    {
        string mic = Microphone.devices[ arg0 ];
        if( otherPlayer.selectedDevice == mic )
        {
            dropdown.SetValueWithoutNotify( oldIndex );
            return;
        }

        oldIndex = arg0;
        player.SwitchMicrofoon( mic );
    }

}
