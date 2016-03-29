using UnityEngine;
using System.Collections;

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
	public float minSpeed = 0f;
	public float maxSpeed = 5f;
	public float shipRotationSpeed = 12f;
	public float shipYawSpeed = 120f;

	[Header("Turret Parameters")]
	public bool localTurretControls = false;
	public float turretConeHalfAngle = 180f;
	public float turretRotationSpeed = 60f;
	public bool turretFloatsHome = false;
	public float turretReturnSpeed = 30f;

	[Header("Unimplemented")]
	public bool turretFaceMouse = false;
	public bool shipFaceMouse = false;
	public bool drawTrajectory = false;
	// natural backward ship motion in local control mode
	
	private Vector2 LeftStick;
	private Vector2 RightStick;
	private Vector3 rightStick3;
	private Vector3 leftStick3;
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

		LeftStick = controls.ID.LeftStick.Vector;
		RightStick = controls.ID.RightStick.Vector;
		rightStick3 = new Vector3(RightStick.x, RightStick.y, 0f);
		leftStick3 = new Vector3(LeftStick.x, LeftStick.y, 0f);

		if (controls.ID.Action4.WasPressed) localShipControls = !localShipControls;
		if (controls.ID.Action3.WasPressed) localTurretControls = !localTurretControls;

		if (strafeControls) {
			float zAngle = 0f;
			if (controls.ID.LeftBumper.IsPressed)
				zAngle = Time.deltaTime * shipYawSpeed;
			if (controls.ID.RightBumper.IsPressed)
				zAngle = Time.deltaTime * -shipYawSpeed;
			shipRotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, 0f, zAngle));
		} else {
			Vector3 normVel = rigid.velocity.normalized;
			Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, normVel);
			shipRotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * shipRotationSpeed);
		}

		if (RightStick.magnitude > 0f) turretTargetAngle = Vector3.Angle(Vector3.up, rightStick3);
		else if (turretFloatsHome) turretTargetAngle = 0f;
		if (turretTargetAngle > turretConeHalfAngle) turretTargetAngle = turretConeHalfAngle;
		if (RightStick.x > 0) turretTargetAngle = -turretTargetAngle;
		Quaternion turretTargetRotation = Quaternion.Euler(0f, 0f, turretTargetAngle);
		float turretSpeed = (RightStick.magnitude == 0) ? turretReturnSpeed : turretRotationSpeed;
		turretRotation = Quaternion.Slerp(turret.rotation, turretTargetRotation, Time.deltaTime * turretSpeed);
	}

	public bool inReverse = false;
	void FixedUpdate() {

		if (localShipControls) rigid.velocity += transform.TransformDirection(leftStick3);
		else rigid.velocity += leftStick3;
		
		if (rigid.velocity.magnitude > maxSpeed) rigid.velocity = maxSpeed * rigid.velocity.normalized;
		if (rigid.velocity.magnitude <= minSpeed) rigid.velocity = Vector3.zero;
		
		transform.rotation = shipRotation;

		if (localTurretControls) turret.localRotation = turretRotation;
		else turret.rotation = turretRotation;
	}
}
