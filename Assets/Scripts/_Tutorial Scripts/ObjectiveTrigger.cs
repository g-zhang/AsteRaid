using UnityEngine;
using System.Collections;

public class ObjectiveTrigger : MonoBehaviour {
	public enum objectiveType {CapturePoint, MercTurret, LaserWall, BaseDamage};

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

			// LaserWall gets destroyed
			else if (type == objectiveType.LaserWall) {

			}

			// Base gets damaged
			else if (type == objectiveType.BaseDamage) {

			}
		}
	}

	void completeObjective () {
		wall.GetComponent<TutorialWall> ().destroyWall ();
		objComplete = true;
	}
}
