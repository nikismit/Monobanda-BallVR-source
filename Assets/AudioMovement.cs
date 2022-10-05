using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioMovement : MonoBehaviour {

	public AudioMovementPlayer2 player2;
	public collisionAdjustmentScriptPlayer1 crashForce;
	public WinState winState;
	private Transform camDist;
	private SnakeBehavior carLine;
	[SerializeField] GameObject ui;

	public Sprite[] ringcountUIArray;
	public Image ringcountUI;

	[Header("Sound References")]
	public AudioPitch_Player1 pitch;
	public SFXManager sfx;
	public Text ringCount;
	public int currentPitch;
	PlayerFeedBack feedBack;

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
	[Space]
	public bool ignorePlayerCol;
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

	private float boostTimer = 100;

	private int numRings = 5;
	private bool canAddCar = true;

	private float invulnerableState = 160;

	//Make sure you attach a Rigidbody in the Inspector of this GameObject
	Rigidbody m_Rigidbody;
	ParticleSystem _partSys;
	private Collider col;


	void Start()
    {
		m_Rigidbody = GetComponent<Rigidbody>();
		_partSys = GetComponent<ParticleSystem>();

		col = gameObject.GetComponent<Collider>();
		carLine = gameObject.GetComponent<SnakeBehavior>();
		feedBack = gameObject.GetComponent<PlayerFeedBack>();
		camDist = GameObject.FindGameObjectWithTag("CarDistRef").transform;


		lastHit = invulnerableState + 1;
		if (!ui.activeSelf && ui != null)
		{
			ui.SetActive(true);
		}
		else
			Debug.LogWarning("ui is null! please add a ui ref.");
		ringcountUI.sprite = ringcountUIArray[numRings];


		FOV = 60.0f;
		SavedFOV = FOV;

		//SetMinAndMaxPitch
		if (comingFromMainMenu == true){
			minimumPitch = PlayerPrefs.GetFloat("Player1Lowest");
			maximumPitch = PlayerPrefs.GetFloat("Player1Highest");
		}

		railSteerRef = railSteerSpeed;
		//railSteerSpeed = 0;

		roadHalf = roadWidth / 2;
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
		if (setCrashCol && collision.gameObject.tag == "Player" && !ignorePlayerCol)
        {
			m_Rigidbody.AddForce(Vector3.forward * (currentTurn - player2.currentTurn) * 20, ForceMode.Impulse);
			m_Rigidbody.drag = 1;
        }
		else if(collision.gameObject.tag == "Track")
		{
			Vector3 colReflect = Vector3.Reflect(transform.forward, collision.contacts[0].normal);

			Physics.IgnoreCollision(collision.collider, col, true);

			if (lastHit > invulnerableState)
            {
				lastHit = 0;
				RemoveRing();
				sfx.crashIntoTrack();
			}

			/*
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
			crashForce.onCollisionCorrection(colReflect);
			*/

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
		if (lastHit < invulnerableState)
		{
			lastHit++;
			feedBack.BlinkFeedBack(true);
		}
		else
			feedBack.BlinkFeedBack(false);


		if (isMoving)
        {
			currentSpeed = maximumForwardSpeed;

			if (boostTimer < 1)
			{
				boostTimer += Time.deltaTime;

				float ease = Mathf.InverseLerp(0, 2, boostTimer) * 5;
				currentSpeed += 10;

				StabilizeCarRot(1);

			}
			else if (transform.position.x >= camDist.position.x - 7.5f + (numRings * 5f))
			{//- 7.5f
				currentSpeed -= 5;
				StabilizeCarRot(5);
				//ringModels[0].transform.Rotate(new Vector3(0, 0, 1));
			}
			else if (transform.position.x < camDist.position.x - 0.1f - 7.5f + (numRings * 5f))
			{
				currentSpeed += 5;
				StabilizeCarRot(5);
				//ringModels[0].transform.Rotate(new Vector3(0, 0, 1));
			}
            else
            {
				StabilizeCarRot(5);
				currentSpeed = maximumForwardSpeed;
			}
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
			sliderPos = Vector3.SmoothDamp(transform.position, sliderVector, ref velocity, 1, railSpeed * Time.deltaTime);

		if (!debugKeyControl)
			CarSoundMovement();
		else
			CarKeyMovement();
	}

	void StabilizeCarRot(float stabilizeSpeed)
	{
		if (!IsGrounded())
        {
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0), stabilizeSpeed * Time.deltaTime);
		}

		if (transform.rotation.eulerAngles.x < 290 && transform.rotation.eulerAngles.x > 10)
        {
			Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 90, 0), 50000);
			Debug.Log("FIX ROT");
		}

	}

	public bool IsGrounded()
    {
		Debug.DrawRay(transform.position, (Vector3.down + Vector3.forward) * 0.5f, Color.green);
		Debug.DrawRay(transform.position, (Vector3.down + Vector3.back) * 0.5f, Color.green);
		Debug.DrawRay(transform.position, (Vector3.down + Vector3.left) * 0.5f, Color.green);
		Debug.DrawRay(transform.position, (Vector3.down + Vector3.right) * 0.5f, Color.green);

		if (Physics.Raycast(transform.position, Vector3.down + Vector3.forward, 0.8f, layer) ||
			Physics.Raycast(transform.position, Vector3.down + Vector3.back, 0.8f, layer) ||
			Physics.Raycast(transform.position, Vector3.down + Vector3.left, 0.8f, layer) ||
			Physics.Raycast(transform.position, Vector3.down + Vector3.right, 0.8f, layer))
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
			currentTurn = hor;


			if (soundTriggersParticles == true)
			{

				emission.rateOverTime = 10;
			}
			else
			{
				emission.rateOverTime = 0;
			}
            if (hasStarted)
            {


			if (currentSpeed < maximumForwardSpeed)
			{
				currentSpeed += forwardAccelaration * Time.fixedDeltaTime;
			}
			else
			{
				currentSpeed -= forwardDeceleration * speedBoostDecelerator * Time.fixedDeltaTime;
			}
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
			if (hasStarted)
			{
				if (currentSpeed < maximumForwardSpeed)
				{
					currentSpeed += forwardAccelaration * Time.fixedDeltaTime;
				}
				else
				{
					currentSpeed -= forwardDeceleration * speedBoostDecelerator * Time.fixedDeltaTime;
				}
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

	float lastHit;

	public void RemoveRing()
    {
		feedBack.HitFeedBack();

		if (numRings > 0)
		{
			if (Time.time - lastHit < 2)
				return;

			lastHit = Time.time;
			numRings--;
			ringcountUI.sprite = ringcountUIArray[numRings];
		}
		else
		{
			winState.PlayerTwoWins();
			Destroy(gameObject);
		}
	}

	public void JumpBoost(float jumpBoost)
    {
		//cubeRb.velocity = new vector3(cubeRB.velocity.x, 0, 0);
		m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, 0, m_Rigidbody.velocity.z);

		m_Rigidbody.AddForce(transform.up * jumpBoost, ForceMode.Impulse);
		//transform.Translate(Vector3.up * jumpBoost);

	}

	public void SetRailConstrains()
	{
		if(lockRigidbodyRotation)
		m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
		//m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		railSteerSpeed = railSteerRef;
		isMoving = true;
		Invoke("SetRailMovement", 1);
	}

	void SetRailMovement()
    {
		hasStarted = true;
	}
}
