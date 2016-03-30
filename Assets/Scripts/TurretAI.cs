using UnityEngine;
using System.Collections.Generic;

public class TurretAI : MonoBehaviour
{
	[Header("TurretAI: Inspector Set Fields")]
	public int teamNumber = 0;
	public float rotationSpeed = 5f;

	public GameObject weaponPrefab;
	public float delayBetweenShots = 0.5f;
	public float weaponMaxDistance = 50f; // See Weapon description.

	[Header("TurretAI: Dynamically Set Fields")]
	public List<DevTest_Player> targets;
	public Transform gun;

	public float elapsedFireDelay;

	void Awake()
	{
		targets = new List<DevTest_Player>();
		gun = transform.Find("Gun");

		elapsedFireDelay = 0f;

		return;
	}

	void Update()
	{
		// TODO: Allow settings to change who it targets.

		if (targets.Count == 0)
		{
			elapsedFireDelay = 0f;
			return;
		}

		Vector3 direction = targets[0].transform.position - transform.position;
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

			weaponComp.startingDirection = gun.rotation * Vector3.forward;
			weaponComp.maxDistance = weaponMaxDistance;
		}

		return;
	}

	void OnTriggerEnter(Collider other)
	{
		DevTest_Player target = other.GetComponent<DevTest_Player>();
		if (target == null)
		{
			return;
		}

		if (target.teamNumber == teamNumber)
		{
			return;
		}

		if (targets.Find(t => t == target) != null)
		{
			return;
		}

		targets.Add(target);
		return;
	}

	void OnTriggerExit(Collider other)
	{
		DevTest_Player target = other.GetComponent<DevTest_Player>();
		if (target == null)
		{
			return;
		}

		if (targets.Find(t => t == target) == null)
		{
			return;
		}

		targets.Remove(target);
		return;
	}
}