using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioMovement : MonoBehaviour {

	[SerializeField] private AudioMovement otherPlayer;
	public Camera camera;
	public WinState winState;
	[SerializeField] ModelEffects modelEffect;
	[SerializeField] TrailRenderer trailEffect;
	[SerializeField] FlameEffect flameEffect;

	public Slider playerHealth;
	[SerializeField] PlayersUIHandler uiHandler;
	[SerializeField] private TutorialHandler tutHandler;

	private Transform camDist;
	[SerializeField] GameObject ui;

	[Header("Sound References")]
	public AudioPitch_Player1 pitch;
	public SFXManager sfx;
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

	//public float objectHeight;
	public float Volume;
	public float FOV;
	private float SavedFOV;
	public bool FovWanted;

	[SerializeField]private LayerMask layer;

	[Header("Options")]
	[SerializeField] int player;
	public float minimumPitch;
	public float maximumPitch;
	public int currentPitchValue;
	public bool soundTriggersParticles;
	public bool isSinglePlayer;
	public bool comingFromMainMenu;

	[Header("Debug")]
	public bool debugKeyControl;
	[SerializeField] bool removeHealth;
	private bool testRailControl = true;
	private bool PitchSliderMovement = true;
	private Vector3 sliderVector;
	//[Range(1f, 50f)]
	private float railSteerSpeed = 15;
	[HideInInspector] public float railSteerRef;
	[Space]
	private bool ignorePlayerCol = true;
	private bool setCrashCol = true;
	private bool lockRigidbodyRotation = true;

	//[Header("Debug CarRail movement")]
	private float roadWidth = 60;
	private float roadHalf;
	private float railSpeed = 2400;
	float sliderPitchInvLerp;

	private Vector3 sliderPos;
	private Vector3 velocity = Vector3.zero;
	[HideInInspector] public float lastValidPitch;

	[HideInInspector] public bool hasStarted = false;
	[HideInInspector] public bool isMoving = false;

	private float boostTimer = 100;

	private int numRings = 5;

	private float invulnerableState = 4;
	private float minimumAmp = -50;

	//Make sure you attach a Rigidbody in the Inspector of this GameObject
	Rigidbody m_Rigidbody;
	ParticleSystem _partSys;

	[Header("BoostParticleReferences")]
	[SerializeField] ParticleSystem[] boostParticle;
	private Collider col;

	float lastHit;

	private float boostAmount = 5;
	float trailTime = 0;

	[HideInInspector] public bool isInPipe;
	[HideInInspector] public bool removeControl = true;

	[SerializeField] ParticleSystem ringEmmitterParticle;

	void Start()
    {
		ringEmmitterParticle.Stop();

		if (tutHandler.androidDebug && player == 1)
		{
			gameObject.SetActive(false);
		}

		m_Rigidbody = GetComponent<Rigidbody>();
		_partSys = GetComponent<ParticleSystem>();

		col = gameObject.GetComponent<Collider>();
		feedBack = gameObject.GetComponent<PlayerFeedBack>();
		camDist = GameObject.FindGameObjectWithTag("CarDistRef").transform;


		lastHit = invulnerableState + 1;
		if (!ui.activeSelf && ui != null)
		{
			ui.SetActive(true);
		}
		else
			Debug.LogWarning("ui is null! please add a ui ref.");
		playerHealth.maxValue = 5;
		playerHealth.value = 5;

		FOV = 60.0f;
		SavedFOV = FOV;
		railSteerRef = railSteerSpeed;

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
			m_Rigidbody.AddForce(Vector3.forward * (currentTurn - otherPlayer.currentTurn) * 20, ForceMode.Impulse);
			m_Rigidbody.drag = 1;
        }
		else if(collision.gameObject.tag == "Track" && !removeHealth)
		{
			Vector3 colReflect = Vector3.Reflect(transform.forward, collision.contacts[0].normal);


			Physics.IgnoreCollision(collision.collider, col, true);

			if (lastHit > invulnerableState)
            {
				StartCoroutine(InvulnerableEnum());
				RemoveRing();
				sfx.crashIntoTrack();
			}

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
		if (other.gameObject.tag == "Ring" && numRings > 0)
		{
            for (int i = 0; i < boostParticle.Length; i++)
            {
				boostParticle[i].Play();
			}

			StartCoroutine(flameEffect.InitiateBoostEffect());
			modelEffect.Squash(new Vector3(0.6f, 0.7f, 1.20f));
			uiHandler.UpdateScore(100, player);

			if (other.gameObject.GetComponent<StrongerBoosterRing>())
            {
				//float boost = other.GetComponent<StringerBoosterRing>().BoostAmount;
				boostAmount = other.GetComponent<StrongerBoosterRing>().BoostAmount;

				m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, 5, m_Rigidbody.velocity.z);
			}


			if (numRings < 5)
			{
				numRings++;
				playerHealth.value = numRings;
			}

			transform.rotation = other.transform.rotation;
			boostTimer = 0;
		}
		if (other.gameObject.GetComponent<JumpPad>())
        {
			modelEffect.Squash(new Vector3(0.7f, 1.2f, 0.7f));
			JumpPad jumpPadRef = other.gameObject.GetComponent<JumpPad>();
			JumpBoost(jumpPadRef);
		}
	}

    void FixedUpdate()
    {
		if (!IsGrounded())
			Invoke("EnableSquash", 1f);
		else if (IsGrounded() && hasStarted)
			CancelInvoke();

		if (otherPlayer != null)
		{
			float colDist = Vector3.Distance(transform.position, otherPlayer.transform.position);

			if (colDist < 2 && transform.position.z >= -29 && transform.position.z <= 29)
			{
				Vector3 currentDirection = (transform.position - otherPlayer.transform.position).normalized;

				float PushAmount = Mathf.InverseLerp(0, 1, colDist);
				m_Rigidbody.AddForce(transform.right * -currentDirection.z * PushAmount * 60);
				//m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
			}
			else if (transform.position.z <= -29 || transform.position.z >= 29)
			{
				m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_Rigidbody.velocity.y, 0);
				m_Rigidbody.angularVelocity = new Vector3(m_Rigidbody.velocity.x, m_Rigidbody.velocity.y, 0);
			}
		}

		if (isMoving)
        {
			currentSpeed = maximumForwardSpeed;

			if (boostTimer < 1)
			{
				boostTimer += Time.deltaTime;
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

		if (!removeControl)
			currentAmp = pitch._currentPublicAmplitude;
		else
			currentAmp = 0;

		Volume = pitch._currentPublicAmplitude;

        if (!freezeCarSteer)
        {
			if (currentPitch < minimumPitch)
			{
				sliderPitchInvLerp = Mathf.InverseLerp(maximumPitch, minimumPitch, minimumPitch);
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

			sliderVector = new Vector3(transform.position.x, transform.position.y, Mathf.Clamp(sliderPitchInvLerp * roadWidth - roadHalf, -roadHalf, roadHalf));

			if (currentAmp < minimumAmp || minimumAmp == -Mathf.Infinity)
			{
				minimumAmp = currentAmp;
			}

			if (currentAmp > minimumAmp + 20 && !debugKeyControl)
			{
				sliderPos = Vector3.SmoothDamp(transform.position, sliderVector, ref velocity, 1, railSpeed * Time.deltaTime);

				modelEffect.dir = currentTurn;
			}
			else
			{
				float slowStop = Mathf.InverseLerp(railSpeed, 0, 10 * Time.deltaTime);
				sliderPos = Vector3.SmoothDamp(transform.position, sliderVector, ref velocity, 1, slowStop);
			}
		}
		else
			sliderPos = transform.position;

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
		}

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

    #region Variables
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
    #endregion

    #region CarKeyControls
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
    #endregion
    #region CarSoundControls
    void CarSoundMovement()
    {
		var emission = _partSys.emission;

		if (currentAmp > pitch.minVolumeDB)
		{
			if(!isMoving || pitch._currentPublicAmplitude <= -60)
            {
				ringEmmitterParticle.maxParticles = 0;
			}
            else
            {
				ringEmmitterParticle.maxParticles = 15;
				ringEmmitterParticle.startSize = Mathf.Clamp(currentPitch, 12, 20);
			}


			if (trailTime <= 1)
			{
				trailTime += 0.05f;
			}

			if (currentPitch > minimumPitch)
			{
				currentTurn = (((currentPitch - minimumPitch) / (maximumPitch - minimumPitch)) * 2) - 1 - 0.3f;
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
    #endregion


    void RampControl(Transform trans)
    {
		Vector3 gravityUp = trans.up;

		transform.up = Vector3.Lerp(transform.up, gravityUp, 20 * Time.deltaTime);
		transform.rotation = Quaternion.Euler(-transform.rotation.eulerAngles.z, 90, 0);
	}

	bool hasLost = false;

	public void RemoveRing()
    {
		feedBack.HitFeedBack();

		if (numRings > 1)
		{
			numRings--;
			playerHealth.value = numRings;
			uiHandler.UpdateHealth(0);
		}
		else if (!hasLost)
		{
			removeControl = true;
			hasLost = true;
			modelEffect.OnDeath();
			playerHealth.value = 0;
			uiHandler.UpdateHealth(0);
			winState.PlayerTwoWins();
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

	IEnumerator InvulnerableEnum()
	{
		lastHit = 0;
		while (lastHit < invulnerableState)
        {
			lastHit += Time.deltaTime;
			feedBack.BlinkFeedBack(true);
			yield return null;
		}
			lastHit = invulnerableState + 1;
			feedBack.BlinkFeedBack(false);
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
		//if(lockRigidbodyRotation)
		m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		railSteerSpeed = railSteerRef;
		isMoving = true;
		Invoke("SetRailMovement", 1);
		ringEmmitterParticle.Play();
	}

	//<---EVENTS--->
	void EnableSquash()
	{
		squashAble = true;
	}

	void SetRailMovement()
    {
		hasStarted = true;
	}

	bool freezeCarSteer = false;

	public void SetPitchVal(int val)
    {
		if(val == 0)
			maximumPitch = currentPitch;
        else
        {
			minimumPitch = currentPitch;
			Invoke("RemoveConstrains", 3);
		}
	}

	void RemoveConstrains()
    {
		freezeCarSteer = false;
	}

	public void ExitPipe()
    {
		boostTimer = 0;
    }
}
