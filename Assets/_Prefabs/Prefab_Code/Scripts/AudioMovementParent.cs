using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioMovementParent : MonoBehaviour
{
	[Header("Main References")]
	public collisionAdjustmentScriptPlayer1 crashForce;
	public WinState winState;
	[SerializeField] GameObject ui;

	[Tooltip("CurrentPlayer")]
	[SerializeField] protected int currentPlayer = 1;

	[Tooltip("UI")]
	public Sprite[] ringcountUIArray;
	public Image ringcountUI;
	public Text ringCount;

	[Tooltip("Audio")]
	public AudioPitch_Player1 pitch;
	public SFXManager sfx;
	public int currentPitch;

	[Header("Movement Speeds")]
	[Range(0f, 50f)]
	public float maximumForwardSpeed; // Default 10
	[Range(0f, 50f)]
	public float forwardAccelaration; // Default 5
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
	public float currentAmp;
	public float currentTurn;
	public float currentSpeed;

	public float objectHeight;
	//public float Volume;
	public float FOV;
	private float SavedFOV;
	public bool FovWanted;

	[SerializeField] private LayerMask layer;

	[Header("Options")]
	public float minimumPitch;
	public float maximumPitch;
	public int currentPitchValue;
	public bool highPitchIsTurnRight;
	public bool soundTriggersParticles;
	public bool isSinglePlayer;
	public bool comingFromMainMenu;

	[Header("Debug")]
	//public bool debugKeyControl;
	private Vector3 sliderVector;
	[Range(1f, 50f)]
	public float railSteerSpeed;
	[HideInInspector] public float railSteerRef;

	[Tooltip("Debug CarRail movement")]
	[SerializeField] float roadWidth = 60;
	private float roadHalf;
	[Range(1f, 5000f)]
	public float railSpeed;


	Vector3 sliderPos;
	private Vector3 velocity = Vector3.zero;
	private float lastValidPitch;

	private bool hasStarted = false;
	private bool isMoving = false;
	private float boostTimer = 100;
	private float invulnerableState = 160;
	private float sliderPitchInvLerp;

	private int numRings = 5;

	//Make sure you attach a Rigidbody in the Inspector of this GameObject
	private Rigidbody m_Rigidbody;
	private ParticleSystem _partSys;
	private Collider col;
	private Transform camDist;
	private PlayerFeedBack feedBack;


	void Start()
	{
		m_Rigidbody = GetComponent<Rigidbody>();
		_partSys = GetComponent<ParticleSystem>();

		col = gameObject.GetComponent<Collider>();
		feedBack = gameObject.GetComponent<PlayerFeedBack>();
		camDist = GameObject.FindGameObjectWithTag("CarDistRef").transform;

		FOV = 60.0f;
		SavedFOV = FOV;
		railSteerRef = railSteerSpeed;
		ringcountUI.sprite = ringcountUIArray[numRings];
		lastHit = invulnerableState + 1;
		roadHalf = roadWidth / 2;

		if (!ui.activeSelf && ui != null)
			ui.SetActive(true);
		else
			Debug.LogWarning("ui is null! please add a ui ref.");



		//SetMinAndMaxPitch
		if (comingFromMainMenu == true)
		{
			minimumPitch = PlayerPrefs.GetFloat("Player" + currentPlayer.ToString() + "Lowest");
			maximumPitch = PlayerPrefs.GetFloat("Player" + currentPlayer.ToString() + "Highest");
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Player")
			Physics.IgnoreCollision(collision.collider, col, true);

		if (collision.gameObject.tag == "Track")
		{
			Vector3 colReflect = Vector3.Reflect(transform.forward, collision.contacts[0].normal);

			Physics.IgnoreCollision(collision.collider, col, true);

			if (lastHit > invulnerableState)
			{
				lastHit = 0;
				RemoveRing();
				sfx.crashIntoTrack();
			}
			if (currentSpeed > maximumForwardSpeed)
				currentSpeed = 0.50f * currentSpeed;
			else if (currentSpeed < 0.75f * maximumForwardSpeed)
				currentSpeed = 0.80f * currentSpeed;
			else
				currentSpeed = 0.70f * currentSpeed;

		}
		else
			currentSpeed = 0.75f * currentSpeed;

	}


	protected void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Ring")
		{
			if (numRings < 5)
			{
				numRings++;
				ringcountUI.sprite = ringcountUIArray[numRings];
			}

			transform.rotation = other.transform.rotation;
			boostTimer = 0;
		}
		if (other.gameObject.GetComponent<JumpPad>())
			JumpBoost(other.gameObject.GetComponent<JumpPad>().jumpStrength);
	}

	void FixedUpdate()
	{
		CarForwardMovement();
		CarSoundControls();
	}

	void StabilizeCarRot(float stabilizeSpeed)
	{
		if (!IsGrounded())
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0), stabilizeSpeed * Time.deltaTime);

		if (transform.rotation.eulerAngles.x < 290 && transform.rotation.eulerAngles.x > 10)
			Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 90, 0), 50000);

	}

	protected bool IsGrounded()
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

    #region Movement

	protected virtual void CarForwardMovement()
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
			}
			else if (transform.position.x < camDist.position.x - 7.4f + (numRings * 5f))
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
		var diffCoef = (currentSpeed / ((maximumForwardSpeed * speedBoost) - maximumForwardSpeed)) * mult;
		if (currentSpeed > maximumForwardSpeed + 1f)
			speedBoostDecelerator = diffCoef;
		else
			speedBoostDecelerator = 1f;

		currentPitch = pitch._currentpublicpitch;
		currentAmp = pitch._currentPublicAmplitude;
		//Volume = pitch._currentPublicAmplitude;

		objectHeight = this.transform.position.y;

		if (objectHeight > 0.6f)
			this.transform.Translate(0, -0.1f, 0);
	}

    protected virtual void CarSoundControls()
	{
		var emission = _partSys.emission;

		sliderVector = new Vector3(transform.position.x, transform.position.y, sliderPitchInvLerp * roadWidth - roadHalf);//Smoothen car movement
		sliderPos = Vector3.SmoothDamp(transform.position, sliderVector, ref velocity, 1, railSpeed * Time.deltaTime);


		if (currentPitch < 7)//Checks if pitch has valid value
			sliderPitchInvLerp = lastValidPitch;
		else
		{
			sliderPitchInvLerp = Mathf.InverseLerp(maximumPitch, minimumPitch, currentPitch);// set value from 0 to 1 (Min pitch value = 7, max pitch value = 30)
			lastValidPitch = sliderPitchInvLerp;
		}


		if (currentAmp > pitch.minVolumeDB)
		{
			if (currentPitch > minimumPitch)
				currentTurn = (((currentPitch - minimumPitch) / (maximumPitch - minimumPitch)) * 2) - 1 - 0.3f;

			if (highPitchIsTurnRight == false)
				currentTurn *= -1;

			if (soundTriggersParticles == true)
				emission.rateOverTime = 10;
			else
				emission.rateOverTime = 0;

			if (hasStarted)
			{
				if (currentSpeed < maximumForwardSpeed)
					currentSpeed += forwardAccelaration * Time.fixedDeltaTime;
				else
					currentSpeed -= forwardDeceleration * speedBoostDecelerator * Time.fixedDeltaTime;
			}

			this.transform.position = sliderPos + transform.forward * currentSpeed * Time.fixedDeltaTime;

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
				currentTurn -= Time.fixedDeltaTime * turningDeceleration;
			else if (currentTurn < 0)
				currentTurn += Time.fixedDeltaTime * turningDeceleration;

			emission.rateOverTime = 0;
			
			this.transform.position = sliderPos + transform.forward * currentSpeed * Time.fixedDeltaTime;

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

	public void JumpBoost(float jumpBoost)
	{
		m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, 0, m_Rigidbody.velocity.z);
		m_Rigidbody.AddForce(transform.up * jumpBoost, ForceMode.Impulse);
	}

	public void SetRailConstrains()
	{
		m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;

		railSteerSpeed = railSteerRef;
		isMoving = true;
		Invoke("SetRailMovement", 1);
	}

	void SetRailMovement()
	{
		hasStarted = true;
	}
	#endregion

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


}
