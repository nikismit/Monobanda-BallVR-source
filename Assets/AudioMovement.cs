using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMovement : MonoBehaviour {

	public AudioPitch pitch;
	//public Transform child;
	int currentPitch;
	public float torque;
	public float thrust;
	//public float minSpeed;
	public float breakSpeed = 2;
	public float turnBreakSpeed = 1;
	public float accelaration = 5;
	public float maxSpeed = 10;
	public float lowPitch;
	public float highPitch;
	public bool highIsRight;
	public float turn;


	//Make sure you attach a Rigidbody in the Inspector of this GameObject
    Rigidbody m_Rigidbody;

    void Start()
    {
        //Fetch the Rigidbody from the GameObject with this script attached
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
		currentPitch = pitch._currentpublicpitch;
		//turn = (((currentPitch-lowPitch)/(highPitch-lowPitch))*2)-1;
		if(currentPitch > lowPitch){
			turn = (((currentPitch-lowPitch)/(highPitch-lowPitch))*2)-1;
			if(thrust < maxSpeed){
				thrust += accelaration *Time.fixedDeltaTime;
			}
			this.transform.Translate(transform.forward * thrust * Time.fixedDeltaTime, Space.World);
			this.transform.Rotate(Vector3.up * torque * turn * Time.fixedDeltaTime, Space.World);
		} else if(thrust >= 0) {
			thrust -= breakSpeed *Time.fixedDeltaTime;
			if(turn > 0){
				turn -= Time.fixedDeltaTime *turnBreakSpeed;
			}else if(turn < 0){
				turn += Time.fixedDeltaTime *turnBreakSpeed;
			}
			this.transform.Translate(transform.forward * thrust * Time.fixedDeltaTime, Space.World);
			this.transform.Rotate(Vector3.up * torque * turn * Time.fixedDeltaTime, Space.World);
		}

		//this.transform.position = new Vector3(child.position.x, 0, child.position.z);

    }


}
