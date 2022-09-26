using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioMovementPlayer2 : MonoBehaviour {

	public collisionAdjustmentScriptPlayer2 crashForce;
	private SnakeBehavior carLine;
	public AudioPitch_Player2 pitch;
	public SFXManager sfx;
	public Text ringCount;
	public int currentPitch;
	public RingMissedHitBox ringMissed;

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

	public FinishScript Done;

	[Header("Options")]
	public float minimumPitch;
	public float maximumPitch;
	public int currentPitchValue;
	public bool highPitchIsTurnRight;
	public bool soundTriggersParticles;
	public bool isSinglePlayer;
	public bool comingFromMainMenu;

	[Header("EndlessRunner")]
	[SerializeField] bool endlessRunner;

	private bool setCrashCol;
	private bool lockRigidbodyRotation;

	//[Header("Debug CarRail movement")]
	private float roadWidth = 60;
	private float roadHalf;
	private float railSpeed;
	float sliderPitchInvLerp;

	private Vector3 sliderVector;
	private float railSteerSpeed;
	private float railSteerRef;
	Vector3 sliderPos;
	private Vector3 velocity = Vector3.zero;
	private float lastValidPitch;

	private bool hasStarted = false;
	private bool testRailControl;
	private bool PitchSliderMovement;
	private bool ignorePlayerCol;
	private Collider col;
	private Transform camDist;
	private float boostTimer = 100;

	private AudioMovement playerOne;

	private int numRings = 5;
	public Sprite[] ringcountUIArray;
	public Image ringcountUI;

	//Make sure you attach a Rigidbody in the Inspector of this GameObject
	Rigidbody m_Rigidbody;
	ParticleSystem _partSys;

    void Start()
    {
		carLine = gameObject.GetComponent<SnakeBehavior>();

		//Fetch the Rigidbody from the GameObject with this script attached
		m_Rigidbody = GetComponent<Rigidbody>();
		_partSys = GetComponent<ParticleSystem>();
		startMaxSpeed = maximumForwardSpeed;
		startAccel = forwardAccelaration;
		FOV = 60.0f;
		SavedFOV = FOV;
		if(comingFromMainMenu == true){
			minimumPitch = PlayerPrefs.GetFloat("Player2Lowest");
			maximumPitch = PlayerPrefs.GetFloat("Player2Highest");
		}

        if (endlessRunner)
        {
			GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].GetComponent<AudioMovement>())
                {
					playerOne = players[i].GetComponent<AudioMovement>();
                }
            }
			//playerOne = GameObject.FindGameObjectsWithTag("Player").GetComponent<AudioMovement>();

			railSpeed = playerOne.railSpeed;
			roadHalf = roadWidth / 2;

			col = gameObject.GetComponent<Collider>();
			ignorePlayerCol = playerOne.ignorePlayerCol;
			testRailControl = playerOne.testRailControl;
			PitchSliderMovement = playerOne.PitchSliderMovement;
			lockRigidbodyRotation = playerOne.lockRigidbodyRotation;
			camDist = GameObject.FindGameObjectWithTag("CarDistRef").transform;
		}
    }

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			Physics.IgnoreCollision(collision.collider, col, ignorePlayerCol);
		}

		if (collision.gameObject.tag == "Box"){
		currentSpeed = currentSpeed;
	}
	else if(collision.gameObject.tag == "Track" || setCrashCol && collision.gameObject.tag == "Player" && !ignorePlayerCol)
		{
			Vector3 colReflect = Vector3.Reflect(transform.forward, collision.contacts[0].normal);

			RemoveRing();


			if (Time.time - lastHit < 2)
			{
				lastHit = Time.time;
				Physics.IgnoreCollision(collision.collider, col, true);
			}


			if (testRailControl)//TODO: only works on right wall
			{
				Vector3 reflect = Vector3.zero;

				if (currentTurn > 0)
					reflect = Vector3.Reflect(transform.right, collision.contacts[0].normal);
				else
					reflect = Vector3.Reflect(-transform.right, collision.contacts[0].normal);
				transform.Translate(reflect, Space.World);
			}
			else
				crashForce.onCollisionCorrection(colReflect);
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

	private bool canAddCar = true;

	private void OnTriggerEnter(Collider other)
    {
		if (other.gameObject.tag == "Ring"){

			ringMissed.PlayerHasEntered();

			if (canAddCar)
			{
				canAddCar = false;
				if (numRings < 5)
                {
					numRings++;
					ringcountUI.sprite = ringcountUIArray[numRings];
					//ringCount.text = numRings.ToString();
				}

				carLine.AddBodyPart(1, 0);
			}
			transform.rotation = other.transform.rotation;
			//currentSpeed = speedBoost * currentSpeed;
			//currentSpeed = 55;
			boostTimer = 0;
		}

		if (other.gameObject.GetComponent<JumpPad>())
			JumpBoost(other.gameObject.GetComponent<JumpPad>().jumpStrength);
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Ring")
			canAddCar = true;
	}

	void FixedUpdate()
    {
		if (playerOne.isMoving)
		{
			if (boostTimer < 1)
			{
				boostTimer += Time.deltaTime;

				float ease = Mathf.InverseLerp(0, 2, boostTimer) * 5;
				Debug.Log(boostTimer);
				//ringModels[0].transform.Rotate(new Vector3(0, 0, 200));
				currentSpeed += 10;

			}
			else if (transform.position.x >= camDist.position.x - 7.5f + (numRings * 5f))
			{
				currentSpeed -= 5;
				//ringModels[0].transform.Rotate(new Vector3(0, 0, 1));
			}
			else if (transform.position.x < camDist.position.x - 0.1f - 7.5f + (numRings * 5f))
			{
				currentSpeed += 5;
				//ringModels[0].transform.Rotate(new Vector3(0, 0, 1));
			}
			else
				currentSpeed = maximumForwardSpeed;
		}

		var mult = speedBoost * maximumForwardSpeed;
			var diffCoef = (currentSpeed/((maximumForwardSpeed*speedBoost)-maximumForwardSpeed))*mult;
			if (currentSpeed > maximumForwardSpeed+1f){
				speedBoostDecelerator = diffCoef;
			}
			else{
				speedBoostDecelerator = 1f;
			}

		currentPitch = pitch._currentpublicpitch;
		currentAmp = pitch._currentPublicAmplitude;
		Volume = pitch._currentPublicAmplitude;
		currentPitchValue = currentPitch;
		objectHeight = this.transform.position.y;
		if (objectHeight > 0.6f){
			this.transform.Translate(0,-0.1f,0);
		}
		var emission = _partSys.emission;

		if (currentPitch < 7)
		{
			sliderPitchInvLerp = lastValidPitch;
		}
		else
		{
			sliderPitchInvLerp = Mathf.InverseLerp(maximumPitch, minimumPitch, currentPitch);// set value from 0 to 1 (Min pitch value = 7, max pitch value = 30)
			lastValidPitch = sliderPitchInvLerp;
		}

		if (hasStarted)
			sliderVector = new Vector3(transform.position.x, transform.position.y, sliderPitchInvLerp * roadWidth - roadHalf);
		else
			sliderVector = transform.position;
		//
		sliderPos = Vector3.SmoothDamp(transform.position, sliderVector, ref velocity, 1, railSpeed * Time.deltaTime);



		// if(currentPitch > minimumPitch && currentAmp > -15f){
		// 		currentTurn = (((currentPitch-minimumPitch)/(maximumPitch-minimumPitch))*2)-1;
		if (currentAmp > pitch.minVolumeDB){
			if(currentPitch > minimumPitch){
				currentTurn = (((currentPitch-minimumPitch)/(maximumPitch-minimumPitch))*2)-1-0.3f;
			}
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
				currentSpeed -= forwardDeceleration* speedBoostDecelerator *Time.fixedDeltaTime;
			}
			if (isSinglePlayer == true) {
				forwardDeceleration = forwardDecelerationSinglePlayer;
			} else {
				forwardDeceleration = forwardDecelerationMultiplayerPlayer;
			}
			if (PitchSliderMovement)
				this.transform.position = sliderPos + transform.forward * currentSpeed * Time.fixedDeltaTime;
			else if (testRailControl)
				this.transform.Translate((transform.forward * currentSpeed + transform.right * currentTurn * railSteerSpeed) * Time.fixedDeltaTime, Space.World);
			else
			{
				this.transform.Translate(transform.forward * currentSpeed * Time.fixedDeltaTime, Space.World);
				this.transform.Rotate(Vector3.up * turningSpeed * currentTurn * Time.fixedDeltaTime, Space.World);
			}
			if (FovWanted == true){
			if (FOV >= 60f && FOV < 80f) {
				SavedFOV = FOV;
				SavedFOV += 1f;
				if (SavedFOV >= 61f && SavedFOV <= 79f){
					FOV += 1f;
					Camera.main.fieldOfView = FOV ;
				}
			}
		}
		} else if(currentSpeed >= 0 && currentAmp <= pitch.minVolumeDB) {
			currentSpeed -= forwardDeceleration* speedBoostDecelerator *Time.fixedDeltaTime;
			if(currentTurn > 0){
				currentTurn -= Time.fixedDeltaTime *turningDeceleration;
			}else if(currentTurn < 0){
				currentTurn += Time.fixedDeltaTime *turningDeceleration;
			}
			emission.rateOverTime = 0;
			if (PitchSliderMovement)
				this.transform.position = sliderPos + transform.forward * currentSpeed * Time.fixedDeltaTime;
			else if (testRailControl)
				this.transform.Translate((transform.forward * currentSpeed + transform.right * currentTurn * railSteerSpeed) * Time.fixedDeltaTime, Space.World);
			else
			{
				this.transform.Translate(transform.forward * currentSpeed * Time.fixedDeltaTime, Space.World);
				this.transform.Rotate(Vector3.up * turningSpeed * currentTurn * Time.fixedDeltaTime, Space.World);
			}
			if (FovWanted == true){
			if (FOV > 60f && FOV <= 80f) {
				SavedFOV = FOV;
				SavedFOV -= 0.1f;
				if (SavedFOV >= 61f && SavedFOV <= 79f){
					FOV -= 0.1f;
					Camera.main.fieldOfView = FOV ;
				}
			}
		}
		}



    }

	float lastHit;

	public void RemoveRing()
	{


        if (numRings > 0)
        {
            if (Time.time-lastHit < 2)
				return;

			lastHit = Time.time;
			numRings--;
			ringcountUI.sprite = ringcountUIArray[numRings];
			//ringCount.text = numRings.ToString();
			Debug.Log("Player2 REMOVERING!");
		}
        else
        {
			Debug.Log("Player2 GAME OVER!");
			Destroy(gameObject);
        }
	}


	public void JumpBoost(float jumpBoost)
	{
		//Debug.LogError("JAJA");
		m_Rigidbody.AddForce(transform.up * jumpBoost, ForceMode.Impulse);
		//this.transform.Translate(transform.up * jumpBoost * Time.fixedDeltaTime, Space.World);
	}

	public void SetRailConstrains()
	{
		if (lockRigidbodyRotation)
			m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		//m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		railSteerSpeed = railSteerRef;
		currentSpeed = maximumForwardSpeed;

		Invoke("SetRailMovement", 1);
	}

	void SetRailMovement()
	{
		//railSteerSpeed = playerOne.railSteerSpeed;
		hasStarted = true;
	}


}
