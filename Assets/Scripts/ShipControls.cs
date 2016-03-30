using UnityEngine;
using System.Collections;


public enum RotationMode {
	PointAtMouse,
	PointAlongMoveStick,
	PointAlongAimStick,
	YawOnMoveStick,
	YawOnYawAxis,
	PointInVelocity
}

public enum TranslaionMode {

}

[RequireComponent(typeof(Controls))]
[RequireComponent(typeof(Rigidbody))]
public class ShipControls : MonoBehaviour {

	[Header("Components")]
	private Controls controls;
	private Rigidbody rigid;
	private Transform turret;

	[Header("Ship Parameters")]
	public bool localShipControls = false;
	public bool strafeControls = false;
	public bool rotationIndependent = false;
	public float minSpeed = 0f;
	public float maxSpeed = 5f;
	public float shipRotationSpeed = 12f;
	public float shipYawSpeed = 120f;
	public bool shipFaceMouse = false;

	// TODOs:
	// reverse max speed indep of forward max speed
	// ship accceleration parameterized
	// move to enums for translation and rotation modes
	// split to turret / ship classes (rotator class?)

	[Header("Turret Parameters")]
	public bool localTurretControls = false;
	public float turretConeHalfAngle = 180f;
	public float turretRotationSpeed = 60f;
	public bool turretFloatsHome = false;
	public float turretReturnSpeed = 30f;
	public bool turretFaceMouse = false;
	
	private Vector3 aimControl;
	private Vector3 moveControl;
	private float turretTargetAngle;
	private Quaternion turretRotation = Quaternion.identity;
	private Quaternion shipRotation = Quaternion.identity;

	void Awake() { 
		controls = GetComponent<Controls>();
		rigid = GetComponent<Rigidbody>();
		turret = transform.Find("Turret");
		if (turret == null) print("No Turret Child!");
	}

	void Update () {
		aimControl = new Vector3(controls.AimStick.x, 0f, controls.AimStick.y);
		moveControl = new Vector3(controls.MoveStick.x, 0f, controls.MoveStick.y);
		SetShipRotation();
		SetTurretRotation();
	}

	private Quaternion currentRotation {
		get {
			return localShipControls ? transform.localRotation : transform.rotation;
		}
	}

	void SetShipRotation() {
		if (shipFaceMouse) PointAlongVector(controls.ToMouseVector);
		else if (rotationIndependent) RotateShipBy(Time.deltaTime * shipYawSpeed * controls.YawAxis);
		else if (localShipControls) RotateShipBy(Time.deltaTime * shipYawSpeed * controls.MoveStick.x);
		else PointAlongVector(rigid.velocity.normalized);
	}

	void RotateShipBy(float degrees) {
		Vector3 angles = currentRotation.eulerAngles;
		angles.y += degrees;
		shipRotation = Quaternion.Euler(angles);
	}

	void PointAlongVector(Vector3 heading) {
		if (heading.magnitude == 0f) return;
		Quaternion targetRotation = Quaternion.LookRotation(heading, Vector3.up);
		shipRotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime * shipRotationSpeed);
	}

	void SetTurretRotation() {

		if (turretFaceMouse) {
			if (controls.ToMouseVector.magnitude == 0f) return;
			Quaternion lookRotation = Quaternion.LookRotation(controls.ToMouseVector, Vector3.up);
			turretRotation = Quaternion.Slerp(turret.rotation, lookRotation, Time.deltaTime * shipRotationSpeed);
			return;
		}

		if (controls.AimStick.magnitude > 0f) turretTargetAngle = Vector3.Angle(Vector3.forward, aimControl);
		else if (turretFloatsHome) turretTargetAngle = 0f;

		if (turretTargetAngle > turretConeHalfAngle) turretTargetAngle = turretConeHalfAngle;
		if (controls.AimStick.x < 0) turretTargetAngle = -turretTargetAngle;
		Quaternion turretTargetRotation = Quaternion.Euler(0f, turretTargetAngle, 0f);
		float turretSpeed = (controls.AimStick.magnitude == 0) ? turretReturnSpeed : turretRotationSpeed;
		Quaternion curRot = localTurretControls ? turret.localRotation : turret.rotation;
		turretRotation = Quaternion.Slerp(curRot, turretTargetRotation, Time.deltaTime * turretSpeed);
		return;
	}
	
	void FixedUpdate() {

		if (localShipControls) {
			float speedSign = Vector3.Dot(rigid.velocity, transform.forward) > 0 ? 1f : -1f;
			rigid.velocity = transform.TransformDirection(new Vector3(0f, 0f, speedSign * rigid.velocity.magnitude + moveControl.z));
		} else if (strafeControls) {
			rigid.velocity += transform.TransformDirection(moveControl);
		} else rigid.velocity += moveControl;
		
		if (rigid.velocity.magnitude > maxSpeed) rigid.velocity = maxSpeed * rigid.velocity.normalized;
		if (rigid.velocity.magnitude <= minSpeed) rigid.velocity = Vector3.zero;

		if (localShipControls) transform.localRotation = shipRotation;
		else transform.rotation = shipRotation;

		if (localTurretControls) turret.localRotation = turretRotation;
		else turret.rotation = turretRotation;
	}
}
