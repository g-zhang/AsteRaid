using UnityEngine;

public class Weapon_LaserBeam : Weapon
{
	[Header("Weapon_LaserBeam: Inspector Set Fields")]
	public float lifetime = 1f;
	public float leadTime = 0.25f;

	[Header("Weapon_LaserBeam: Dynamically Set Fields")]
	public float timeElapsed;
	public Transform laser;

	void Start()
	{
		laser = transform.Find ("laser");
		if (laser == null) print ("FU");
		laser.gameObject.SetActive (false);
		startingVelocity.Normalize();

		transform.position += startingVelocity * maxDistance;
		transform.rotation =
			Quaternion.LookRotation(transform.forward, startingVelocity);

		Vector3 scale = transform.localScale;
		scale.y = maxDistance;
		transform.localScale = scale;

		timeElapsed = 0f;

		laser.GetComponent<Renderer>().material.color =
			GameManager.GM.teamColors[(int)originator.teamNumber];

        return;
	}

	void Update()
	{
		timeElapsed += Time.deltaTime;
		if (timeElapsed >= leadTime && !laser.gameObject.activeSelf) {
			laser.gameObject.SetActive(true);
		}

		if (timeElapsed >= lifetime)
		{
			Destroy(gameObject);
		}

		return;
	}
}