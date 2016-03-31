using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Player))]
public class PlayerRespawn : MonoBehaviour {

    private Player player;
    private float currDelayTime;
    private Vector3 respawnLocationVector;

    [Header("Config")]
    public Transform respawnLocation;
    public float respawnDelayTime = 2f;
    public float maxRandomOffset = .5f;

	// Use this for initialization
	void Start () {
        player = GetComponent<Player>();
        currDelayTime = respawnDelayTime;
        respawnLocationVector = respawnLocation.position;
	}
	
	// Update is called once per frame
	void Update () {
        if(player.currHealth <= 0)
        {
            if(currDelayTime > 0)
            {
                currDelayTime -= Time.deltaTime;
            } else
            {
                transform.position = new Vector3(respawnLocationVector.x
                                                 + Random.Range(-maxRandomOffset, maxRandomOffset),
                                                 transform.position.y,
                                                 respawnLocationVector.z
                                                 + Random.Range(-maxRandomOffset, maxRandomOffset));
                //renable the player
                player.enabled = true;
                player.currHealth = player.maxHealth;
                transform.Find("Mesh1").GetComponent<MeshRenderer>().enabled = true;
                transform.Find("Mesh1").GetComponent<Collider>().enabled = true;
                transform.Find("Turret/Barrel").GetComponent<MeshRenderer>().enabled = true;
                transform.Find("Turret/Barrel").GetComponent<Collider>().enabled = true;
                currDelayTime = respawnDelayTime;
            }
        }
	}
}
