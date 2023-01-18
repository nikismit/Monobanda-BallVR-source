using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingText : MonoBehaviour
{
    private TextMeshProUGUI loadingText;

    private float elapsedTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        loadingText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.unscaledTime;

        if (elapsedTime < 1)
            loadingText.text = "Loading";
        else if (elapsedTime < 2)
            loadingText.text = "Loading.";
        else if (elapsedTime < 3)
            loadingText.text = "Loading..";
        else if (elapsedTime < 4)
            loadingText.text = "Loading...";
        else if (elapsedTime < 4)
            elapsedTime = 0;
    }
}
