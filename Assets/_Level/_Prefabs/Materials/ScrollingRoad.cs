using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingRoad : MonoBehaviour
{
    Renderer roadRender;

    float speed = 0.1f;

    void Start()
    {
        roadRender = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //float y = speed 
        roadRender.material.SetTextureOffset("_MainTex", new Vector2(0,speed * Time.deltaTime));
    }
}
