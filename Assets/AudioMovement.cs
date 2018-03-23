using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMovement : MonoBehaviour {

	public AudioPitch pitch;
	public Collider someCollider;
	//public Transform child;
	int currentPitch;
	float currentAmp;
	[Header("Movement Speeds")]
	public float maximumForwardSpeed = 10;
	public float forwardAccelaration = 5;
	public float forwardDeceleration = 2;

	public float turningSpeed;
	public float turningDeceleration = 1;

	public float currentTurn;
	public float currentSpeed;
	private float startMaxSpeed;
	private float startAccel;
	private float startDecel;
	
	[Header("Options")]
	public float minimumPitch;
	public float maximumPitch;
	public bool highPitchIsTurnRight;
	public bool amplitudeControlsSpeed;
	public bool soundTriggersParticles;
	
	private float currentHeight;
	private float previousHeight;
	public bool goingUp = false;

	//Make sure you attach a Rigidbody in the Inspector of this GameObject
    Rigidbody m_Rigidbody;
	ParticleSystem _partSys;

    void Start()
    {
        //Fetch the Rigidbody from the GameObject with this script attached
		currentHeight = this.transform.position.y;
		previousHeight = currentHeight;
        m_Rigidbody = GetComponent<Rigidbody>();
		_partSys = GetComponent<ParticleSystem>();
		startMaxSpeed = maximumForwardSpeed;
		startAccel = forwardAccelaration;
		startDecel = forwardDeceleration;
    }

    void FixedUpdate()
    {
		currentPitch = pitch._currentpublicpitch;
		currentAmp = pitch._currentPublicAmplitude;
		var emission = _partSys.emission;
		currentHeight = this.transform.position.y;
		if(currentHeight>previousHeight){
			goingUp = true;
			previousHeight = currentHeight;
		} else {
			goingUp = false;
			previousHeight = currentHeight;
		}
		RaycastHit hit;
		Ray ray = new Ray(transform.position, Vector3.down);
		if (someCollider.Raycast(ray, out hit, 10)) {
			var slope = hit.normal;
			//print(slope);
			if(goingUp == false){
				if(Mathf.Abs(hit.normal.x) > Mathf.Abs(hit.normal.z)){
					forwardDeceleration = startDecel * (1.0f - Mathf.Abs(hit.normal.x)*1.5f);
				} else {
					forwardDeceleration = startDecel * (1.0f - Mathf.Abs(hit.normal.z)*1.5f);
				}
			} else {
				if(Mathf.Abs(hit.normal.x) > Mathf.Abs(hit.normal.z)){
					forwardDeceleration = startDecel * (1.0f + Mathf.Abs(hit.normal.x));
				} else {
					forwardDeceleration = startDecel * (1.0f + Mathf.Abs(hit.normal.z));
				}
			}
			//Adjust character based on normal
		}

		if(currentPitch > minimumPitch){
			currentTurn = (((currentPitch-minimumPitch)/(maximumPitch-minimumPitch))*2)-1;
			if(highPitchIsTurnRight == false){
				currentTurn *= -1;
			}
			if(amplitudeControlsSpeed == true){
				maximumForwardSpeed = currentAmp+10;
				forwardAccelaration = (currentAmp+10)/2;
				forwardDeceleration = (currentAmp+10)/2;
			} else {
				maximumForwardSpeed = startMaxSpeed;
				forwardAccelaration = startAccel;
				//forwardDeceleration = startAccel;
			}
			if(soundTriggersParticles == true){

				emission.rateOverTime = 10;
			} else {
				emission.rateOverTime = 0;
			}
			if(currentSpeed < maximumForwardSpeed){
				currentSpeed += forwardAccelaration *Time.fixedDeltaTime;
			} else {
				currentSpeed -= forwardDeceleration *Time.fixedDeltaTime;
			}
			this.transform.Translate(transform.forward * currentSpeed * Time.fixedDeltaTime, Space.World);
			this.transform.Rotate(Vector3.up * turningSpeed * currentTurn * Time.fixedDeltaTime, Space.World);
		} else if(currentSpeed >= 0) {
			currentSpeed -= forwardDeceleration *Time.fixedDeltaTime;
			if(currentTurn > 0){
				currentTurn -= Time.fixedDeltaTime *turningDeceleration;
			}else if(currentTurn < 0){
				currentTurn += Time.fixedDeltaTime *turningDeceleration;
			}
			emission.rateOverTime = 0;
			this.transform.Translate(transform.forward * currentSpeed * Time.fixedDeltaTime, Space.World);
			this.transform.Rotate(Vector3.up * turningSpeed * currentTurn * Time.fixedDeltaTime, Space.World);
		}

		//this.transform.position = new Vector3(child.position.x, 0, child.position.z);

    }


}
