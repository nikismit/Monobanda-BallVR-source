using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMovement2 : MonoBehaviour {

	public AnimationCurve curve;

	public AudioPitch pitch;
	//public Transform child;
	int currentPitch;
	float currentAmp;
	[Header("Movement Speeds")]
	public float maximumForwardSpeed;
	public float forwardAccelaration;
	public float forwardDeceleration;

	public float turningSpeed;
	public float turningDeceleration = 1;
	private float turningSpeedSave;

	public float currentTurn;
	public float currentSpeed;
	private float startMaxSpeed;
	private float startAccel;
	private float startDecel;
	public bool hasStopped;
	
	[Header("Options")]
	public float minimumPitch;
	public float maximumPitch;
	public bool highPitchIsTurnRight;
	public bool amplitudeControlsSpeed;
	public bool soundTriggersParticles;
	public bool mouseIsSteering;
	public bool walkingIsTrue;
	public float SteeringSpeed;
	private float SteeringSpeedSave;

	public GameObject car;
	public float xAngle, yAngle;
	public float angleOfFlight;
	public float properRotation;
	public float prevValue;

	public float FallingRate;
	private float SavedFallingRate;
	public Camera camera;

	//Make sure you attach a Rigidbody in the Inspector of this GameObject
    Rigidbody m_Rigidbody;
	//ParticleSystem _partSys;

	public float FOV;
	private float SavedFOV;

	[Header("Animation")]
	public Animator m_Animator;

    void Start()
    {
        //Fetch the Rigidbody from the GameObject with this script attached
        m_Rigidbody = GetComponent<Rigidbody>();
		//_partSys = GetComponent<ParticleSystem>();

		startMaxSpeed = maximumForwardSpeed;
		startAccel = forwardAccelaration;
		startDecel = forwardDeceleration;

		turningSpeedSave = turningSpeed;
		SteeringSpeedSave = SteeringSpeed;
		SavedFallingRate = 0f;
		FOV = 60.0f;
		SavedFOV = FOV;
    }


	private void Turning(){
		xAngle = turningSpeed * currentTurn * Time.fixedDeltaTime;
		car.transform.Rotate(-xAngle, 0, yAngle, Space.Self);
	}
	private void NoTurning (){
		xAngle = 0;
		car.transform.Rotate(xAngle, -yAngle, 0, Space.Self);
	}

	private void Buffer(){
		if ( properRotation >= 270 && properRotation <= 360 - angleOfFlight){
			turningSpeed = 0;
			hasStopped = true;			
		}
		else if (properRotation >= angleOfFlight && properRotation <= 90){
			turningSpeed = 0;
			hasStopped = true;
		}
		if (prevValue < currentTurn && hasStopped == true){
			turningSpeed = turningSpeedSave;
			hasStopped = false;
		}
		if (prevValue > currentTurn && hasStopped == true){
			turningSpeed = turningSpeedSave;
			hasStopped = false;
		}
	}

	private void Falling(){
		car.transform.Translate(-transform.up * SavedFallingRate * Time.fixedDeltaTime, Space.World);
	}

	private void Steering(){
		Vector3 mousePos = Input.mousePosition;
		{
			var normalised = (mousePos.x / Screen.width) * 2 - 1;
			Debug.Log (normalised);
			yAngle = - normalised * SteeringSpeed;
		}
	}

	private void SteerWithSpeed(){
		if (currentSpeed <= 0f) {
			SteeringSpeed = 0f;
		}
		else if (currentSpeed > 0f) {
			SteeringSpeed = SteeringSpeedSave;
		}
	}


    void FixedUpdate()
    {
		currentPitch = pitch._currentpublicpitch;
		currentAmp = pitch._currentPublicAmplitude;
		//var emission = _partSys.emission;

		if(mouseIsSteering == true){
			Steering ();
			SteerWithSpeed ();
		}

		properRotation = car.transform.eulerAngles.x;
		
			// IMPORTANT LINE OF CODE   V
		if(currentPitch > minimumPitch){ // I would propose to limit the this if statemnet to include && currentPitch < maximumPitch.
			currentTurn = (((currentPitch-minimumPitch)/(maximumPitch-minimumPitch))*2)-1; //this normalises the pitch input to a value between -1 and 1.
			// IMPORTANT LINE OF CODE   ^
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
				forwardDeceleration = startDecel;
			}
			//if(soundTriggersParticles == true){

				//emission.rateOverTime = 10;
			//} else {
				//emission.rateOverTime = 0;
			//}
			if(currentSpeed < maximumForwardSpeed){
				currentSpeed += forwardAccelaration *Time.fixedDeltaTime;
			} else {
				currentSpeed -= forwardDeceleration *Time.fixedDeltaTime;
			}
			this.transform.Translate(transform.forward * currentSpeed * Time.fixedDeltaTime, Space.World);
			if (walkingIsTrue == true){
				NoTurning();
				m_Animator.SetBool("Walking", true);
			}
			else {
				Buffer();
				Turning();
			}
			SavedFallingRate = FallingRate;
			if (FOV >= 60f && FOV < 80f) {
				SavedFOV = FOV;
				SavedFOV += 1f;
				if (SavedFOV >= 61f && SavedFOV <= 79f){
					FOV += 1f;
					Camera.main.fieldOfView = FOV ;
				}

			}
			prevValue = currentTurn;

		} else if(currentSpeed >= 0) {
			currentSpeed -= forwardDeceleration *Time.fixedDeltaTime;
			if(currentTurn > 0){
				currentTurn -= Time.fixedDeltaTime *turningDeceleration;
			}else if(currentTurn < 0){
				currentTurn += Time.fixedDeltaTime *turningDeceleration;
			}
			//emission.rateOverTime = 0;
			this.transform.Translate(transform.forward * currentSpeed * Time.fixedDeltaTime, Space.World);

			if (walkingIsTrue == true) {
				NoTurning();
				m_Animator.SetBool("Walking", false);
			}
			else {
				Falling ();
				Buffer();
				Turning();
			}

			if (FOV > 60f && FOV <= 80f) {
				SavedFOV = FOV;
				SavedFOV -= 0.1f;
				if (SavedFOV >= 61f && SavedFOV <= 79f){
					FOV -= 0.1f;
					Camera.main.fieldOfView = FOV ;
				}
			}

			prevValue = currentTurn;
		}
    }
}
