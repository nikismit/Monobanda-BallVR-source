using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBalls : MonoBehaviour {

	[Header("Ball Options")]
    
    public GameObject _ballPrefab;
    public Material _ballMaterial;
    public PhysicMaterial _ballPhysicMaterial;
	public int _ballPoolAmount;
    public bool _growPool;
    List<GameObject> _balls;
    List<Material> _lMaterial;
    List<PhysicMaterial> _lPhysicMaterial;

	public Transform _spawnLocation;

    // ball size
    public float _growTimeMax;
    public Vector2 _ballsizeMinMax;
    public Vector2 _ballBounceMinMax;
	public float _forceAdd;
	public bool _bounceBasedOnPitch;
	public bool _massMultiplyBySize;


    private GameObject _currentBall;
    private Material _currentMaterial;
    private Color _currentColor;
    private int _currentItem;
    private Rigidbody _currentRigidbody;
    private SphereCollider _currentSphereCollider;

    //microphone variables
	[Header("Mic Options")]
	public float _minPitch;
	public float _maxPitch;
	public float _maxRegisteredAmplitude;
    private float _micPitch;
    private float _micAmplitude;


    private float _timeRecording;
    private bool _isSpeaking;

    
    private float _ballSizeCurrent;

    
    
    private float _highestAmplitude;

    
    


    // Use this for initialization
    void Start () {

        //pooling balls
        _balls = new List<GameObject>();
        _lMaterial = new List<Material>();
        _lPhysicMaterial = new List<PhysicMaterial>();

        for (int i = 0; i < _ballPoolAmount; i++)
        {
            GameObject obj = (GameObject)Instantiate(_ballPrefab);
            Material mat = new Material(_ballMaterial);
            PhysicMaterial physicmat = new PhysicMaterial(i.ToString()); 

            obj.GetComponent<Renderer>().material = mat;
            obj.GetComponent<SphereCollider>().material = physicmat;
            obj.SetActive(false);
            _balls.Add(obj);
            _lMaterial.Add(mat);
            _lPhysicMaterial.Add(physicmat);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        //pitch

        _micPitch =  Mathf.Clamp01(((Mathf.Clamp((float)VoiceProfile._voicePitch, _minPitch, _maxPitch))-_minPitch) / (_maxPitch - _minPitch));




        _micAmplitude = VoiceProfile._amplitudeCurrent;
   
        if ((_micAmplitude >= VoiceProfile._amplitudeSilence) && (!_isSpeaking)) //start speaking SPAWN
        {
            _currentColor = new Color(0, 0, 0, 1);
            _isSpeaking = true;
            _currentBall = GetPooledBall();
            _currentMaterial = _currentBall.GetComponent<Renderer>().material;
            _currentRigidbody = _currentBall.GetComponent<Rigidbody>();
            _currentSphereCollider = _currentBall.GetComponent<SphereCollider>();
            _currentMaterial.SetColor("_Color", _currentColor);
            _currentBall.transform.position = _spawnLocation.position;
            _currentRigidbody.isKinematic = true;
        }

        if ((_micAmplitude < VoiceProfile._amplitudeSilence) && (_isSpeaking)) //stop speaking RELEASE
        {
            _currentRigidbody.isKinematic = false;
            _isSpeaking = false;
            _timeRecording = 0;
            _highestAmplitude = Mathf.Clamp(_highestAmplitude, 0, _maxRegisteredAmplitude);
            _currentRigidbody.AddForce(this.transform.forward * _forceAdd * _highestAmplitude);
            _highestAmplitude = 0;
        }

        if (_isSpeaking) //WHILE speaking
        {
            if (_micAmplitude > _highestAmplitude)
            {
                _highestAmplitude = _micAmplitude;
            }
            _currentBall.transform.position = _spawnLocation.position;
            _timeRecording += Time.deltaTime;
           
            _ballSizeCurrent = Mathf.Lerp(_ballsizeMinMax.x, _ballsizeMinMax.y, Mathf.Clamp01(_timeRecording / _growTimeMax));
            _currentBall.transform.localScale = new Vector3(_ballSizeCurrent, _ballSizeCurrent, _ballSizeCurrent);

			bool greenDown = false;
			float redGreen = 1.0f;
			float greenBlue = 0.0f;

			if(_micPitch >= 0 && _micPitch <=0.5f){
				greenDown = true;
				redGreen = _micPitch*2;
				greenBlue = 0;
			} else if (_micPitch > 0.5f && _micPitch <= 1.0f){
				greenDown = false;
				greenBlue = (_micPitch - 0.5f)*2;
				redGreen = 0;
			}
			if(greenDown){
				_currentColor = new Color(1 - redGreen, redGreen, 0, 1);
			} else {
				_currentColor = new Color(0, 1-greenBlue, greenBlue, 1);
			}
            _currentMaterial.SetColor("_Color", _currentColor);
			_currentMaterial.SetColor("_EmissionColor", _currentColor);

            if (_massMultiplyBySize)
            {
                _currentRigidbody.mass = (1 - _micPitch) * _ballSizeCurrent;
            }
            else
            {
                _currentRigidbody.mass = (1 - _micPitch);
            }

            if (_bounceBasedOnPitch)
            {
                _currentSphereCollider.material.bounciness = Mathf.Lerp(_ballBounceMinMax.x, _ballBounceMinMax.y, _micPitch);
            }
            else
            {
                _currentSphereCollider.material.bounciness = 1;
            }
            _currentSphereCollider.material.bounceCombine = PhysicMaterialCombine.Multiply;
        }
    }


    GameObject GetPooledBall()
    {
        for (int i = 0; i < _balls.Count; i++)
        {
            if (!_balls[i].activeInHierarchy)
            {
                _balls[i].SetActive(true);
                _currentItem = i;
                return _balls[i];
            }
        }
        

        if (_growPool)
        {
            GameObject obj = (GameObject)Instantiate(_ballPrefab);
            _balls.Add(obj);
            _currentItem = _balls.Count - 1;
            return obj;

        }

        // we kunnen hier ook maken dat een eerdere bal wordt weggehaald inplaats van de nieuwe bal
        return null;

    }


}
