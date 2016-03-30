using UnityEngine;
using System.Collections;

public enum RotationMode {
	PointAtMouse,
	PointAlongStick,
	YawOnStick,
	YawOnYawAxis,
	PointInVelocity
}

public class Swivel : MonoBehaviour {

	public Quaternion rotation = Quaternion.identity;
	public RotationMode swivelMode = RotationMode.PointInVelocity;
	public float rotationLerpSpeed = 60f;

	public void RotateBy(float degrees) {
		Vector3 angles = transform.rotation.eulerAngles;
		angles.y += degrees;
		rotation = Quaternion.Euler(angles);
	}

	public void PointAlongVector(Vector3 heading) {
		if (heading.magnitude == 0f) return;
		Quaternion targetRotation = Quaternion.LookRotation(heading, Vector3.up);
		rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationLerpSpeed);
	}

}
