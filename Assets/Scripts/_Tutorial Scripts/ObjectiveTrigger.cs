using UnityEngine;
using System.Collections;

public class ObjectiveTrigger : MonoBehaviour {
	public enum objectiveType {CapturePoint, MercTurret, LaserWall, BaseDamage};

	public objectiveType type;
	public Team team;
	public GameObject wall;

	void Start() {

	}

	// Update is called once per frame
	void Update () {
		// CapturePoint gets captured
		if (type == objectiveType.CapturePoint) {
			ControlPoint CP = GetComponent<ControlPoint> ();
			if ((team == Team.Team1 && CP.driftPoint == CP.captureAbsValue) ||
				(team == Team.Team2 && CP.driftPoint == (-1 * CP.captureAbsValue))) {
				wall.GetComponent<TutorialWall> ().destroyWall ();
			}
		}
		// MercTurrect
		else if (type == objectiveType.CapturePoint) {

		}
		// LaserWall
		else if (type == objectiveType.CapturePoint) {

		}
		// BaseDamage
		else if (type == objectiveType.CapturePoint) {

		}
	}
}
