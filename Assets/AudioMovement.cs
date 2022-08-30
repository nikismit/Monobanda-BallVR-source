using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioMovement : MonoBehaviour {

	public collisionAdjustmentScriptPlayer1 crashForce;
	public AudioPitch_Player1 pitch;
	public SFXManager sfx;
	public int currentPitch;

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
	[Range(0f, 5f)]
	public float speedBoost;
	private float speedBoostDecelerator = 1f;

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
	public bool FovWanted;



	// public FinishScript Done;

	[Header("Options")]
	public float minimumPitch;
	public float maximumPitch;
	public int currentPitchValue;
	public bool highPitchIsTurnRight;
	public bool soundTriggersParticles;
	public bool isSinglePlayer;
	public bool comingFromMainMenu;

	[Header("Debug")]
	public bool debugKeyControl;



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

		//SetMinAndMaxPitch
		if(comingFromMainMenu == true){
			minimumPitch = PlayerPrefs.GetFloat("Player1Lowest");
			maximumPitch = PlayerPrefs.GetFloat("Player1Highest");
		}

    }

		void OnCollisionEnter(Collision collision)
{
		if (collision.gameObject.tag == "Box"){
			currentSpeed = currentSpeed;
		}
		else if(collision.gameObject.tag == "Track"){
			crashForce.onCollisionCorrection();
			sfx.crashIntoTrack();
			if(currentSpeed> maximumForwardSpeed){
					currentSpeed = 0.50f * currentSpeed;
			}
			else if (currentSpeed< 0.75f*maximumForwardSpeed){
				currentSpeed = 0.80f * currentSpeed;
			}
			else{
				currentSpeed = 0.70f * currentSpeed;
			}
		}
		else{
			currentSpeed = 0.75f * currentSpeed;
		}
}
	private void OnTriggerEnter(Collider other)
    {
			if (other.gameObject.tag == "Ring"){
			transform.rotation = other.transform.rotation;
			currentSpeed = speedBoost * currentSpeed;
			}
    }

    void FixedUpdate()
    {
			var mult = speedBoost * maximumForwardSpeed;
			var diffCoef = (currentSpeed/((maximumForwardSpeed*speedBoost)-maximumForwardSpeed))*mult;
			if (currentSpeed > maximumForwardSpeed+1f){
				speedBoostDecelerator = diffCoef;
			}
			else{
				speedBoostDecelerator = 1f;
			}
		// if (Done.theEnd == true){
		// 	turningSpeed = 0f;
		// 	currentAmp = -50f;
		// 	currentSpeed -= forwardDeceleration *Time.fixedDeltaTime;
		// 	this.transform.Translate(transform.forward * currentSpeed * Time.fixedDeltaTime, Space.World);
		// 	if (Done.timer >= 3.5f){
		// 		Scene scene = SceneManager.GetActiveScene();
		// 		SceneManager.LoadScene(scene.name);
		// 		Debug.Log("Reloading Scene");
		// 	}
		// }
		//else{
		currentPitch = pitch._currentpublicpitch;
		currentAmp = pitch._currentPublicAmplitude;
		Volume = pitch._currentPublicAmplitude;
		currentPitchValue = currentPitch;
		objectHeight = this.transform.position.y;
		if (objectHeight > 0.6f){
			this.transform.Translate(0,-0.1f,0);
		}


		if (!debugKeyControl)
			CarSoundMovement();
		else
			CarKeyMovement();
		// if(currentPitch > minimumPitch && currentAmp > -15f){
		// 		currentTurn = (((currentPitch-minimumPitch)/(maximumPitch-minimumPitch))*2)-1;


	}

	void CarKeyMovement()
    {
		float hor = Input.GetAxisRaw("Horizontal");
		float ver = Input.GetAxisRaw("Vertical");

		var emission = _partSys.emission;

		if (hor != 0 || ver > 0)
		{
			//currentTurn = (((currentPitch - minimumPitch) / (maximumPitch - minimumPitch)) * 2) - 1 - 0.3f;
			currentTurn = hor;


			if (soundTriggersParticles == true)
			{

				emission.rateOverTime = 10;
			}
			else
			{
				emission.rateOverTime = 0;
			}
			if (currentSpeed < maximumForwardSpeed)
			{
				currentSpeed += forwardAccelaration * Time.fixedDeltaTime;
			}
			else
			{
				currentSpeed -= forwardDeceleration * speedBoostDecelerator * Time.fixedDeltaTime;
			}
			if (isSinglePlayer == true)
			{
				forwardDeceleration = forwardDecelerationSinglePlayer;
			}
			else
			{
				forwardDeceleration = forwardDecelerationMultiplayerPlayer;
			}
			this.transform.Translate(transform.forward * currentSpeed * Time.fixedDeltaTime, Space.World);
			this.transform.Rotate(Vector3.up * turningSpeed * currentTurn * Time.fixedDeltaTime, Space.World);
			if (FovWanted == true)
			{
				if (FOV >= 60f && FOV < 80f)
				{
					SavedFOV = FOV;
					SavedFOV += 1f;
					if (SavedFOV >= 61f && SavedFOV <= 79f)
					{
						FOV += 1f;
						Camera.main.fieldOfView = FOV;
					}

				}
			}
		}
		else if (currentSpeed >= 0 && hor == 0 && ver < 1)
		{
			currentSpeed -= forwardDeceleration * speedBoostDecelerator * Time.fixedDeltaTime;
			if (currentTurn > 0)
			{
				currentTurn -= Time.fixedDeltaTime * turningDeceleration;
			}
			else if (currentTurn < 0)
			{
				currentTurn += Time.fixedDeltaTime * turningDeceleration;
			}
			emission.rateOverTime = 0;
			this.transform.Translate(transform.forward * currentSpeed * Time.fixedDeltaTime, Space.World);
			this.transform.Rotate(Vector3.up * turningSpeed * currentTurn * Time.fixedDeltaTime, Space.World);
			if (FovWanted == true)
			{
				if (FOV > 60f && FOV <= 80f)
				{
					SavedFOV = FOV;
					SavedFOV -= 0.1f;
					if (SavedFOV >= 61f && SavedFOV <= 79f)
					{
						FOV -= 0.1f;
						Camera.main.fieldOfView = FOV;
					}
				}
			}
		}
	}

	void CarSoundMovement()
    {
		var emission = _partSys.emission;

		if (currentAmp > pitch.minVolumeDB)
		{
			if (currentPitch > minimumPitch)
			{
				currentTurn = (((currentPitch - minimumPitch) / (maximumPitch - minimumPitch)) * 2) - 1 - 0.3f;
			}
			if (highPitchIsTurnRight == false)
			{
				currentTurn *= -1;
			}
			if (soundTriggersParticles == true)
			{

				emission.rateOverTime = 10;
			}
			else
			{
				emission.rateOverTime = 0;
			}
			if (currentSpeed < maximumForwardSpeed)
			{
				currentSpeed += forwardAccelaration * Time.fixedDeltaTime;
			}
			else
			{
				currentSpeed -= forwardDeceleration * speedBoostDecelerator * Time.fixedDeltaTime;
			}
			if (isSinglePlayer == true)
			{
				forwardDeceleration = forwardDecelerationSinglePlayer;
			}
			else
			{
				forwardDeceleration = forwardDecelerationMultiplayerPlayer;
			}
			this.transform.Translate(transform.forward * currentSpeed * Time.fixedDeltaTime, Space.World);
			this.transform.Rotate(Vector3.up * turningSpeed * currentTurn * Time.fixedDeltaTime, Space.World);
			if (FovWanted == true)
			{
				if (FOV >= 60f && FOV < 80f)
				{
					SavedFOV = FOV;
					SavedFOV += 1f;
					if (SavedFOV >= 61f && SavedFOV <= 79f)
					{
						FOV += 1f;
						Camera.main.fieldOfView = FOV;
					}

				}
			}
		}
		else if (currentSpeed >= 0 && currentAmp <= pitch.minVolumeDB)
		{
			currentSpeed -= forwardDeceleration * speedBoostDecelerator * Time.fixedDeltaTime;
			if (currentTurn > 0)
			{
				currentTurn -= Time.fixedDeltaTime * turningDeceleration;
			}
			else if (currentTurn < 0)
			{
				currentTurn += Time.fixedDeltaTime * turningDeceleration;
			}
			emission.rateOverTime = 0;
			this.transform.Translate(transform.forward * currentSpeed * Time.fixedDeltaTime, Space.World);
			this.transform.Rotate(Vector3.up * turningSpeed * currentTurn * Time.fixedDeltaTime, Space.World);
			if (FovWanted == true)
			{
				if (FOV > 60f && FOV <= 80f)
				{
					SavedFOV = FOV;
					SavedFOV -= 0.1f;
					if (SavedFOV >= 61f && SavedFOV <= 79f)
					{
						FOV -= 0.1f;
						Camera.main.fieldOfView = FOV;
					}
				}
			}
		}
	}


}
