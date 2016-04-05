using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnDrone : MonoBehaviour {

	public enum spawnType {CP = 0, Base};

	[Header("Inspector Set Fields")]
	public spawnType type;
	public GameObject attackDrone;
	public GameObject defenseDrone;
	public float spawnRate;
	public int maxDrones;

	public Material Team1Mat, Team2Mat;

	[Header("Dynamically Set Fields")]
	// :/ I need to figure out a smarter method
	// At this point, you have to set it at each CP
	public GameObject team1Base;
	public GameObject team2Base;

	ControlPoint CP;
	public List<GameObject> spawnedADrones_Team1;
	public List<GameObject> spawnedADrones_Team2;
	public List<GameObject> spawnedDDrones_Team1;
	public List<GameObject> spawnedDDrones_Team2;

	float elapsedSpawnDelay;

	void Start () {
		if (type == spawnType.CP) {
			CP = GetComponent<ControlPoint> ();
		}
		team1Base = GameManager.GM.base_team1;
		team2Base = GameManager.GM.base_team2;
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

		if (type == spawnType.CP) { 
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
		// if base
		else {
			return (GetComponent<BaseHealth> ().teamNumber);
		}
	}

	void spawnAttackDrone(Team teamNum) {
		GameObject aDrone = Instantiate(attackDrone);
		aDrone.GetComponent<AttackDroneController> ().teamNumber = teamNum;
		aDrone.GetComponent<AttackDroneController> ().CPSpawn = this.gameObject;
		aDrone.transform.position = transform.position;

		if (teamNum == Team.Team1) {
			aDrone.GetComponent<AttackDroneController> ().enemyBase = team2Base;
			spawnedADrones_Team1.Add (aDrone);

			//aDrone.transform.Find ("Mesh1").GetComponent<Renderer> ().materials [1] = Team1Mat;
			aDrone.transform.Find ("Mesh1").GetComponent<Renderer> ().material = Team1Mat;
		}
		else if (teamNum == Team.Team2) {
			aDrone.GetComponent<AttackDroneController> ().enemyBase = team1Base;
			spawnedADrones_Team2.Add (aDrone);

			//aDrone.transform.Find ("Mesh1").GetComponent<Renderer> ().materials [1] = Team2Mat;
			aDrone.transform.Find ("Mesh1").GetComponent<Renderer> ().material = Team2Mat;
		}
	}

	void spawnDefenseDrone(Team teamNum) {
		GameObject dDrone = Instantiate (defenseDrone);
		dDrone.GetComponent<DefenseDroneController> ().teamNumber = teamNum;
		dDrone.GetComponent<DefenseDroneController> ().CPSpawn = this.gameObject;
		dDrone.transform.position = transform.position;

		if (type == spawnType.Base) {
			dDrone.GetComponent<DefenseDroneController> ().radius = 6f;
		}

		if (teamNum == Team.Team1) {
			spawnedDDrones_Team1.Add (dDrone);

			//dDrone.transform.Find ("Mesh1").GetComponent<Renderer> ().materials [1] = Team1Mat;
			dDrone.transform.Find ("Mesh1").GetComponent<Renderer> ().material = Team1Mat;
		}
		else if (teamNum == Team.Team2) {
			spawnedDDrones_Team2.Add (dDrone);

			//dDrone.transform.Find ("Mesh1").GetComponent<Renderer> ().materials [1] = Team2Mat;
			dDrone.transform.Find ("Mesh1").GetComponent<Renderer> ().material = Team2Mat;
		}
	}
}
