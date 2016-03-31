using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CPSpawnDrone : MonoBehaviour {

	ControlPoint CP;
	public GameObject attackDrone;
	public float spawnRate;
	public int maxDrones;
	public List<GameObject> spawnedDrones;

	float elapsedSpawnDelay;

	void Start () {
		CP = GetComponent<ControlPoint> ();
	}

	void Update () {
		elapsedSpawnDelay += Time.deltaTime;
		if (elapsedSpawnDelay >= spawnRate) {
			elapsedSpawnDelay = 0f;

			if (whichTeam () != 0 && (spawnedDrones.Count) < maxDrones) {
				spawnAttackDrone (whichTeam ());
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

		spawnedDrones.Add (aDrone);
	}

}
