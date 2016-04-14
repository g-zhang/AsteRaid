using UnityEngine;
using System.Collections;

public class LaserWallController : MonoBehaviour {

	[Header("LaserWall: Inspector Set Fields")]
	public GameObject gTurret1;
	public GameObject gTurret2;
	public float enemyDamage;
	public float ejectForce = 1f;
	public float ejectStunTime = 0.8f;

	[Header("LaserWall: Dynamic Set Fields")]
	public Team teamNumber;

	void Start() {
		teamNumber = gTurret1.GetComponent<HealthSystem> ().teamNumber;

		Color teamColor = Color.green;
		if (teamNumber == Team.Team1) {
			teamColor = GameManager.GM.teamColors [(int)Team.Team1];
			teamColor.a = 0.5f;
		}
		if (teamNumber == Team.Team2) {
			teamColor = GameManager.GM.teamColors [(int)Team.Team2];
			teamColor.a = 0.5f;
		}
		GetComponent<Renderer> ().material.color = teamColor;
	}

	// Update is called once per frame
	void Update () {
		if (gTurret1 == null && gTurret2 == null) {
			Destroy (this.gameObject);
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other == null) return;

		Transform parent = other.transform;
		while (parent.parent != null)
		{
			parent = parent.parent;
		}
			
		foreach (Transform tr in parent.transform) {
			foreach (Weapon w in tr.GetComponentsInChildren<Weapon>())
				if (w is Weapon_LaserBeam) return;
		}

		HealthSystem otherHS = parent.GetComponent<HealthSystem> ();
		if (parent.GetComponent<ShipControls>() == null) return; 

		if (otherHS != null) {

			if (otherHS.teamNumber != teamNumber && parent.GetComponent<ShipControls>().remainingStunTime <= 0f) {
				otherHS.currHealth -= enemyDamage;
                parent.GetComponent<Controls>().VibrateFor(1f, .5f);
				parent.GetComponent<ShipControls> ().remainingStunTime = ejectStunTime;
				parent.GetComponent<Rigidbody> ().AddForce (transform.forward * ejectForce, ForceMode.Impulse);
			}
		}
	}
}
