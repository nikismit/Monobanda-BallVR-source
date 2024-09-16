using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRoads : MonoBehaviour {

	SpawnManager spawnManager;

	// Use this for initialization
	void Start () {
		spawnManager = GetComponent<SpawnManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SpawnTriggerEntered(){
		spawnManager.MoveRoad();
		spawnManager.MoveRing ();
	}
}
