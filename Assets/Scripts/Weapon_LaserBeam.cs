using UnityEngine;

public class Weapon_LaserBeam : Weapon
{
	[Header("Weapon_LaserBeam: Inspector Set Fields")]
	public float lifetime = 1f;

	[Header("Weapon_LaserBeam: Dynamically Set Fields")]
	public float timeElapsed;

	void Start()
	{
		startingVelocity.Normalize();

		transform.position += startingVelocity * maxDistance;
		transform.rotation =
			Quaternion.LookRotation(transform.forward, startingVelocity);

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
}