using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidUISafeArea : MonoBehaviour
{
    RectTransform rectTrans;
    Rect safeArea;
    Vector2 minAnchor;
    Vector2 maxAnchor;

    private void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
        safeArea = Screen.safeArea;
        minAnchor = minAnchor + safeArea.size;

        minAnchor.x /= Screen.width;
        minAnchor.y /= Screen.height;
        maxAnchor.x /= Screen.width;
        maxAnchor.y /= Screen.height;

        rectTrans.anchorMin = minAnchor;
        rectTrans.anchorMax = maxAnchor;
    }
}
