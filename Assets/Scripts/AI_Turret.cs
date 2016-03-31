using UnityEngine;

public class AI_Turret : AI
{
	[Header("AI_Turret: Inspector Set Fields")]
	public float rotationSpeed = 5f;

	public GameObject weaponPrefab;
	public float delayBetweenShots = 0.5f;
	public float weaponMaxDistance = 50f; // See Weapon description.

	[Header("AI_Turret: Dynamically Set Fields")]
	public RangeFinder range;
	public Transform gun;

	public float elapsedFireDelay;

	void Awake()
	{
		range = transform.Find("Range").GetComponent<RangeFinder>();
		gun = transform.Find("Gun");

		elapsedFireDelay = 0f;

		BaseAwake();
		return;
	}

	void Update()
	{
		// TODO: Allow settings to change who it targets.

		if (range.inRange.Count == 0)
		{
			elapsedFireDelay = 0f;
			return;
		}

		Vector3 direction =
			range.inRange[0].transform.position - transform.position;
		Quaternion targetRotation =
			Quaternion.LookRotation(direction, Vector3.up);
		gun.rotation = Quaternion.Slerp(
			gun.rotation, targetRotation, Time.deltaTime * rotationSpeed);

		elapsedFireDelay += Time.deltaTime;
		if (elapsedFireDelay >= delayBetweenShots)
		{
			elapsedFireDelay = 0f;

			GameObject weapon = Instantiate(weaponPrefab);
			weapon.transform.position = gun.position;

			Weapon weaponComp = weapon.GetComponent<Weapon>();

			weaponComp.startingDirection = gun.forward;
			weaponComp.maxDistance = weaponMaxDistance;
			weaponComp.teamNumber = teamNumber;
		}

		return;
	}

	void FixedUpdate()
	{
		BaseFixedUpdate();
		return;
	}

	void OnTriggerStay(Collider other)
	{
		BaseOnTriggerStay(other);
		return;
	}
}