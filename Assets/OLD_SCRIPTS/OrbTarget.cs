using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbTarget : MonoBehaviour
{
  // public AudioMovement audioMove;
  public AudioPitch_Player1 pitch;
  public float orbTarget;
  private float currentTurn;
  [Header("Options")]
	public float minimumPitch;
	public float maximumPitch;
  [Range(0f,10f)]
  public float width;




    void Start()
    {
      orbTarget = 0f;
    }

    void Update()
    {
      var currentPitch = pitch._currentpublicpitch;
  		var currentAmp = pitch._currentPublicAmplitude;
  		// var Volume = pitch._currentPublicAmplitude;
      if(currentAmp > pitch.minVolumeDB){
  			if(currentPitch > minimumPitch){
  				currentTurn = (((currentPitch-minimumPitch)/(maximumPitch-minimumPitch))*2)-1;
        }
      }

      if(currentAmp > 0f){
        orbTarget = currentTurn * width;
        // this.transform.localPosition = new Vector3 (0f, 0f, orbTarget);
        this.transform.localPosition = new Vector3 (orbTarget, 0f, 0f);
      }
      else{
        this.transform.localPosition = new Vector3 (0f, 0f, 0f);
      }
    }

}
