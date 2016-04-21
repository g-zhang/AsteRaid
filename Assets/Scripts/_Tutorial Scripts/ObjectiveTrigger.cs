using UnityEngine;
using System.Collections;

public class ObjectiveTrigger : MonoBehaviour {
	public enum objectiveType {CapturePoint, MercTurret, BaseDamage};

	public objectiveType type;
	public Team team;
	public GameObject wall;

	bool objComplete;

	void Start() {
		objComplete = false;
	}

	// Update is called once per frame
	void Update () {
		if (!objComplete) {
			
			// CapturePoint gets captured
			if (type == objectiveType.CapturePoint) {
				ControlPoint CP = GetComponent<ControlPoint> ();
				if ((team == Team.Team1 && CP.driftPoint == CP.captureAbsValue) ||
				   (team == Team.Team2 && CP.driftPoint == (-1 * CP.captureAbsValue))) {
					completeObjective ();
				}
			}

			// MercTurrect gets changed
			else if (type == objectiveType.MercTurret) {
				AI_Turret turret = GetComponent<AI_Turret> ();
				if (turret.teamNumber == team) {
					completeObjective ();
				}
			}
		}
	}

	void completeObjective () {
		wall.GetComponent<TutorialWall> ().destroyWall ();
		objComplete = true;
	}

	void OnTriggerEnter(Collider other) {
		if (!objComplete && type == objectiveType.BaseDamage) {
			if (other.gameObject.GetComponent<Weapon> ().originator.gameObject.GetComponent<Player> () != null) {
				other.gameObject.GetComponent<Weapon> ().originator.currHealth = 0;
				for (int i = 0; i < GameManager.NUM_PLAYERS; i++) {
					if (!TutorialManager.TM.playerReady[i] &&
						TutorialManager.TM.players [i] == other.gameObject.GetComponent<Weapon> ().originator.gameObject) {
						TutorialManager.TM.playerReady [i] = true;
					}
				}
			}
		}
	}
}
