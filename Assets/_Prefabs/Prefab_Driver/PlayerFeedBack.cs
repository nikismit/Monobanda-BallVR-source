using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFeedBack : MonoBehaviour
{
    public GameObject playerModel;
    private float time;
    private float hitDuration = 10;
    private float shakeStrength = 0.5f;

     public ParticleSystem hitParticle;

    private Material mat;
    private MeshRenderer meshRender;
    private bool blinkFlipFlop = false;

    // Start is called before the first frame update
    void Start()
    {
        time = hitDuration;
        mat = playerModel.GetComponent<Renderer>().material;
        meshRender = playerModel.GetComponent<MeshRenderer>();

        /*mat.SetFloat("_Mode", 4f);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;*/
    }

    public void HitFeedBack()
    {
        time = 0;
        hitParticle.Play();
    }

    void Update()
    {
        if (time < hitDuration)
        {
            time++;
            Vector3 shakePos = new Vector3(Random.Range(-shakeStrength, shakeStrength), Random.Range(-shakeStrength, shakeStrength), Random.Range(-shakeStrength, shakeStrength));
            playerModel.transform.localPosition = shakePos;
        }
        else
            playerModel.transform.localPosition = Vector3.zero;



    }

    public void BlinkFeedBack(bool _enableBlink)
    {
        if (_enableBlink)
        {
            if (!blinkFlipFlop)
            {
                blinkFlipFlop = true;
                meshRender.enabled = false;
            }
            else
            {
                blinkFlipFlop = false;
                meshRender.enabled = true;
            }
        }
        else
        {
            blinkFlipFlop = false;
            meshRender.enabled = true;
        }


    }
}
