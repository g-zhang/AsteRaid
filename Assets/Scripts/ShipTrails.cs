using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(ShipControls))]
[RequireComponent(typeof(Swivel))]
[RequireComponent(typeof(Rigidbody))]
public class ShipTrails : MonoBehaviour {

    [Header("Wind Trail Config")]
	public GameObject windTrailPrefab;
	public Vector3[] trailPositions = new Vector3[] {
		new Vector3(-1f,0,-.5f),
		new Vector3(1f,0,-.5f),
	};

	private ShipControls shipControls;
	private Rigidbody rigid;
	private float lastFrameVelocity = 0f;
	private List<GameObject> trails = new List<GameObject>();
	private List<GameObject> deadTrails = new List<GameObject>();

    [Header("Spin Trail Config")]
    public GameObject spinTrailPrefab;
    public Vector3[] spinPositions = new Vector3[] {
        new Vector3(-2f,0,0),
        new Vector3(2f,0,0),
    };
    private List<GameObject> spinTrails = new List<GameObject>();
    private Swivel shipRotation;

    void Start() {
		shipControls = GetComponent<ShipControls>();
        shipRotation = GetComponent<Swivel>();
		rigid = GetComponent<Rigidbody>();
	}

	void Update() {
		if (lastFrameVelocity < shipControls.maxSpeed && rigid.velocity.magnitude >= shipControls.maxSpeed && !shipRotation.isSpinning) enableTrails();
		if (lastFrameVelocity >= shipControls.maxSpeed && rigid.velocity.magnitude < shipControls.maxSpeed) disableTrails();
		lastFrameVelocity = rigid.velocity.magnitude;

        if(shipRotation.isSpinning && spinTrails.Count <= 0)
        {
            enableSpinTrails();
        }
        else if(!shipRotation.isSpinning && spinTrails.Count > 0)
        {
            disableSpinTrails();
        }
	}

	private void enableTrails() {
		foreach (Vector3 pos in trailPositions) {
			GameObject trail = Instantiate(windTrailPrefab);
			trail.transform.parent = transform;
			trail.transform.localPosition = pos;
			trails.Add(trail);
		}
	}

	private void disableTrails() {
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

    private void enableSpinTrails()
    {
        foreach (Vector3 pos in spinPositions)
        {
            GameObject trail = Instantiate(spinTrailPrefab);
            trail.transform.parent = transform;
            trail.transform.localPosition = pos;
            spinTrails.Add(trail);
        }
    }

    private void disableSpinTrails()
    {
        foreach (GameObject trail in spinTrails) Destroy(trail);
        spinTrails.Clear();
    }

    public void disableAllTrails()
    {
        disableSpinTrails();
        disableTrails();
    }
}
