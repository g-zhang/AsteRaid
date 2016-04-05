using UnityEngine;
using System.Collections;

public class DefenseDroneController : AI {

	[Header("DefenseDrone: Inspector Set Fields")]
	public float spinSpeed;
	public float rotationSpeed;
	public float thrusterSpeed;
	public float drag;
	public GameObject weaponPrefab;
	public float delayBetweenShots = 0.5f;
	public float maxEnemyDistance = 10f;
	public float angleChange = 0.5f;
	public float angleOffset = 0.05f;

	public float radius = 3f;
	public float radiusOffset = 0.2f;

	float currThrusterSpeed;

	public LayerMask raycastMask;

	[Header("DefenseDrone: Dynamically Set Fields")]
	public GameObject CPSpawn;
	public GameObject enemyBase;
	public float currRadius;
	public float currAngle;

	public float elapsedFireDelay;
	public NavMeshAgent NMAgent;
	Transform mesh;
	Rigidbody RB;

	void Start () {
		mesh = transform.Find ("Mesh1");
		elapsedFireDelay = 0f;
		RB = GetComponent<Rigidbody> ();

		NMAgent = GetComponent<NavMeshAgent> ();

		transform.Find ("Range").GetComponent<SphereCollider> ().radius = maxEnemyDistance;
		currRadius = radius;
		currAngle = 0f;

		circleCP ();
	}

	void Update () {

		spin ();

		// If there is something in range
		if (range.inRange.Count > 0) {
			NMAgent.Stop ();

			rotateToFaceEnemy (range.inRange [0]);	
			thruster ();
			fireWeapon (range.inRange [0]);
		}
		// Otherwise, go around and do stuff;
		else {
			circleCP ();
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
		if (teamNumber == Team.Team1) {
			CPSpawn.GetComponent<CPSpawnDrone> ().spawnedDDrones_Team1.Remove (this.gameObject);
		} else if (teamNumber == Team.Team2) {
			CPSpawn.GetComponent<CPSpawnDrone> ().spawnedDDrones_Team2.Remove (this.gameObject);
		}
	}

	void circleCP() {
		NMAgent.Resume ();
		if (NMAgent.remainingDistance <= 0.1f) {
			Vector3 cpPos = CPSpawn.transform.position;
			float offsetX, offsetZ;
			float newAngle = currAngle + Random.Range(angleChange - angleOffset, angleChange + angleOffset);
			float newRadius = Random.Range (radius - radiusOffset, radius + radiusOffset);

			offsetX = newRadius * Mathf.Cos (newAngle);
			offsetZ = newRadius * Mathf.Sin (newAngle);

			Vector3 newPos = new Vector3 (cpPos.x + offsetX, transform.position.y, cpPos.z + offsetZ);

			NMAgent.SetDestination( newPos);

			currAngle = newAngle;
			currRadius = newRadius;
		}
	}
}
