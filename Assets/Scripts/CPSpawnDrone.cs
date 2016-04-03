using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CPSpawnDrone : MonoBehaviour {

	[Header("Inspector Set Fields")]
	public GameObject attackDrone;
	public float spawnRate;
	public int maxDrones;

	// :/ I need to figure out a smarter method
	// At this point, you have to set it at each CP
	public GameObject team1Base;
	public GameObject team2Base;

	[Header("Dynamically Set Fields")]
	ControlPoint CP;
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
		aDrone.GetComponent<AttackDroneController> ().CPSpawn = this.gameObject;
		aDrone.transform.position = transform.position;

		if (teamNum == 1) {
			aDrone.GetComponent<AttackDroneController> ().enemyBase = team2Base;
			spawnedDrones_Team1.Add (aDrone);
		}
		else if (teamNum == 2) {
			aDrone.GetComponent<AttackDroneController> ().enemyBase = team1Base;
			spawnedDrones_Team2.Add (aDrone);
		}
	}

}
