using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Controls))]
public class MovementControls : MonoBehaviour {

    private Controls controls;

    public float maxDistance;
    public GameObject marker;

    public Vector2 dir;
    public List<Vector3> positions = new List<Vector3>();
    public List<Vector3> directions = new List<Vector3>();
    public List<GameObject> markers = new List<GameObject>();

	// Use this for initialization
	void Start () {
        controls = GetComponent<Controls>();
        positions.Add(transform.position);
	}
	
	// Update is called once per frame
	void Update () {
        if(positions.Count < 1)
        {
            positions.Add(transform.position);
        }
        if(markers.Count < 1)
        {
            markers.Add(Instantiate(marker));
        }

        if(positions.Count > 1 && directions.Count > 0)
        {
            for(int i = 0; i < Mathf.Min(positions.Count, directions.Count); i++)
            {
                Debug.DrawRay(positions[i], directions[i], Color.red);
            }
        }

        dir = controls.MoveDir(positions[positions.Count - 1], maxDistance);
        Debug.DrawRay(positions[positions.Count - 1], new Vector3(dir.x, dir.y, 0f), Color.red);
        markers[markers.Count - 1].transform.position = positions[positions.Count - 1] + new Vector3(dir.x, dir.y, 0f);

        if (controls.SetPoint)
        {
            directions.Add(new Vector3(dir.x, dir.y, 0f));
            positions.Add(positions[positions.Count - 1] + directions[positions.Count - 1]);
            markers.Add(Instantiate(marker));
        }

        if(controls.UnsetPoint)
        {
            if(directions.Count > 0 && positions.Count > 1)
            {
                directions.RemoveAt(directions.Count - 1);
                positions.RemoveAt(positions.Count - 1);
                Destroy(markers[markers.Count - 1]);
                markers.RemoveAt(markers.Count - 1);
            }
        }
	}
}
