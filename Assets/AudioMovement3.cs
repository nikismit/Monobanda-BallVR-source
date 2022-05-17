using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMovement3 : MonoBehaviour {

	public AnimationCurve curve;

	public AudioPitch pitch;

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


    Rigidbody m_Rigidbody;


	public float FOV;
	private float SavedFOV;
	private bool On = false;
	public float xRot = 0f;
	public bool turning;

    void Start()
    {

        m_Rigidbody = GetComponent<Rigidbody>();

		startMaxSpeed = maximumForwardSpeed;
		startAccel = forwardAccelaration;
		startDecel = forwardDeceleration;

		turningSpeedSave = turningSpeed;
		SteeringSpeedSave = SteeringSpeed;
		SavedFallingRate = 0f;
		FOV = 60.0f;
		SavedFOV = FOV;
		turning = false;
    }


	private void Turning(){
		xAngle = turningSpeed * currentTurn * Time.fixedDeltaTime;

		car.transform.Rotate (-xAngle, 0, 0, Space.Self);
		xRot = transform.localEulerAngles.x;
		if (On == false){
			//if(xRot>0f){
			//	xRot -= 5f;
			//}
			//else if(xRot<0f){
			//	xRot += 5f;
			//}
		}
		car.transform.Rotate (0, -yAngle,0, Space.World);
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


		if(mouseIsSteering == true){
			if(Input.GetMouseButton(0)){
				Steering();
				turning = true;
			}
			else{
				// yAngle = 0f;
				if (yAngle > 0.1f){
					yAngle -= 0.1f;
				}
				else if (yAngle < -0.1f){
					yAngle += 0.1f;
				}
				turning = false;
			}

		}

		properRotation = car.transform.eulerAngles.x;


		if(currentPitch > minimumPitch){
		//if(currentAmp > pitch.minVolumeDB){
			currentTurn = (((currentPitch-minimumPitch)/(maximumPitch-minimumPitch))*2)-1;

			// Bool Checks:
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

			// Movement:
			if(currentSpeed < maximumForwardSpeed){
				currentSpeed += forwardAccelaration *Time.fixedDeltaTime;
			} else {
				currentSpeed -= forwardDeceleration *Time.fixedDeltaTime;
			}
			this.transform.Translate(transform.forward * currentSpeed * Time.fixedDeltaTime, Space.World);
				Buffer();
				Turning();
			On = true;

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
			this.transform.Translate(transform.forward * currentSpeed * Time.fixedDeltaTime, Space.World);
				Falling ();
				Buffer();
				Turning();

			if (FOV > 60f && FOV <= 80f) {
				SavedFOV = FOV;
				SavedFOV -= 0.05f;
				if (SavedFOV >= 61f && SavedFOV <= 79f){
					FOV -= 0.05f;
					Camera.main.fieldOfView = FOV ;
					On = false;
				}
			}
			prevValue = currentTurn;
		}
    }
}
