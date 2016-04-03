using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(ShipControls))]
[RequireComponent(typeof(Rigidbody))]
public class ShipTrails : MonoBehaviour {

	public GameObject windTrailPrefab;
	public Vector3[] trailPositions = new Vector3[] {
		new Vector3(-.5f,0,0),
		new Vector3(.5f,0,0),
	};

	private ShipControls shipControls;
	private Rigidbody rigid;
	private float lastFrameVelocity = 0f;
	private List<GameObject> trails = new List<GameObject>();
	private List<GameObject> deadTrails = new List<GameObject>();

	void Start() {
		shipControls = GetComponent<ShipControls>();
		rigid = GetComponent<Rigidbody>();
	}

	void Update() {
		if (lastFrameVelocity < shipControls.maxSpeed && rigid.velocity.magnitude >= shipControls.maxSpeed) enableTrails();
		if (lastFrameVelocity >= shipControls.maxSpeed && rigid.velocity.magnitude < shipControls.maxSpeed) disableTrails();
		lastFrameVelocity = rigid.velocity.magnitude;
	}

	private void enableTrails() {
		print("EN");
		foreach (Vector3 pos in trailPositions) {
			GameObject trail = Instantiate(windTrailPrefab);
			trail.transform.parent = transform;
			trail.transform.localPosition = pos;
			trails.Add(trail);
		}
	}

	private void disableTrails() {
		print("DIS");
		foreach (GameObject trail in trails) {
			trail.transform.parent = null;
			deadTrails.Add(trail);
		}
		trails.Clear();
		Invoke("killTrails", 10f);
	}

	private void killTrails() {
		foreach (GameObject trail in deadTrails) Destroy(trail);
		deadTrails.Clear();
	}
}
