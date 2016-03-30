﻿using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Weapon_Bullet : Weapon
{
	[Header("Weapon_Bullet: Inspector Set Fields")]
	public float speed = 5f;

	[Header("Weapon_Bullet: Dynamically Set Fields")]
	public Rigidbody rigid;
	public Vector3 startPosition;

	void Start()
	{
		rigid = GetComponent<Rigidbody>();
		startPosition = transform.position;

		startingDirection.Normalize();
		rigid.velocity = startingDirection * speed;

		return;
	}

	void Update()
	{
		if (Vector3.Magnitude(transform.position - startPosition) > maxDistance)
		{
			Destroy(gameObject);
		}
		return;
	}

	void OnTriggerEnter(Collider other)
	{
		Transform parent = other.transform;
		while (parent.parent != null)
		{
			parent = parent.parent;
		}

		Player otherPlayer = parent.GetComponent<Player>();
		AI otherAI = parent.GetComponent<AI>();

		if (otherPlayer != null)
		{
			if (otherPlayer.teamNumber == teamNumber)
			{
				return;
			}
		}
		if (otherAI != null)
		{
			if (otherAI.teamNumber == teamNumber)
			{
				return;
			}
		}

		Destroy(gameObject);
		return;
	}

	void OnTriggerStay(Collider other)
	{
		BaseOnTriggerStay(other);
		return;
	}
}