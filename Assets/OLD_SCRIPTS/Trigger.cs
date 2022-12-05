using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour {

	public SpawnRoads spawnRoads; 
	public int Score;

	// Use this for initialization
	void Start () {
		Score = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnTriggerEnter(Collider other){
		if (other.gameObject.CompareTag ("Finish")) {
			spawnRoads.SpawnTriggerEntered ();
		}
		//if (other.gameObject.CompareTag ("Ring")) {
		//	Score += 1;
		//	Debug.Log (Score);
		//}
	}
	void OnTriggerStay(Collider other)
	{
		if (other.gameObject.CompareTag ("Ring")) {
			ScoreScript.scoreValue += 1;
			//Score += 1;
			//Debug.Log (Score);
			Debug.Log(ScoreScript.scoreValue);
		}
	}
}
