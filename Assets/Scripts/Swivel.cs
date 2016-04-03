using UnityEngine;
using System.Collections;

public enum RotationMode {
	PointAtMouse,
	PointAlongMoveStick,
	PointAlongAimStick,
	PointInVelocity,
	YawOnMoveStick,
	YawOnAimStick,
	YawOnYawAxis
}

public class Swivel : MonoBehaviour {

	private Rigidbody rigid;
	private Controls controls;
	private Quaternion rotation = Quaternion.identity;

    [Header("Swivel Parameters")]
	public RotationMode swivelMode = RotationMode.PointInVelocity;
	public float rotationLerpSpeed = 60f;
	public float yawSpeed = 120f;

    [Header("Spin Attack Config")]
    public float spinTime = .5f;
    public float slerpSpeed = .5f;
    public float maxSpinSpeed = 3600f;
    float currSpinSpeed = 0f;
    bool isSpinning = false;
    float currSpinTime = 0f;

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

	public void SetRotation() {
		switch (swivelMode) {
			case (RotationMode.PointAtMouse):
				PointAlongVector(ToMouseVector);
				break;
			case (RotationMode.YawOnMoveStick):
				RotateBy(Time.deltaTime * yawSpeed * controls.MoveStick.x);
				break;
			case (RotationMode.YawOnAimStick):
				RotateBy(Time.deltaTime * yawSpeed * controls.AimStick.x);
				break;
			case (RotationMode.YawOnYawAxis):
				RotateBy(Time.deltaTime * yawSpeed * controls.YawAxis);
				break;
			case (RotationMode.PointInVelocity):
				PointAlongVector(rigid.velocity.normalized);
				break;
			case (RotationMode.PointAlongMoveStick):
				PointAlongVector(Vector2ToXZVector3(controls.MoveStick));
				break;
			case (RotationMode.PointAlongAimStick):
				PointAlongVector(Vector2ToXZVector3(controls.AimStick));
				break;
			default:
				break;
		}
        if (isSpinning)
        {
            SpinAttack();
        }
    }

    void SpinAttack()
    {
        if (currSpinTime > 0f)
        {
            currSpinTime -= Time.deltaTime;
            currSpinSpeed = maxSpinSpeed;
        }
        else
        {
            currSpinSpeed = Mathf.Lerp(currSpinSpeed, 0f, rotationLerpSpeed * Time.deltaTime);
        }

        RotateBy(currSpinSpeed * Time.deltaTime);

        if (currSpinSpeed <= rotationLerpSpeed)
        {
            currSpinTime = spinTime;
            isSpinning = false;
            currSpinSpeed = 0f;
        }
    }

	Vector3 Vector2ToXZVector3(Vector2 v2) {
		return new Vector3(v2.x, 0f, v2.y);
	}

	void Start() {
        currSpinTime = spinTime;

		controls = GetComponent<Controls>();
		rigid = GetComponent<Rigidbody>();
		if (controls != null && rigid != null) return;

		controls = transform.parent.GetComponent<Controls>();
		rigid = transform.parent.GetComponent<Rigidbody>();
		if (controls != null && rigid != null) return;
		print("Warning: Swivel requires a Controls and a Rigidbody on this object or it's parent!");
		Destroy(this);
	}

	protected virtual void Update() {
		SetRotation();
        if(controls.SpinButtonWasPressed)
        {
            isSpinning = true;
        }
	}

	protected virtual void FixedUpdate() {
		transform.rotation = rotation;
	}

	public Vector3 ToMouseVector {
		get {
			return controls.MousePosition - transform.position;
		}
	}
}
