using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {

    [Header("Config")]
    public GameObject playerToFollow;
    public bool rotateWithPlayer = false;

    private Transform target;

	// Use this for initialization
	void Start () {
        target = playerToFollow.GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
        if(target != null)
        {
            transform.position = new Vector3(target.position.x,
                                 transform.position.y,
                                 target.position.z);
            if (rotateWithPlayer)
            {
                Quaternion rot = new Quaternion();
                rot.eulerAngles = new Vector3(transform.rotation.eulerAngles.x,
                                              target.rotation.eulerAngles.y,
                                              transform.rotation.eulerAngles.z);
                transform.rotation = rot;
            }
        }
	}
}
