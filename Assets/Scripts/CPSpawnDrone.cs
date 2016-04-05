using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CPSpawnDrone : MonoBehaviour {

	[Header("Inspector Set Fields")]
	public GameObject attackDrone;
	public GameObject defenseDrone;
	public float spawnRate;
	public int maxDrones;

	// :/ I need to figure out a smarter method
	// At this point, you have to set it at each CP
	public GameObject team1Base;
	public GameObject team2Base;

	[Header("Dynamically Set Fields")]
	ControlPoint CP;
	public List<GameObject> spawnedADrones_Team1;
	public List<GameObject> spawnedADrones_Team2;
	public List<GameObject> spawnedDDrones_Team1;
	public List<GameObject> spawnedDDrones_Team2;

	float elapsedSpawnDelay;

	void Start () {
		CP = GetComponent<ControlPoint> ();
	}

	void Update () {
		elapsedSpawnDelay += Time.deltaTime;
		if (elapsedSpawnDelay >= spawnRate) {
			elapsedSpawnDelay = 0f;

			Team currTeam = whichTeam ();

			if ((currTeam == Team.Team1 && (spawnedADrones_Team1.Count) < maxDrones) 
				|| (currTeam == Team.Team2 && (spawnedADrones_Team2.Count) < maxDrones)) {
				spawnAttackDrone (currTeam);
			}

			if ((currTeam == Team.Team1 && (spawnedDDrones_Team1.Count) < maxDrones) 
				|| (currTeam == Team.Team2 && (spawnedDDrones_Team2.Count) < maxDrones)) {
				spawnDefenseDrone (currTeam);
			}
		}
	}

	// Returns: 0 - Neutral, 1 - Team1, 2 - Team2
	Team whichTeam() {
		// Team1
		if (CP.driftPoint == CP.captureAbsValue) {
			return Team.Team1;
		}
		// Team2
		else if (CP.driftPoint == (-1f * CP.captureAbsValue)) {
			return Team.Team2;
		}
		return Team.Neutral;
	}

	void spawnAttackDrone(Team teamNum) {
		GameObject aDrone = Instantiate(attackDrone);
		aDrone.GetComponent<AttackDroneController> ().teamNumber = teamNum;
		aDrone.GetComponent<AttackDroneController> ().CPSpawn = this.gameObject;
		aDrone.transform.position = transform.position;

		if (teamNum == Team.Team1) {
			aDrone.GetComponent<AttackDroneController> ().enemyBase = team2Base;
			spawnedADrones_Team1.Add (aDrone);
		}
		else if (teamNum == Team.Team2) {
			aDrone.GetComponent<AttackDroneController> ().enemyBase = team1Base;
			spawnedADrones_Team2.Add (aDrone);
		}
	}

	void spawnDefenseDrone(Team teamNum) {
		GameObject dDrone = Instantiate (defenseDrone);
		dDrone.GetComponent<DefenseDroneController> ().teamNumber = teamNum;
		dDrone.GetComponent<DefenseDroneController> ().CPSpawn = this.gameObject;
		dDrone.transform.position = transform.position;

		if (teamNum == Team.Team1) {
			spawnedDDrones_Team1.Add (dDrone);
		}
		else if (teamNum == Team.Team2) {
			spawnedDDrones_Team2.Add (dDrone);
		}
	}
}
