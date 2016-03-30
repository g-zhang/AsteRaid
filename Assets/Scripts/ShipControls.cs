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
	public bool shipFaceMouse = false;

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
		aimControl = new Vector3(controls.AimStick.x, controls.AimStick.y, 0f);
		moveControl = new Vector3(controls.MoveStick.x, controls.MoveStick.y, 0f);
		SetShipRotation();
		SetTurretRotation();
	}

	void SetShipRotation() {

		Quaternion currentRotation = localShipControls ? transform.localRotation : transform.rotation;

		if (shipFaceMouse) {
			Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, controls.ToMouseVector);
			shipRotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime * shipRotationSpeed);
			return;
		}

		if (strafeControls) { 
			Vector3 angles = currentRotation.eulerAngles;
			angles.z += Time.deltaTime * shipYawSpeed * controls.YawAxis;
			shipRotation = Quaternion.Euler(angles);
			return;
		}

		Vector3 normVel = rigid.velocity.normalized;
		Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, normVel);
		shipRotation = Quaternion.Slerp(currentRotation, lookRotation, Time.deltaTime * shipRotationSpeed);
		return;

	}

	void SetTurretRotation() {

		if (turretFaceMouse) {
			Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, controls.ToMouseVector);
			turretRotation = Quaternion.Slerp(turret.rotation, lookRotation, Time.deltaTime * shipRotationSpeed);
			return;
		}

		if (controls.AimStick.magnitude > 0f) turretTargetAngle = Vector3.Angle(Vector3.up, aimControl);
		else if (turretFloatsHome) turretTargetAngle = 0f;
		if (turretTargetAngle > turretConeHalfAngle) turretTargetAngle = turretConeHalfAngle;
		if (controls.AimStick.x > 0) turretTargetAngle = -turretTargetAngle;
		Quaternion turretTargetRotation = Quaternion.Euler(0f, 0f, turretTargetAngle);
		float turretSpeed = (controls.AimStick.magnitude == 0) ? turretReturnSpeed : turretRotationSpeed;
		turretRotation = Quaternion.Slerp(turret.rotation, turretTargetRotation, Time.deltaTime * turretSpeed);
		return;
	}
	
	void FixedUpdate() {

		if (localShipControls) rigid.velocity += transform.TransformDirection(moveControl);
		else rigid.velocity += moveControl;
		
		if (rigid.velocity.magnitude > maxSpeed) rigid.velocity = maxSpeed * rigid.velocity.normalized;
		if (rigid.velocity.magnitude <= minSpeed) rigid.velocity = Vector3.zero;

		if (localShipControls) transform.localRotation = shipRotation;
		else transform.rotation = shipRotation;

		if (localTurretControls) turret.localRotation = turretRotation;
		else turret.rotation = turretRotation;
	}
}
