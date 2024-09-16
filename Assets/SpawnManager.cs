using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {

	public List<GameObject> roads;
	public List<GameObject> rings;
	private float offset = 800f;
	private float ringOffset = 800f;

	void Start () {
	}
	
	public void MoveRoad(){
		GameObject movedRoad = roads [0];

		float newX = roads[0].transform.position.x + offset;
		movedRoad.transform.position = new Vector3 (newX, 0, 0);
		roads.Remove (movedRoad);
		roads.Add (movedRoad);
	}
	public void MoveRing(){
		for (int i = 0; i < 3; i++) {
			GameObject movedRing = rings [0];
			float newX = rings [0].transform.position.x + ringOffset;
			movedRing.transform.position = new Vector3 (newX, Random.Range(0f, 100f), 0);
			rings.Remove(movedRing);
			rings.Add (movedRing);
		}
	}
}
