using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMovement : MonoBehaviour {

	public AudioPitch pitch;

	int currentPitch;
	public float currentAmp;
	[Header("Movement Speeds")]
	[Range(0f, 50f)]
	public float maximumForwardSpeed; // Default 10
	[Range(0f, 50f)]
	public float forwardAccelaration; // Default 5
	[Range(0f, 50f)]
	public float forwardDecelerationSinglePlayer;
	[Range(0f, 50f)]
	public float forwardDecelerationMultiplayerPlayer;

	private float forwardDeceleration; // Default 2
	[Range(0f, 200f)]
	public float turningSpeed;
	[Range(0f, 50f)]
	public float turningDeceleration; // Default 1

	[Header("Diagnostics")]
	public float currentTurn;
	public float currentSpeed;
	private float startMaxSpeed;
	private float startAccel;

	public float objectHeight;
	public float Volume;
	public Camera camera;
	public float FOV;
	private float SavedFOV;

	[Header("Options")]
	public float minimumPitch;
	public float maximumPitch;
	public int currentPitchValue;
	public bool highPitchIsTurnRight;
	public bool soundTriggersParticles;
	public bool isSinglePlayer;



	//Make sure you attach a Rigidbody in the Inspector of this GameObject
    Rigidbody m_Rigidbody;
	ParticleSystem _partSys;

    void Start()
    {
        //Fetch the Rigidbody from the GameObject with this script attached
        m_Rigidbody = GetComponent<Rigidbody>();
		_partSys = GetComponent<ParticleSystem>();
		startMaxSpeed = maximumForwardSpeed;
		startAccel = forwardAccelaration;
		FOV = 60.0f;
		SavedFOV = FOV;
    }

		void OnCollisionEnter(Collision collision)
{
		//currentSpeed = -0.5f * currentSpeed;
		//currentSpeed = 0f;
		if (collision.gameObject.tag == "Box"){
			currentSpeed = currentSpeed;
		}
		else{
			currentSpeed = 0.6f * currentSpeed;
		}
}

    void FixedUpdate()
    {
		currentPitch = pitch._currentpublicpitch;
		currentAmp = pitch._currentPublicAmplitude;
		Volume = pitch._currentPublicAmplitude;
		currentPitchValue = currentPitch;
		objectHeight = this.transform.position.y;
		if (objectHeight > 0.6f){
			this.transform.Translate(0,-0.1f,0);
		}
		var emission = _partSys.emission;

		if(currentPitch > minimumPitch){
			currentTurn = (((currentPitch-minimumPitch)/(maximumPitch-minimumPitch))*2)-1;
			if(highPitchIsTurnRight == false){
				currentTurn *= -1;
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
			if (isSinglePlayer == true) {
				forwardDeceleration = forwardDecelerationSinglePlayer;
			} else {
				forwardDeceleration = forwardDecelerationMultiplayerPlayer;
			}
			this.transform.Translate(transform.forward * currentSpeed * Time.fixedDeltaTime, Space.World);
			this.transform.Rotate(Vector3.up * turningSpeed * currentTurn * Time.fixedDeltaTime, Space.World);
			if (FOV >= 60f && FOV < 80f) {
				SavedFOV = FOV;
				SavedFOV += 1f;
				if (SavedFOV >= 61f && SavedFOV <= 79f){
					FOV += 1f;
					Camera.main.fieldOfView = FOV ;
				}

			}
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
			if (FOV > 60f && FOV <= 80f) {
				SavedFOV = FOV;
				SavedFOV -= 0.1f;
				if (SavedFOV >= 61f && SavedFOV <= 79f){
					FOV -= 0.1f;
					Camera.main.fieldOfView = FOV ;
				}
			}
		}

		//this.transform.position = new Vector3(child.position.x, 0, child.position.z);

    }


}
