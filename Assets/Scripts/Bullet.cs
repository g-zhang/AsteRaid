using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : Weapon
{
	[Header("Bullet: Inspector Set Fields")]
	public float speed = 5f;

	[Header("Bullet: Dynamically Set Fields")]
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
}