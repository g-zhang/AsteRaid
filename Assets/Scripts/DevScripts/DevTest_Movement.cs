using UnityEngine;
using System.Collections.Generic;

public class DevTest_Movement : MonoBehaviour
{
	[Header("DevTest_Movement: Inspector Set Fields")]
	public float movementSpeed = 5f;
	public List<DevTest_Player> players;

	[Header("DevTest_Movement: Dynamically Set Fields")]
	public int playerIndex;
	RigidbodyConstraints activeConstraints;
	RigidbodyConstraints passiveConstraints;

	void Start()
	{
		activeConstraints =
			RigidbodyConstraints.FreezeRotation |
			RigidbodyConstraints.FreezePositionZ;
		passiveConstraints = RigidbodyConstraints.FreezeAll;

		foreach (DevTest_Player player in players)
		{
			player.rigid.constraints = passiveConstraints;
		}

		playerIndex = 0;
		players[playerIndex].rigid.constraints = activeConstraints;

		return;
	}
	
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			players[playerIndex].rigid.velocity = Vector3.zero;
			players[playerIndex].rigid.constraints = passiveConstraints;

			++playerIndex;
			if (playerIndex >= players.Count)
			{
				playerIndex = 0;
			}

			players[playerIndex].rigid.constraints = activeConstraints;
		}

		return;
	}

	void FixedUpdate()
	{
		Vector3 vel = Vector3.zero;
		if (Input.GetKey(KeyCode.W))
		{
			vel.y += movementSpeed;
		}
		if (Input.GetKey(KeyCode.S))
		{
			vel.y -= movementSpeed;
		}
		if (Input.GetKey(KeyCode.A))
		{
			vel.x -= movementSpeed;
		}
		if (Input.GetKey(KeyCode.D))
		{
			vel.x += movementSpeed;
		}

		players[playerIndex].rigid.velocity = vel;
		return;
	}
}