using UnityEngine;
using System.Collections;

public class LaserWallController : MonoBehaviour {

	[Header("LaserWall: Inspector Set Fields")]
	public GameObject gTurret1;
	public GameObject gTurret2;
	public float enemyDamage;
	public float ejectForce = 1f;

	[Header("LaserWall: Dynamic Set Fields")]
	public Team teamNumber;

	void Start() {
		teamNumber = gTurret1.GetComponent<HealthSystem> ().teamNumber;
	}

	// Update is called once per frame
	void Update () {
		if (gTurret1 == null && gTurret2 == null) {
			Destroy (this);
		}
	}

	void OnTriggerEnter(Collider other) {
		Transform parent = other.transform;
		while (parent.parent != null)
		{
			parent = parent.parent;
		}

		HealthSystem otherHS = parent.GetComponent<HealthSystem> ();

		if (otherHS != null) {
			if (otherHS.teamNumber != teamNumber) {
				otherHS.currHealth -= 20f;
				if (otherHS.currHealth < 0f)
					otherHS.currHealth = 0f;
				
				parent.GetComponent<Rigidbody> ().AddForce (transform.forward * ejectForce);
			}
		} else
			print (")):");
	}
}
