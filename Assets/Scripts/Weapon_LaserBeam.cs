using UnityEngine;

public class Weapon_LaserBeam : Weapon
{
	[Header("Weapon_LaserBeam: Inspector Set Fields")]
	public float lifetime = 1f;
	public float leadTime = 0.25f;

	public LayerMask team1LaserCollisions;
	public LayerMask team2LaserCollisions;

	[Header("Weapon_LaserBeam: Dynamically Set Fields")]
	public float timeElapsed;
	public Transform laser;

	Transform firer;
	LayerMask finalLaserCollisions;

	float lasLength;

	void Start()
	{
		laser = transform.Find ("laser");
		laser.gameObject.SetActive (false);
		timeElapsed = 0f;

		firer = transform.parent;

		if (firer == null) {
			print ("ERROR: Orphaned laser.");
			Destroy (gameObject);
		}

		transform.parent = null;
		lasLength = laser.localScale.z;
		laser.localPosition = new Vector3(0, 0, (lasLength / 2) + 1);
		transform.rotation = firer.rotation;

		HealthSystem hs = firer.GetComponent<HealthSystem>();
		if (hs.teamNumber == Team.Team1)
		{
			finalLaserCollisions = team1LaserCollisions;
		}
		else if (hs.teamNumber == Team.Team2)
		{
			finalLaserCollisions = team2LaserCollisions;
		}

		ParticleSystem.CollisionModule coll =
			transform.Find("particles").GetComponent<ParticleSystem>().collision;
		coll.collidesWith = finalLaserCollisions;

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

		transform.position = firer.position;
		transform.rotation = firer.rotation;

		Debug.DrawRay(transform.position, transform.forward * laser.localScale.z);

		RaycastHit hit;
		if (Physics.Raycast(transform.position - transform.forward,
			transform.forward, out hit, laser.localScale.z + 1, finalLaserCollisions))
		{
			Vector3 scale = laser.localScale;
			scale.z = lasLength * (hit.distance / lasLength);
			laser.localScale = scale;
		}
		else
		{
			Vector3 scale = laser.localScale;
			scale.z = lasLength;
			laser.localScale = scale;
		}
		laser.localPosition = new Vector3(0, 0, (laser.localScale.z / 2) + 1);

		return;
	}
}