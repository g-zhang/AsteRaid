﻿using UnityEngine;
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
	public float speedLimitLerpSpeed = .2f;

	void FixedUpdate() {

		Vector3 pos = transform.position;
		pos.y = 0f;
		transform.position = pos;

		transform.rotation = rotation;

		rigid.velocity *= 1 - friction;
		float alignment = Vector3.Dot(transform.forward, rigid.velocity.normalized);
		float effectiveMaxSpeed = maxSpeed + alignment * alignmentEffect * maxSpeed;

		Vector3 moveControl = new Vector3(controls.MoveStick.x, 0f, controls.MoveStick.y);
		moveControl *= Time.deltaTime * acceleration;

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

		if (rigid.velocity.magnitude > effectiveMaxSpeed) rigid.velocity *= 1 - speedLimitLerpSpeed;
		if (rigid.velocity.magnitude <= minSpeed) rigid.velocity *= 1 + speedLimitLerpSpeed;

	}
}
