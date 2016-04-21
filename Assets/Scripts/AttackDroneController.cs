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
	Transform mesh;
	Rigidbody RB;

	bool gotoBaseSet;

	void Start () {

		if (teamNumber == Team.Team1) {
			gameObject.layer = LayerMask.NameToLayer("BlueDrone");
			foreach (Transform tr in transform) {
				if (tr.gameObject.name == "Range")
					tr.gameObject.layer = LayerMask.NameToLayer("BlueWeapon");
				else
					tr.gameObject.layer = LayerMask.NameToLayer("BlueDrone");
			}
		}

		if (teamNumber == Team.Team2) {
			gameObject.layer = LayerMask.NameToLayer("RedDrone");
			foreach (Transform tr in transform) {
				if (tr.gameObject.name == "Range")
					tr.gameObject.layer = LayerMask.NameToLayer("RedWeapon");
				else
					tr.gameObject.layer = LayerMask.NameToLayer("RedDrone");
			}
		}

		mesh = transform.Find ("Mesh1");
		elapsedFireDelay = 0f;
		RB = GetComponent<Rigidbody> ();

		NMAgent = GetComponent<NavMeshAgent> ();

		gotoBase ();
		gotoBaseSet = false;
	}

	void Update () {

		spin ();

		// If there is something in range
		if (range.inRange.Count > 0) {
			NMAgent.Stop ();

			gotoBaseSet = false;

			rotateToFaceEnemy (GetTarget(range.inRange));	
			thruster ();
			fireWeapon (GetTarget(range.inRange));
		}
		// If the base is in range
		else if (NMAgent.remainingDistance <= maxEnemyDistance) {
				NMAgent.Stop ();
				gotoBaseSet = false;
				rotateToFaceEnemy (enemyBase);
				fireWeapon (enemyBase);
		}
		// Otherwise, just head to the base
		else if (!gotoBaseSet) {
			gotoBase ();
		}

	}

    protected override void DoOnDamage()
	{
		Instantiate (MusicMan.MM.damageSoundSource, transform.position, Quaternion.identity);
        Color tcolor = GameManager.GM.getTeamColor(teamNumber);
        ShipColor scolor = transform.Find("Mesh1").GetComponent<ShipColor>();
        if(scolor != null)
        {
            scolor.FlashColor(Color.Lerp(tcolor, Color.white, .75f), .1f);
        }
    }

    void spin() {
		mesh.Rotate (0f, spinSpeed * RB.velocity.magnitude, 0f);
	}

	void rotateToFaceEnemy(GameObject enemy) {
		if (enemy == null) return;
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
            if(MusicMan.MM.droneBulletSoundSource != null)
            {
                Instantiate(MusicMan.MM.droneBulletSoundSource, transform.position, Quaternion.identity);
            }

			weapon.transform.position = transform.position;

			// make bullets tiny lol
			weapon.transform.localScale = new Vector3 (0.3f, 0.3f, 0.3f);
			if (teamNumber == Team.Team1) {
				weapon.layer = LayerMask.NameToLayer ("BlueWeapon");
			} else if (teamNumber == Team.Team2) {
				weapon.layer = LayerMask.NameToLayer ("RedWeapon");
			}

			Weapon weaponComp = weapon.GetComponent<Weapon>();

			weaponComp.startingVelocity = transform.forward;
			weaponComp.startingVelocity.Normalize();
			weaponComp.originator = this;

			weaponComp.damagePower = 0.5f;
		}
	}

	// Deal with clean up after death
	void OnDestroy() {

        if(CPSpawn != null)
        {
            if (teamNumber == Team.Team1)
            {
                CPSpawn.GetComponent<SpawnDrone>().spawnedADrones_Team1.Remove(this.gameObject);
            }
            else if (teamNumber == Team.Team2)
            {
                CPSpawn.GetComponent<SpawnDrone>().spawnedADrones_Team2.Remove(this.gameObject);
            }
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
