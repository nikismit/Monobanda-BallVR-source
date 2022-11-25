using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioMovement : MonoBehaviour {

	public AudioMovementPlayer2 player2;
	public collisionAdjustmentScriptPlayer1 crashForce;
	public WinState winState;
	private Transform camDist;
	//private SnakeBehavior carLine;
	[SerializeField] GameObject ui;
	[SerializeField] ModelEffects modelEffect;
	[SerializeField] TrailRenderer trailEffect;
	[SerializeField] FlameEffect flameEffect;
	//private PlayerNotifyHandler notifyHandler;

	//public Sprite[] ringcountUIArray;
	public Slider playerHealth;
	public TextMeshProUGUI ringCountText;
	[SerializeField] PlayersUIHandler uiHandler;
	private float score = 0;

	[Header("Sound References")]
	//public AudioPitch_Player1 pitch;
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
	public bool testVoiceSetback;
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

	private float invulnerableState = 160;
	[SerializeField] private float minimumAmp = 0;

	//Make sure you attach a Rigidbody in the Inspector of this GameObject
	Rigidbody m_Rigidbody;
	ParticleSystem _partSys;
	[SerializeField] ParticleSystem[] boostParticle;
	private Collider col;

	float lastHit;

	private float voiceSetbackTime = 0;
	public float maxVoiceSetback = 7;
	private float boostAmount = 5;

	[HideInInspector] public bool isInPipe;

	void Start()
    {
		m_Rigidbody = GetComponent<Rigidbody>();
		_partSys = GetComponent<ParticleSystem>();

		col = gameObject.GetComponent<Collider>();
		//carLine = gameObject.GetComponent<SnakeBehavior>();
		feedBack = gameObject.GetComponent<PlayerFeedBack>();
		camDist = GameObject.FindGameObjectWithTag("CarDistRef").transform;

		//notifyHandler = gameObject.GetComponent<PlayerNotifyHandler>();

		lastHit = invulnerableState + 1;
		if (!ui.activeSelf && ui != null)
		{
			ui.SetActive(true);
		}
		else
			Debug.LogWarning("ui is null! please add a ui ref.");
		playerHealth.maxValue = 5;
		playerHealth.value = 5;
		//ringcountUI.sprite = ringcountUIArray[numRings];


		FOV = 60.0f;
		SavedFOV = FOV;

		//SetMinAndMaxPitch
		/*
		if (comingFromMainMenu == true){
			minimumPitch = PlayerPrefs.GetFloat("Player1Lowest");
			maximumPitch = PlayerPrefs.GetFloat("Player1Highest");
		}
		*/

		railSteerRef = railSteerSpeed;
		//railSteerSpeed = 0;

		roadHalf = roadWidth / 2;
	}

	bool squashAble = false;

    void OnCollisionEnter(Collision collision)
	{
        if (collision.gameObject.layer == 8 && squashAble)
        {
			squashAble = false;
			modelEffect.Squash(new Vector3(1.5f, 0.7f, 1.5f));
		}


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
			//else

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
            for (int i = 0; i < boostParticle.Length; i++)
            {
				boostParticle[i].Play();
			}

			flameEffect.InitiateBoostEffect();

			//score += 100;
			//ringCountText.text = score.ToString();

			modelEffect.Squash(new Vector3(0.6f, 0.7f, 1.20f));

			uiHandler.UpdateScore(100, 0);

			if (other.gameObject.GetComponent<StringerBoosterRing>())
            {
				//float boost = other.GetComponent<StringerBoosterRing>().BoostAmount;
				boostAmount = other.GetComponent<StringerBoosterRing>().BoostAmount;

				m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, 5, m_Rigidbody.velocity.z);
				//m_Rigidbody.AddForce(transform.forward * boost, ForceMode.Impulse);

				//m_Rigidbody.drag = 1;

				//Invoke("ResetDrag", 2);
			}


			if (numRings < 5)
			{
				//Debug.Log(5 / numRings);
				numRings++;
				//ringcountUI.sprite = ringcountUIArray[numRings];
				playerHealth.value = numRings;
				//ringCount.text = numRings.ToString();
			}
			//carLine.AddBodyPart(1, 0);

			transform.rotation = other.transform.rotation;
			boostTimer = 0;
		}
		if (other.gameObject.GetComponent<JumpPad>())
        {
			modelEffect.Squash(new Vector3(0.7f, 1.2f, 0.7f));
			JumpPad jumpPadRef = other.gameObject.GetComponent<JumpPad>();
			//Debug.LogError("JUMPING");
			//Debug.LogError("JUMPING");
			JumpBoost(jumpPadRef);
		}
	}

    private void ResetDrag()
    {
		m_Rigidbody.drag = 0;
	}

	void EnableSquash()
    {
		squashAble = true;
	}

    void FixedUpdate()
    {
		if (!IsGrounded())
			Invoke("EnableSquash", 1f);
		else if (IsGrounded() && hasStarted)
			CancelInvoke();

		//notifyHandler.currentVolume = Volume;
		if (player2 != null)
		{
			float colDist = Vector3.Distance(transform.position, player2.transform.position);

			if (colDist < 2 && transform.position.z >= -29 && transform.position.z <= 29)
			{
				Vector3 currentDirection = (transform.position - player2.transform.position).normalized;

				float PushAmount = Mathf.InverseLerp(0, 1, colDist);
				m_Rigidbody.AddForce(transform.right * -currentDirection.z * PushAmount * 60);
				m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
			}
			else if (transform.position.z <= -29 || transform.position.z >= 29)
			{
				//m_Rigidbody.velocity = Vector3.zero;
				//m_Rigidbody.angularVelocity = Vector3.zero;
				m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_Rigidbody.velocity.y, 0);
				m_Rigidbody.angularVelocity = new Vector3(m_Rigidbody.velocity.x, m_Rigidbody.velocity.y, 0);
				//m_Rigidbody.constraints = RigidbodyConstraints.FreezePositionX;
			}
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



		if (isMoving)
        {
			currentSpeed = maximumForwardSpeed;

			if (boostTimer < 1)
			{
				boostTimer += Time.deltaTime;

				float ease = Mathf.InverseLerp(0, 2, boostTimer) * 5;
				currentSpeed += boostAmount;

				StabilizeCarRot(1);
			}
			else if (transform.position.x >= camDist.position.x - 7.5f + (numRings * 5f))
			{//- 7.5f
				boostAmount = 10;
				currentSpeed -= 5;
				//currentSpeed -= Mathf.Abs(transform.position.x - camDist.position.x) / 2;
				StabilizeCarRot(5);
			}
			else if (transform.position.x < camDist.position.x - 0.1f - 7.5f + (numRings * 5f))
			{
				//boostAmount = 10;
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

		//currentPitch = pitch._currentPublicPitches[0];
		//currentAmp = pitch._currentPublicAmplitudes[0];
		//Volume = pitch._currentPublicAmplitudes[0];

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

			if (currentPitch < calMinPitch)
				calMinPitch = currentPitch;
			if (currentPitch > calMaxPitch)
				calMaxPitch = currentPitch;
		}

		//if (hasStarted)
			sliderVector = new Vector3(transform.position.x, transform.position.y, Mathf.Clamp(sliderPitchInvLerp * roadWidth - roadHalf, -roadHalf,roadHalf));
		//else
		//sliderVector = transform.position;
		if (currentAmp < minimumAmp || minimumAmp == -Mathf.Infinity)
		{
			minimumAmp = currentAmp;
		}

		if (testVoiceSetback && Input.GetAxisRaw("Horizontal") != 0 && debugKeyControl && hasStarted ||
			testVoiceSetback && !debugKeyControl && hasStarted && currentAmp <= minimumAmp + 20)
		{
			//Debug.Log("Setting back Time!");
			voiceSetbackTime = 0;
		}
		else if (testVoiceSetback && hasStarted && Input.GetAxisRaw("Horizontal") == 0 ||
				 testVoiceSetback && !debugKeyControl && hasStarted && currentAmp >= minimumAmp + 20)
		{
			//Debug.Log("TimerSET" + voiceSetbackTime);
			voiceSetbackTime += Time.deltaTime;

			if (voiceSetbackTime >= maxVoiceSetback)
			{
				voiceSetbackTime = 0;
				RemoveRing();
			}
		}


		if (currentAmp > minimumAmp + 20 && !debugKeyControl)
        {
			voiceSetbackTime = 0;
			sliderPos = Vector3.SmoothDamp(transform.position, sliderVector, ref velocity, 1, railSpeed * Time.deltaTime);

			//modelEffect.dir = sliderPos.z;
			modelEffect.dir = currentTurn;
			//modelEffect.dir = Mathf.Ceil(currentTurn);
		}
        else
        {
			float slowStop = Mathf.InverseLerp(railSpeed, 0, 10 * Time.deltaTime);
			sliderPos = Vector3.SmoothDamp(transform.position, sliderVector, ref velocity, 1, slowStop);

			//modelEffect.dir = 0;
		}

        if (!isInPipe)
        {
			if (!debugKeyControl)
				CarSoundMovement();
			else
				CarKeyMovement();
		}


		
		if (IsGrounded() && GroundCtrl() != null)
		{
			RampControl(GroundCtrl());
		}
        else
        {
			Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 90, 0), 50000);
			//Debug.Log("Ground");
		}
		/*
		if(transform.rotation.y != 90)
        {
			transform.rotation = Quaternion.Euler(0, 90, 0);
		}
		*/

		trailEffect.time = trailTime;

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
	

	public Transform GroundCtrl()
	{
		Debug.DrawRay(transform.position, (Vector3.down) * 0.8f, Color.green);
		RaycastHit hit;

		if (Physics.Raycast(transform.position, Vector3.down, out hit,0.8f, layer))
			return hit.transform;
		else
			return null;
	}

	float trailTime = 0;

	void CarKeyMovement()
    {
		float hor = Input.GetAxisRaw("Horizontal");
		float ver = Input.GetAxisRaw("Vertical");

		var emission = _partSys.emission;
		modelEffect.dir = hor;

		if (hor != 0 || ver > 0)
		{
            if (trailTime <= 1)
            {
				trailTime += 0.05f;
            }

			if (numRings > 0)
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


			if (trailTime >= 0.1)
			{
				trailTime -= 0.05f;
			}

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

	float calMinPitch = 20;
	float calMaxPitch;

	void CarSoundMovement()
    {
		var emission = _partSys.emission;



		//Debug.Log("SoundScale = "+ soundScale);

		//modelEffect.dir = carDir.x;

		if (currentAmp > pitch.minVolumeDB)
		{


			if (trailTime <= 1)
			{
				trailTime += 0.05f;
			}

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
				if (PitchSliderMovement && numRings > 0)
					this.transform.position = sliderPos + transform.forward * currentSpeed * Time.fixedDeltaTime;
				//else if (testRailControl)
					//this.transform.Translate((transform.forward * currentSpeed + transform.right * currentTurn * railSteerSpeed) * Time.fixedDeltaTime, Space.World);
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
			if (trailTime >= 0.1)
			{
				trailTime -= 0.05f;
			}

			trailEffect.time = 0.1f;

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

	void RampControl(Transform trans)
    {
		Vector3 gravityUp = trans.up;
		Vector3 localUp = transform.up;

		transform.up = Vector3.Lerp(transform.up, gravityUp, 20 * Time.deltaTime);
		transform.rotation = Quaternion.Euler(-transform.rotation.eulerAngles.z, 90, 0);
	}

	public void RemoveRing()
    {
		feedBack.HitFeedBack();

		if (numRings > 1)
		{
			numRings--;
			//ringcountUI.sprite = ringcountUIArray[numRings];
			playerHealth.value = numRings;
			uiHandler.UpdateHealth(0);
		}
		else
		{
			modelEffect.OnDeath();
			playerHealth.value = 0;
			uiHandler.UpdateHealth(0);
			winState.PlayerTwoWins();
			//Destroy(gameObject);
		}
	}

	bool jumpCoolDown = false;

	public void JumpBoost(JumpPad jumpRef)
    {
		m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, 0, m_Rigidbody.velocity.z);

        if (!jumpCoolDown)
        {
			jumpCoolDown = true;
			StartCoroutine(Jumping(jumpRef));
		}
	}

	float curveTime = 0;
	float curveDuration = 1f;

	IEnumerator Jumping(JumpPad jumpRef)
    {
		m_Rigidbody.AddForce(transform.up * jumpRef.jumpStrength, ForceMode.Impulse);
		jumpRef.source.Play();
		yield return new WaitForSeconds(0.5f);
		jumpCoolDown = false;
	}

	public void SetRailConstrains()
	{
		if(lockRigidbodyRotation)
		//m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
		m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		railSteerSpeed = railSteerRef;
		isMoving = true;
		Invoke("SetRailMovement", 1);
	}

	void SetRailMovement()
    {
		hasStarted = true;
	}

	public void SetMaxPitchVal()
    {
		//minimumPitch = PlayerPrefs.GetFloat("Player1Lowest");
		//if (currentPitch > 25)
			maximumPitch = calMaxPitch;
		//maximumPitch = currentPitchValue;
	}

	public void SetMinPitchVal()
	{
		//if (currentPitch > 5 && currentPitch < 25)
			minimumPitch = calMinPitch;
		//maximumPitch = PlayerPrefs.GetFloat("Player1Highest");
	}

	public void ExitPipe()
    {
		boostTimer = 0;
    }
}
