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

	[Header("Ship Parameters")]
	public float minSpeed = 0f;
	public float maxSpeed = 5f;
	public float acceleration = 10f;
	public TranslationMode shipTranslationMode = TranslationMode.GlobalMotion;
	public float friction = 0.01f;
	public float alignmentEffect = .2f;

	// TODOs:
	// reverse max speed indep of forward max speed
	// ship accceleration parameterized

	Vector3 baseVelocity = Vector3.zero;

	void FixedUpdate() {

		Vector3 pos = transform.position;
		pos.y = 0f;
		transform.position = pos;

		transform.rotation = rotation;
		baseVelocity *= 1-friction;

		Vector3 moveControl = new Vector3(controls.MoveStick.x, 0f, controls.MoveStick.y);
		moveControl *= Time.deltaTime * acceleration;

		switch (shipTranslationMode) {
			case TranslationMode.GlobalMotion:
				baseVelocity += moveControl;
				break;
			case TranslationMode.Strafing:
				baseVelocity += transform.TransformDirection(moveControl);
				break;
			case TranslationMode.ThrusterOnly:
				float speedSign = Vector3.Dot(baseVelocity, transform.forward) > 0 ? 1f : -1f;
				float forwardSpeed = speedSign * baseVelocity.magnitude + moveControl.z;
				Vector3 localVelocity = new Vector3(0f, 0f, forwardSpeed);
				baseVelocity = transform.TransformDirection(localVelocity);
				break;
			default:
				break;
		}

		if (baseVelocity.magnitude > maxSpeed) baseVelocity = maxSpeed * baseVelocity.normalized;
		if (baseVelocity.magnitude <= minSpeed) rigid.velocity = Vector3.zero;

		Vector3 boostVelocity = Vector3.zero;

		float dot = Vector3.Dot(transform.forward, baseVelocity.normalized);
		boostVelocity = dot * alignmentEffect * baseVelocity;

		rigid.velocity = baseVelocity + boostVelocity;

	}
}
