using UnityEngine;
using System.Collections.Generic;

public class SpawnDrone : MonoBehaviour {

	public enum spawnType {CP = 0, Base};

	[Header("Inspector Set Fields")]
	public spawnType type;
	public GameObject attackDrone;
	public GameObject defenseDrone;

	public float attackSpawnRate = 5;
	public float defenseSpawnRate = 5;
	public int attackMaxDrones = 3;
	public int defenseMaxDrones = 3;

	public Vector3 attackRandomSpawnOffset = Vector3.zero;

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

	float attackElapsedSpawnDelay;
	float defenseElapsedSpawnDelay;

	void Start () {
		if (type == spawnType.CP) {
			CP = GetComponent<ControlPoint> ();
		}
		team1Base = GameManager.GM.base_team1;
		team2Base = GameManager.GM.base_team2;
	}

	void Update()
	{
		if ((type != spawnType.Base) && (CP.captureSpectrum != CP.driftPoint))
		{
			return;
		}

		AttackDroneUpdate();
		DefenseDroneUpdate();

		return;
	}

	private void AttackDroneUpdate()
	{
		attackElapsedSpawnDelay += Time.deltaTime;
		if (attackElapsedSpawnDelay < attackSpawnRate)
		{
			return;
		}
		attackElapsedSpawnDelay -= attackSpawnRate;

		Team currTeam = whichTeam();
		if ((currTeam == Team.Team1) && (spawnedADrones_Team1.Count < attackMaxDrones))
		{
			spawnAttackDrone(currTeam);
		}
		else if ((currTeam == Team.Team2) && (spawnedADrones_Team2.Count < attackMaxDrones))
		{
			spawnAttackDrone(currTeam);
		}

		return;
	}

	private void DefenseDroneUpdate()
	{
		defenseElapsedSpawnDelay += Time.deltaTime;
		if (defenseElapsedSpawnDelay < defenseSpawnRate)
		{
			return;
		}
		defenseElapsedSpawnDelay -= defenseSpawnRate;

		Team currTeam = whichTeam();
		if ((currTeam == Team.Team1) && (spawnedDDrones_Team1.Count < defenseMaxDrones))
		{
			spawnDefenseDrone(currTeam);
		}
		else if ((currTeam == Team.Team2) && (spawnedDDrones_Team2.Count < defenseMaxDrones))
		{
			spawnDefenseDrone(currTeam);
		}

		return;
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
		return (GetComponent<BaseHealth> ().teamNumber);
	}

	void spawnAttackDrone(Team teamNum) {
		Vector3 pos = Vector3.Lerp(transform.position + attackRandomSpawnOffset,
			transform.position - attackRandomSpawnOffset, Random.value);

		GameObject aDrone = Instantiate(attackDrone, pos, Quaternion.identity) as GameObject;
		aDrone.GetComponent<AttackDroneController> ().teamNumber = teamNum;
		aDrone.GetComponent<AttackDroneController> ().CPSpawn = this.gameObject;

		if (teamNum == Team.Team1) {
			aDrone.GetComponent<AttackDroneController> ().enemyBase = team2Base;
			spawnedADrones_Team1.Add (aDrone);
			aDrone.transform.Find ("Mesh1").GetComponent<Renderer> ().material = Team1Mat;
		}
		else if (teamNum == Team.Team2) {
			aDrone.GetComponent<AttackDroneController> ().enemyBase = team1Base;
			spawnedADrones_Team2.Add (aDrone);
			aDrone.transform.Find ("Mesh1").GetComponent<Renderer> ().material = Team2Mat;
		}
	}

	void spawnDefenseDrone(Team teamNum) {
		GameObject dDrone = Instantiate (defenseDrone, transform.position, Quaternion.identity) as GameObject;
		dDrone.GetComponent<DefenseDroneController> ().teamNumber = teamNum;
		dDrone.GetComponent<DefenseDroneController> ().CPSpawn = this.gameObject;

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
