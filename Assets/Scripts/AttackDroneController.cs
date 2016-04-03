using UnityEngine;
using System.Collections;

public class AttackDroneController : AI {

	[Header("AttackDrone: Inspector Set Fields")]
	public float spinSpeed;
	public float rotationSpeed;
	public float thrusterSpeed;
	public float drag;
	public GameObject weaponPrefab;
	public float delayBetweenShots = 0.5f;
	public float maxEnemyDistance = 10f;

	float currThrusterSpeed;

	public LayerMask raycastMask;

	[Header("AttackDrone: Dynamically Set Fields")]
	public GameObject CPSpawn;
	public GameObject enemyBase;

	public float elapsedFireDelay;
	public NavMeshAgent NMAgent;
	RangeFinder range;
	Transform mesh;
	Rigidbody RB;

	bool gotoBaseSet;

	void Start () {
		range = transform.Find("Range").GetComponent<RangeFinder>();
		mesh = transform.Find ("Mesh1");
		elapsedFireDelay = 0f;
		RB = GetComponent<Rigidbody> ();

		NMAgent = GetComponent<NavMeshAgent> ();

		transform.Find ("Range").GetComponent<SphereCollider> ().radius = maxEnemyDistance;

		gotoBase ();
		gotoBaseSet = false;
	}

	void Update () {
		// Should only happen once
		if (range.parentTeamNumber != teamNumber) {
			range.parentTeamNumber = teamNumber;
			range.inRange.Clear ();
		}

		// NOT PROPER PLACE
		if (GetComponent<HealthSystem> ().teamNumber != teamNumber) {
			GetComponent<HealthSystem> ().teamNumber = teamNumber;	
			gotoBaseSet = false;
		}

		spin ();

		// If there is something in range
		if (range.inRange.Count > 0) {
			NMAgent.Stop ();

			gotoBaseSet = false;

			rotateToFaceEnemy (range.inRange [0]);	
			thruster ();
			fireWeapon (range.inRange [0]);
		}
		// Otherwise, go around and do stuff;
		else {
			if (NMAgent.remainingDistance <= maxEnemyDistance) {

				NMAgent.Stop ();

				gotoBaseSet = false;

				rotateToFaceEnemy (enemyBase);

				fireWeapon (enemyBase);
			}
			else {
				if (!gotoBaseSet) {
					gotoBase ();
				}
			}
		}
	}

	void spin() {
		mesh.Rotate (0f, spinSpeed * RB.velocity.magnitude, 0f);
	}

	void rotateToFaceEnemy(GameObject enemy) {
		Vector3 direction =
			enemy.transform.position - transform.position;

		currThrusterSpeed = direction.magnitude / thrusterSpeed;

		Quaternion targetRotation =
			Quaternion.LookRotation(direction, Vector3.up);

		transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, rotationSpeed);
	}

	void thruster() {
		RB.velocity = RB.velocity * (1 - drag);
		RB.AddForce (transform.forward * currThrusterSpeed);
	}

	void fireWeapon(GameObject enemy) {
		elapsedFireDelay += Time.deltaTime;
		if (elapsedFireDelay >= delayBetweenShots)
		{
			elapsedFireDelay = 0f;

			GameObject weapon = Instantiate(weaponPrefab);
			weapon.transform.position = transform.position;

			// make bullets tiny lol
			weapon.transform.localScale = new Vector3 (0.15f, 0.15f, 0.15f);

			Weapon weaponComp = weapon.GetComponent<Weapon>();

			weaponComp.startingVelocity = transform.forward;
			weaponComp.startingVelocity.Normalize();

			weaponComp.maxDistance = maxEnemyDistance;
			weaponComp.teamNumber = teamNumber;
		}
	}

	// Deal with clean up after death
	void OnDestroy() {
		if (teamNumber == 1) {
			CPSpawn.GetComponent<CPSpawnDrone> ().spawnedDrones_Team1.Remove (this.gameObject);
		} else if (teamNumber == 2) {
			CPSpawn.GetComponent<CPSpawnDrone> ().spawnedDrones_Team2.Remove (this.gameObject);
		}
	}

	void gotoBase() {
		Vector3 enemyBaseDest = enemyBase.transform.position;
		Ray rayToEnemyBase = new Ray (transform.position, enemyBaseDest);
		RaycastHit enemyBaseHitInfo;

		if (Physics.Raycast (rayToEnemyBase, out enemyBaseHitInfo, Mathf.Infinity, raycastMask)) {
			enemyBaseDest = enemyBaseHitInfo.point;
		}

		enemyBaseDest = new Vector3 (enemyBaseDest.x, NMAgent.transform.position.y - 0.75f, enemyBaseDest.z);

		Debug.DrawRay (enemyBaseDest, Vector3.up);

		NMAgent.SetDestination (enemyBaseDest);
		gotoBaseSet = true;

		NMAgent.Resume ();
	}
}
