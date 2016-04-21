using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {

    [Header("Config")]
    public GameObject playerToFollow;
    public bool rotateWithPlayer = false;

    private Transform target;
    private Player player;
    private Player.State prevState;

    // Use this for initialization
    void Start () {
        target = playerToFollow.GetComponent<Transform>();
        player = playerToFollow.GetComponent<Player>();
        if (target == null || player == null)
        {
            Debug.LogError("Target player isn't a player!");
            Destroy(this);
        }
        prevState = player.currState;
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

        if(prevState != player.currState)
        {
            //if(player.currState == Player.State.Dead)
            //{
            //    GetComponent<Camera>().cullingMask |= (1 << LayerMask.NameToLayer("Ghost"));
            //}
            //else
            //{
            //    GetComponent<Camera>().cullingMask = ~(1 << LayerMask.NameToLayer("Ghost"));
            //}
            prevState = player.currState;
        }
	}
}
