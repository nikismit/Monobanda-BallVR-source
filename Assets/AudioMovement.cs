using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioMovement : MonoBehaviour {

	public collisionAdjustmentScriptPlayer1 crashForce;
	private SnakeBehavior carLine;
	public AudioPitch_Player1 pitch;
	public SFXManager sfx;
	public Text ringCount;
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


	[SerializeField]private LayerMask layer;

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
	public bool testRailControl;
	public bool PitchSliderMovement;
	private Vector3 sliderVector;
	[Range(1f, 50f)]
	public float railSteerSpeed;
	[HideInInspector] public float railSteerRef;

	public bool ignorePlayerCol;
	private Collider col;

	public bool setCrashCol;
	public bool lockRigidbodyRotation;

	[Header("Debug CarRail movement")]
	[SerializeField] float roadWidth = 60;
	private float roadHalf;
	[Range(1f, 5000f)] public float railSpeed;
	float sliderPitchInvLerp;

	Vector3 sliderPos;
	private Vector3 velocity = Vector3.zero;
	private float lastValidPitch;

	private bool hasStarted = false;
	[HideInInspector] public bool isMoving = false;

	public Sprite[] ringcountUIArray;
	public SpriteRenderer ringcountUI;

	private float boostTimer = 100;

	private Transform camDist;

	private int numRings = 0;

	//Make sure you attach a Rigidbody in the Inspector of this GameObject
	Rigidbody m_Rigidbody;
	ParticleSystem _partSys;

    void Start()
    {
		col = gameObject.GetComponent<Collider>();
		carLine = gameObject.GetComponent<SnakeBehavior>();

        //Fetch the Rigidbody from the GameObject with this script attached
        m_Rigidbody = GetComponent<Rigidbody>();
		_partSys = GetComponent<ParticleSystem>();
		startMaxSpeed = maximumForwardSpeed;
		startAccel = forwardAccelaration;
		FOV = 60.0f;
		SavedFOV = FOV;

		//SetMinAndMaxPitch
		if (comingFromMainMenu == true){
			minimumPitch = PlayerPrefs.GetFloat("Player1Lowest");
			maximumPitch = PlayerPrefs.GetFloat("Player1Highest");
		}

		railSteerRef = railSteerSpeed;
		railSteerSpeed = 0;

		roadHalf = roadWidth / 2;

		camDist = GameObject.FindGameObjectWithTag("CarDistRef").transform;

		//if (lockRigidbody)
		//m_Rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
	}

    void OnCollisionEnter(Collision collision)
	{
		/*
		if (collision.gameObject.tag == "Player")
        {
			Debug.Log("yyoyoyo");

			Vector3 otherVel = collision.rigidbody.velocity;

			var speed = m_Rigidbody.velocity.magnitude;

			var dir = Vector3.Reflect(m_Rigidbody.velocity.normalized, collision.contacts[0].normal);

			m_Rigidbody.velocity = dir * Mathf.Max(speed, 0f);

        }

		if (collision.gameObject.tag == "Box")
		{
			currentSpeed = currentSpeed;
		}
				*/

		if (collision.gameObject.tag == "Player")
		{
			Physics.IgnoreCollision(collision.collider, col, ignorePlayerCol);
		}


		if (collision.gameObject.tag == "Box"){
			currentSpeed = currentSpeed;
		}
		else if(collision.gameObject.tag == "Track" || setCrashCol && collision.gameObject.tag == "Player" && !ignorePlayerCol)
		{

			Vector3 col = Vector3.Reflect(transform.forward, collision.contacts[0].normal);


			//Debug.LogWarning(collision.contacts[0].normal);


			if (testRailControl)//TODO: only works on right wall
            {
				Vector3 reflect = Vector3.zero;

				if(currentTurn > 0)
					reflect = Vector3.Reflect(transform.right, collision.contacts[0].normal);
					else
				reflect = Vector3.Reflect(-transform.right, collision.contacts[0].normal);
				transform.Translate(reflect, Space.World);
			}
			else
			crashForce.onCollisionCorrection(col);

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
		if (other.gameObject.tag == "Ring")
		{
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
			//transform.position
			boostTimer = 0;
		}

		Debug.Log(other);
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
        if (isMoving)
        {
			if (boostTimer < 1)
			{
				boostTimer += Time.deltaTime;

				float ease = Mathf.InverseLerp(0, 2, boostTimer) * 5;
				Debug.Log(boostTimer);
				//ringModels[0].transform.Rotate(new Vector3(0, 0, 200));
				currentSpeed += 10;

			}
			else if (transform.position.x >= camDist.position.x + (numRings * 2))
			{
				currentSpeed -= 5;
				//ringModels[0].transform.Rotate(new Vector3(0, 0, 1));
			}
			else if (transform.position.x < camDist.position.x - 0.1f + (numRings * 2))
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
		//currentPitchValue = currentPitch;

		objectHeight = this.transform.position.y;
		if (objectHeight > 0.6f){
			this.transform.Translate(0,-0.1f,0);
		}


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



		if (!debugKeyControl)
			CarSoundMovement();
		else
			CarKeyMovement();
		// if(currentPitch > minimumPitch && currentAmp > -15f){
		// 		currentTurn = (((currentPitch-minimumPitch)/(maximumPitch-minimumPitch))*2)-1;

        if (!IsGrounded())
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0), 1 * Time.deltaTime);
	}

	public bool IsGrounded()
    {
		if (Physics.Raycast(transform.position, Vector3.down + Vector3.forward, 5, layer) ||
			Physics.Raycast(transform.position, Vector3.down + Vector3.back, 5, layer) ||
			Physics.Raycast(transform.position, Vector3.down + Vector3.left, 5, layer) ||
			Physics.Raycast(transform.position, Vector3.down + Vector3.right, 5, layer))
			return true;
		else
			return false;
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

			if (testRailControl)
				this.transform.Translate((transform.forward * currentSpeed + transform.right * currentTurn * railSteerSpeed) * Time.fixedDeltaTime, Space.World);
            else
            {
				this.transform.Translate(transform.forward * currentSpeed * Time.fixedDeltaTime, Space.World);
				this.transform.Rotate(Vector3.up * turningSpeed * currentTurn * Time.fixedDeltaTime, Space.World);
			}

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

			if(testRailControl)
				this.transform.Translate((transform.forward * currentSpeed + transform.right * currentTurn * railSteerSpeed) * Time.fixedDeltaTime, Space.World);
            else
            {
				this.transform.Translate(transform.forward * currentSpeed * Time.fixedDeltaTime, Space.World);
				this.transform.Rotate(Vector3.up * turningSpeed * currentTurn * Time.fixedDeltaTime, Space.World);
			}

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
			if (testRailControl)
            {
				if (PitchSliderMovement)
					this.transform.position = sliderPos + transform.forward * currentSpeed * Time.fixedDeltaTime;
				else if (testRailControl)
					this.transform.Translate((transform.forward * currentSpeed + transform.right * currentTurn * railSteerSpeed) * Time.fixedDeltaTime, Space.World);
				else
				{
					this.transform.Translate(transform.forward * currentSpeed * Time.fixedDeltaTime, Space.World);
					this.transform.Rotate(Vector3.up * turningSpeed * currentTurn * Time.fixedDeltaTime, Space.World);
				}
			}
			else
			{
				if (PitchSliderMovement)
					this.transform.position = sliderPos + transform.forward * currentSpeed * Time.fixedDeltaTime;
				else if (testRailControl)
					this.transform.Translate((transform.forward * currentSpeed + transform.right * currentTurn * railSteerSpeed) * Time.fixedDeltaTime, Space.World);
				else
				{
					this.transform.Translate(transform.forward * currentSpeed * Time.fixedDeltaTime, Space.World);
					this.transform.Rotate(Vector3.up * turningSpeed * currentTurn * Time.fixedDeltaTime, Space.World);
				}
			}
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
			{
				if (PitchSliderMovement)
					this.transform.position = sliderPos + transform.forward * currentSpeed * Time.fixedDeltaTime;
				else if (testRailControl)
					this.transform.Translate((transform.forward * currentSpeed + transform.right * currentTurn * railSteerSpeed) * Time.fixedDeltaTime, Space.World);
				else
				{
					this.transform.Translate(transform.forward * currentSpeed * Time.fixedDeltaTime, Space.World);
					this.transform.Rotate(Vector3.up * turningSpeed * currentTurn * Time.fixedDeltaTime, Space.World);
				}
			}
			{
				if (PitchSliderMovement)
					this.transform.position = sliderPos + transform.forward * currentSpeed * Time.fixedDeltaTime;
				else if (testRailControl)
					this.transform.Translate((transform.forward * currentSpeed + transform.right * currentTurn * railSteerSpeed) * Time.fixedDeltaTime, Space.World);
				else
				{
					this.transform.Translate(transform.forward * currentSpeed * Time.fixedDeltaTime, Space.World);
					this.transform.Rotate(Vector3.up * turningSpeed * currentTurn * Time.fixedDeltaTime, Space.World);
				}
			}
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

	public void RemoveRing()
    {
		if (numRings > 0)
		{
			numRings--;
			ringcountUI.sprite = ringcountUIArray[numRings];
			//ringCount.text = numRings.ToString();
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
		if(lockRigidbodyRotation)
		m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		//m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		railSteerSpeed = railSteerRef;
		currentSpeed = maximumForwardSpeed;
		isMoving = true;

		Invoke("SetRailMovement",1);
	}

	void SetRailMovement()
    {
		hasStarted = true;
	}
}
