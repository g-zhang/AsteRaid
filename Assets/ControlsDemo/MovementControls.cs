using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Controls))]
[RequireComponent(typeof(Player))]
public class MovementControls : MonoBehaviour {

    private Controls controls;

    [Header("Config")]
    public float maxDistance;
    public float maxDistancePerFrame;
    public GameObject marker;

    [Header("Status")]
    public float currentFrameDistance;
    public Vector2 dir;
    public List<Vector3> positions = new List<Vector3>();
    public List<Vector3> directions = new List<Vector3>();
    public List<GameObject> markers = new List<GameObject>();

	public Player player;

	// Use this for initialization
	void Start () {
        controls = GetComponent<Controls>();
        positions.Add(transform.position);
		player = GetComponent<Player>();
        currentFrameDistance = maxDistancePerFrame;
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

        dir = controls.MoveDir(positions[positions.Count - 1], Mathf.Min(maxDistance, currentFrameDistance));
        Debug.DrawRay(positions[positions.Count - 1], new Vector3(dir.x, dir.y, 0f), Color.red);
        markers[markers.Count - 1].transform.position = positions[positions.Count - 1] + new Vector3(dir.x, dir.y, 0f);

        if (controls.SetPoint && currentFrameDistance > 0f)
        {
            directions.Add(new Vector3(dir.x, dir.y, 0f));
            positions.Add(positions[positions.Count - 1] + directions[positions.Count - 1]);
            markers.Add(Instantiate(marker));
            currentFrameDistance -= directions[directions.Count - 1].magnitude;
        }

        if(controls.UnsetPoint)
        {
            if(directions.Count > 0 && positions.Count > 1)
            {
                currentFrameDistance += directions[directions.Count - 1].magnitude;
                currentFrameDistance = currentFrameDistance > maxDistancePerFrame ? maxDistancePerFrame : currentFrameDistance;
                directions.RemoveAt(directions.Count - 1);
                positions.RemoveAt(positions.Count - 1);
                Destroy(markers[markers.Count - 1]);
                markers.RemoveAt(markers.Count - 1);
            }
        }

		if (controls.SetFrame)
        {
			Vector3[] relPositions = new Vector3[positions.Count];
			for (int i = 0; i < positions.Count; i++)
				relPositions[i] = positions[i] - positions[0];
			player.QueueFrame(new MoveFrame(new BezierCurve(relPositions)));
			Vector3 tmp = positions[positions.Count - 1];
			positions.Clear();
			positions.Add(tmp);
			directions.Clear();

            while(markers.Count > 2)
            {
                Destroy(markers[0]);
                markers.RemoveAt(0);
            }

            currentFrameDistance = maxDistancePerFrame;

			print("adding move frame with ctl points" + positions.ToString());
		}

        if (controls.ActivateFrames)
        {
            player.ActivateFrames();
        }
	}
}
