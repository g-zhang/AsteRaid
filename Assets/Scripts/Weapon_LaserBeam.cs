using UnityEngine;

public class Weapon_LaserBeam : Weapon
{
	[Header("Weapon_LaserBeam: Inspector Set Fields")]
	public float lifetime = 1f;

	[Header("Weapon_LaserBeam: Dynamically Set Fields")]
	public float timeElapsed;

	void Start()
	{
		startingDirection.Normalize();

		transform.position += startingDirection * maxDistance;
		transform.rotation =
			Quaternion.LookRotation(transform.forward, startingDirection);

		Vector3 scale = transform.localScale;
		scale.y = maxDistance;
		transform.localScale = scale;

		timeElapsed = 0f;

		return;
	}

	void Update()
	{
		timeElapsed += Time.deltaTime;
		if (timeElapsed >= lifetime)
		{
			Destroy(gameObject);
		}

		return;
	}

	void OnTriggerStay(Collider other)
	{
		BaseOnTriggerStay(other);
		return;
	}
}