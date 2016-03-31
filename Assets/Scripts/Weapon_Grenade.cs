using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Weapon_Grenade : Weapon
{
	[Header("Weapon_Grenade: Inspector Set Fields")]
	public float startingSpeed = 5f;
	public float explosionSpeed = 5f;
	public float maxExplosionSize = 10f;
	public float fudgeValue = 0.1f;

	public Material explosionMat;

	[Header("Weapon_Grenade: Dynamically Set Fields")]
	public Rigidbody rigid;
	public Renderer rend;

	public Vector3 startPosition;
	public bool isExploding;

	void Start()
	{
		rigid = GetComponent<Rigidbody>();
		rend = GetComponent<Renderer>();

		startPosition = transform.position;
		isExploding = false;

		rigid.velocity = startingVelocity * startingSpeed;
		startingVelocity = rigid.velocity;

		return;
	}

	void FixedUpdate()
	{
		if (isExploding)
		{
			Explode();
		}
		else
		{
			Move();
		}

		return;
	}

	void OnTriggerEnter(Collider other)
	{
		if (isExploding)
		{
			return;
		}

		Transform parent = other.transform;
		while (parent.parent != null)
		{
			parent = parent.parent;
		}

		Weapon otherWeapon = parent.GetComponent<Weapon>();
		Player otherPlayer = parent.GetComponent<Player>();
		AI otherAI = parent.GetComponent<AI>();

		if (otherWeapon != null)
		{
			return;
		}
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

		rend.material = explosionMat;
		isExploding = true;

		return;
	}

	void Explode()
	{
		if (transform.localScale.x >= maxExplosionSize)
		{
			Destroy(gameObject);
			return;
		}

		float sizeChange = explosionSpeed * Time.fixedDeltaTime;
		transform.localScale +=
			new Vector3(sizeChange, sizeChange, sizeChange);
		
		return;
	}

	void Move()
	{
		float currentDistance =
			Vector3.Magnitude(transform.position - startPosition);
		if (currentDistance >= (maxDistance - fudgeValue))
		{
			rend.material = explosionMat;
			isExploding = true;
		}

		rigid.velocity = Vector3.Lerp(
			startingVelocity, Vector3.zero, currentDistance / maxDistance);
		return;
	}
}