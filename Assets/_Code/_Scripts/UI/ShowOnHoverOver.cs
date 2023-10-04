using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowOnHoverOver : MonoBehaviour
{
    private CanvasGroup group;
    new private RectTransform transform;
    private float target = 0;

    void Start()
    {
        transform = (RectTransform)base.transform;
        group = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        Vector2 localMousePosition = transform.InverseTransformPoint(Input.mousePosition);
        target = transform.rect.Contains(localMousePosition)? 1 : 0;
        
        group.interactable = System.Convert.ToBoolean( Mathf.RoundToInt( group.alpha ) );

        group.alpha = Mathf.MoveTowards( group.alpha, target, 5 * Time.deltaTime );
    }

}
