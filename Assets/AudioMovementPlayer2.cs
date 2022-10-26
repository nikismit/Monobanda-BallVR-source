using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioMovementPlayer2 : MonoBehaviour {

	public collisionAdjustmentScriptPlayer2 crashForce;
	private SnakeBehavior carLine;
	public AudioPitch_Player2 pitch;
	public SFXManager sfx;
	public Text ringCount;
	public int currentPitch;
	PlayerFeedBack feedBack;
	[SerializeField] LayerMask layer;

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

	public FinishScript Done;

	[Header("Options")]
	public float minimumPitch;
	public float maximumPitch;
	public int currentPitchValue;
	public bool highPitchIsTurnRight;
	public bool soundTriggersParticles;
	public bool isSinglePlayer;
	public bool comingFromMainMenu;

	[Header("DebugEndlessRunner")]
	[SerializeField] bool endlessRunner;
	[SerializeField] bool removeHealth;

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
	private float minimumAmp = 0;

	private AudioMovement playerOne;

	private int numRings = 5;
	public Sprite[] ringcountUIArray;
	public Image ringcountUI;

	//Make sure you attach a Rigidbody in the Inspector of this GameObject
	Rigidbody m_Rigidbody;
	ParticleSystem _partSys;
	[SerializeField] ParticleSystem boostParticle;
	public AudioMovement player1;
	public WinState winState;

	private float invulnerableState = 160;

	private float voiceSetbackTime = 0;
	private float maxVoiceSetback = 7;

	void Start()
    {
		maxVoiceSetback = player1.maxVoiceSetback;

		carLine = gameObject.GetComponent<SnakeBehavior>();
		_partSys = GetComponent<ParticleSystem>();

		col = gameObject.GetComponent<Collider>();
		feedBack = gameObject.GetComponent<PlayerFeedBack>();
		m_Rigidbody = GetComponent<Rigidbody>();


		lastHit = invulnerableState + 1;
		FOV = 60.0f;
		SavedFOV = FOV;
		if(comingFromMainMenu == true){
			minimumPitch = PlayerPrefs.GetFloat("Player2Lowest");
			maximumPitch = PlayerPrefs.GetFloat("Player2Highest");
		}
		ringcountUI.sprite = ringcountUIArray[numRings];

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
		if (setCrashCol && collision.gameObject.tag == "Player" && !ignorePlayerCol)
		{
			m_Rigidbody.AddForce(Vector3.forward * (currentTurn - player1.currentTurn) * 20, ForceMode.Impulse);
			m_Rigidbody.drag = 1;
		}
		else if(collision.gameObject.tag == "Track" && !removeHealth)
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

				if (currentTurn > 0)
					reflect = Vector3.Reflect(transform.right, collision.contacts[0].normal);
				else
					reflect = Vector3.Reflect(-transform.right, collision.contacts[0].normal);
				transform.Translate(reflect, Space.World);
			}
			else
				crashForce.onCollisionCorrection(colReflect);
		sfx.crashIntoTrack();
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

	private bool canAddCar = true;

	private void OnTriggerEnter(Collider other)
    {
		if (other.gameObject.tag == "Ring"){
			
			boostParticle.Play();

			if (numRings < 5)
            {
				numRings++;
				ringcountUI.sprite = ringcountUIArray[numRings];
			}

				//carLine.AddBodyPart(1, 0);
			transform.rotation = other.transform.rotation;
			boostTimer = 0;
		}

		if (other.gameObject.GetComponent<JumpPad>())
		{
			JumpPad jumpPadRef = other.gameObject.GetComponent<JumpPad>();

			JumpBoost(jumpPadRef);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Ring")
			canAddCar = true;
	}

	void FixedUpdate()
    {
		float colDist = Vector3.Distance(transform.position, player1.transform.position);

		if (colDist < 1)
		{
			Vector3 currentDirection = (transform.position - player1.transform.position).normalized;

			float PushAmount = Mathf.InverseLerp(0, 1, colDist);
			m_Rigidbody.AddForce(transform.right * -currentDirection.z * PushAmount * 80);
		}

		if (lastHit < invulnerableState)
		{
			lastHit++;
			feedBack.BlinkFeedBack(true);
		}
		else
		{
			lastHit = invulnerableState + 1;
			feedBack.BlinkFeedBack(false);
		}


		if (playerOne.isMoving)
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
			{
				currentSpeed -= 5;
				StabilizeCarRot(5);
			}
			else if (transform.position.x < camDist.position.x - 0.1f - 7.5f + (numRings * 5f))
			{
				currentSpeed += 5;
				StabilizeCarRot(5);
			}
            else
            {
				StabilizeCarRot(5);
				currentSpeed = maximumForwardSpeed;
			}
		}

		var mult = speedBoost * maximumForwardSpeed;
		var diffCoef = (currentSpeed/((maximumForwardSpeed*speedBoost)-maximumForwardSpeed))*mult;
		if (currentSpeed > maximumForwardSpeed+1f)
		{
			speedBoostDecelerator = diffCoef;
		}
		else
		{
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


		if (currentPitch < 7)
		{
			sliderPitchInvLerp = lastValidPitch;
		}
		else
		{
			sliderPitchInvLerp = Mathf.InverseLerp(maximumPitch, minimumPitch, currentPitch);// set value from 0 to 1 (Min pitch value = 7, max pitch value = 30)
			lastValidPitch = sliderPitchInvLerp;
		}

		//if (hasStarted)
			sliderVector = new Vector3(transform.position.x, transform.position.y, sliderPitchInvLerp * roadWidth - roadHalf);
        //else
        //sliderVector = transform.position;

        if (currentAmp < minimumAmp)
        {
			minimumAmp = currentAmp;
        }

		if (currentAmp > minimumAmp + 20)
		{
			sliderPos = Vector3.SmoothDamp(transform.position, sliderVector, ref velocity, 1, railSpeed * Time.deltaTime);
		}
		else
		{
			float slowStop = Mathf.InverseLerp(railSpeed, 0, 10 * Time.deltaTime);
			//Vector3 sliderVector = new Vector3(transform.position.x, transform.position.y, sliderPitchInvLerp * roadWidth - roadHalf);
			sliderPos = Vector3.SmoothDamp(transform.position, sliderVector, ref velocity, 1, slowStop);
		}
		//sliderPos = Vector3.SmoothDamp(transform.position, sliderVector, ref velocity, 1, railSpeed * Time.deltaTime);

		CarSoundMovement();

		if (IsGrounded() && GroundCtrl() != null)
		{
			RampControl(GroundCtrl());
		}
		else
		{
			Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 90, 0), 50000);
			//Debug.Log("Ground");
		}

		if (player1.testVoiceSetback && Input.GetAxisRaw("Horizontal") != 0 && player1.debugKeyControl && hasStarted ||
			player1.testVoiceSetback && !player1.debugKeyControl && hasStarted && currentAmp <= minimumAmp + 20)
		{
			//Debug.Log("Setting back Time!");
			voiceSetbackTime = 0;
		}
		else if (player1.testVoiceSetback && hasStarted && Input.GetAxisRaw("Horizontal") == 0 ||
				 player1.testVoiceSetback && !player1.debugKeyControl && hasStarted && currentAmp >= minimumAmp + 20)
		{
			//Debug.Log("TimerSET" + voiceSetbackTime);
			voiceSetbackTime += Time.deltaTime;

			if (voiceSetbackTime >= maxVoiceSetback && !removeHealth)
			{
				voiceSetbackTime = 0;
				RemoveRing();
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
			if (PitchSliderMovement)
				this.transform.position = sliderPos + transform.forward * currentSpeed * Time.fixedDeltaTime;
			else if (testRailControl)
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
			if (PitchSliderMovement)
				this.transform.position = sliderPos + transform.forward * currentSpeed * Time.fixedDeltaTime;
			else if (testRailControl)
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

	void RampControl(Transform trans)
	{
		Vector3 gravityUp = trans.up;
		Vector3 localUp = transform.up;

		transform.up = Vector3.Lerp(transform.up, gravityUp, 20 * Time.deltaTime);
		transform.rotation = Quaternion.Euler(-transform.rotation.eulerAngles.z, 90, 0);
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
			Debug.Log("FIX ROT Player2");
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

	public Transform GroundCtrl()
	{
		Debug.DrawRay(transform.position, (Vector3.down) * 0.8f, Color.green);
		RaycastHit hit;

		if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.8f, layer))
			return hit.transform;
		else
			return null;
	}

	float lastHit;

	public void RemoveRing()
	{
		feedBack.HitFeedBack();

		if (numRings > 0)
        {
            //if (Time.time-lastHit < 2)
				//return;

			//lastHit = Time.time;
			numRings--;
			ringcountUI.sprite = ringcountUIArray[numRings];
		}
        else
        {
			winState.PlayerOneWins();
			Destroy(gameObject);
		}
	}

	bool jumpCoolDown = false;

	public void JumpBoost(JumpPad jumpRef)
	{
		//m_Rigidbody.AddForce(transform.up * jumpBoost, ForceMode.Impulse);
		//m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, 0, m_Rigidbody.velocity.z);
		if (!jumpCoolDown)
		{
			jumpCoolDown = true;
			StartCoroutine(Jumping(jumpRef));
		}
		//m_Rigidbody.AddForce(transform.up * jumpBoost, ForceMode.Impulse);
	}

	IEnumerator Jumping(JumpPad jumpRef)
	{
		m_Rigidbody.AddForce(transform.up * jumpRef.jumpStrength, ForceMode.Impulse);
		jumpRef.source.Play();
		yield return new WaitForSeconds(0.5f);
		jumpCoolDown = false;
	}

	public void SetRailConstrains()
	{
		if (lockRigidbodyRotation)
			m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
		//m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

		railSteerSpeed = railSteerRef;

		Invoke("SetRailMovement", 1);
	}

	void SetRailMovement()
	{
		hasStarted = true;
	}


}
