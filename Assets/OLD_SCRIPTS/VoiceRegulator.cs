using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceRegulator : MonoBehaviour
{
    [SerializeField] GameObject calibrationMenu;

    // Start is called before the first frame update
    void Start()
    {
        calibrationMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !calibrationMenu.activeSelf)
            calibrationMenu.SetActive(true);
        else if (Input.GetKeyDown(KeyCode.P) && calibrationMenu.activeSelf)
            calibrationMenu.SetActive(false);
    }
}
