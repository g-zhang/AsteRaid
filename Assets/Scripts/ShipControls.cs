using UnityEngine;
using System.Collections;

public enum TranslationMode {
	GlobalMotion,
	ThrusterOnly,
	Strafing
}

[RequireComponent(typeof(Controls))]
[RequireComponent(typeof(Rigidbody))]
public class ShipControls : Swivel {

	[Header("Components")]
	private Controls controls;
	private Rigidbody rigid;
	private Transform turret;

	[Header("Ship Parameters")]
	public float minSpeed = 0f;
	public float maxSpeed = 5f;
	public float shipYawSpeed = 120f;
	public TranslationMode shipTranslationMode = TranslationMode.GlobalMotion;
	public RotationMode turretRotationMode = RotationMode.PointAlongStick;

	// TODOs:
	// reverse max speed indep of forward max speed
	// ship accceleration parameterized
	// split to turret / ship classes

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

	void SetShipRotation() {
		switch (swivelMode) {
			case (RotationMode.PointAtMouse):
				PointAlongVector(controls.ToMouseVector);
				break;
			case (RotationMode.YawOnStick):
				RotateBy(Time.deltaTime * shipYawSpeed * controls.MoveStick.x);
				break;
			case (RotationMode.YawOnYawAxis):
				RotateBy(Time.deltaTime * shipYawSpeed * controls.YawAxis);
				break;
			case (RotationMode.PointInVelocity):
				PointAlongVector(rigid.velocity.normalized);
				break;
			case (RotationMode.PointAlongStick):
				PointAlongVector(controls.MoveStick);
				break;
			default:
				break;
		}
	}

	void SetTurretRotation() {

		if (turretFaceMouse) {
			if (controls.ToMouseVector.magnitude == 0f) return;
			Quaternion lookRotation = Quaternion.LookRotation(controls.ToMouseVector, Vector3.up);
			turretRotation = Quaternion.Slerp(turret.rotation, lookRotation, Time.deltaTime * turretRotationSpeed);
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
	}
	
	void FixedUpdate() {

		switch (shipTranslationMode) {
			case TranslationMode.GlobalMotion:
				rigid.velocity += moveControl;
				break;
			case TranslationMode.Strafing:
				rigid.velocity += transform.TransformDirection(moveControl);
				break;
			case TranslationMode.ThrusterOnly:
				float speedSign = Vector3.Dot(rigid.velocity, transform.forward) > 0 ? 1f : -1f;
				float forwardSpeed = speedSign * rigid.velocity.magnitude + moveControl.z;
				Vector3 localVelocity = new Vector3(0f, 0f, forwardSpeed);
				rigid.velocity = transform.TransformDirection(localVelocity);
				break;
			default:
				break;
		}
		
		if (rigid.velocity.magnitude > maxSpeed) rigid.velocity = maxSpeed * rigid.velocity.normalized;
		if (rigid.velocity.magnitude <= minSpeed) rigid.velocity = Vector3.zero;
		transform.rotation = rotation;
		if (localTurretControls) turret.localRotation = turretRotation;
		else turret.rotation = turretRotation;
	}
}
