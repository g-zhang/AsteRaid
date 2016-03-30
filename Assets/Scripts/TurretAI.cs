using UnityEngine;
using System.Collections.Generic;

public class TurretAI : MonoBehaviour
{
	[Header("TurretAI: Inspector Set Fields")]
	public int teamNumber = 0;
	public float rotationSpeed = 5f;

	[Header("TurretAI: Dynamically Set Fields")]
	public List<DevTest_Player> targets;
	public Transform gun;

	void Awake()
	{
		targets = new List<DevTest_Player>();
		gun = transform.Find("Gun");

		return;
	}

	void Update()
	{
		if (targets.Count == 0)
		{
			return;
		}

		Vector3 direction = targets[0].transform.position - transform.position;
		Quaternion targetRotation =
			Quaternion.LookRotation(direction, gun.up);
		gun.localRotation = Quaternion.Slerp(
			gun.localRotation, targetRotation, Time.deltaTime * rotationSpeed);

		// TODO: Shoot at the target.

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