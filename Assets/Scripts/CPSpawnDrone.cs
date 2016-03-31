using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CPSpawnDrone : MonoBehaviour {

	ControlPoint CP;
	public GameObject attackDrone;
	public float spawnRate;
	public int maxDrones;
	public List<GameObject> spawnedDrones_Team1;
	public List<GameObject> spawnedDrones_Team2;

	float elapsedSpawnDelay;

	void Start () {
		CP = GetComponent<ControlPoint> ();
	}

	void Update () {
		elapsedSpawnDelay += Time.deltaTime;
		if (elapsedSpawnDelay >= spawnRate) {
			elapsedSpawnDelay = 0f;

			int currTeam = whichTeam ();

			if ((currTeam == 1 && (spawnedDrones_Team1.Count) < maxDrones) 
				|| (currTeam == 2 && (spawnedDrones_Team2.Count) < maxDrones)) {
				spawnAttackDrone (currTeam);
			}
		}

		// REALLY HACKY CLEANUP, PLEASE CHANGE LATER
		for (int i = 0; i < spawnedDrones_Team1.Count; i++) {
			if (spawnedDrones_Team1 [i].GetComponent<PlayerStructure> ().currHealth <= 1) {
				Destroy (spawnedDrones_Team1 [i]);
				spawnedDrones_Team1.RemoveAt (i);
			}
		}
		for (int i = 0; i < spawnedDrones_Team2.Count; i++) {
			if (spawnedDrones_Team2 [i].GetComponent<PlayerStructure> ().currHealth <= 1) {
				Destroy (spawnedDrones_Team2 [i]);
				spawnedDrones_Team2.RemoveAt (i);
			}
		}
	}

	// Returns: 0 - Neutral, 1 - Team1, 2 - Team2
	int whichTeam() {
		// Team1
		if (CP.driftPoint == CP.captureAbsValue) {
			return 1;
		}
		// Team2
		else if (CP.driftPoint == (-1f * CP.captureAbsValue)) {
			return 2;
		}
		return 0;
	}

	void spawnAttackDrone(int teamNum) {
		GameObject aDrone = Instantiate(attackDrone);
		aDrone.GetComponent<AttackDroneController> ().teamNumber = teamNum;
		aDrone.transform.position = transform.position;

		if (teamNum == 1) {
			spawnedDrones_Team1.Add (aDrone);
		}
		else if (teamNum == 2) {
			spawnedDrones_Team2.Add (aDrone);
		}
	}

}
