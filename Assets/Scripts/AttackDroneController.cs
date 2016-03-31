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
	public float weaponMaxDistance = 50f; // See Weapon description.
	public float maxEnemyDistance = 10f;

	float currThrusterSpeed;

	[Header("AttackDrone: Dynamically Set Fields")]
	public float elapsedFireDelay;
	RangeFinder range;
	Transform mesh;
	Rigidbody RB;

	void Start () {
		range = transform.Find("Range").GetComponent<RangeFinder>();
		mesh = transform.Find ("Mesh1");
		elapsedFireDelay = 0f;
		RB = GetComponent<Rigidbody> ();

		GetComponent<PlayerStructure> ().teamNumber = teamNumber;

		transform.Find ("Range").GetComponent<SphereCollider> ().radius = maxEnemyDistance;
	}

	void Update () {
		// Should only happen once
		if (range.parentTeamNumber != teamNumber) {
			range.parentTeamNumber = teamNumber;
			range.inRange.Clear ();
		}

		spin ();

		// If there is something in range
		if (range.inRange.Count > 0) {
			rotateToFaceEnemy (range.inRange [0]);	
			thruster ();
			fireWeapon (range.inRange [0]);
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

			weaponComp.maxDistance = weaponMaxDistance;
			weaponComp.teamNumber = teamNumber;
		}
	}


}
