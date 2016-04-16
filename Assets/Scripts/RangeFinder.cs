using System;
using UnityEngine;
using System.Collections.Generic;

public class RangeFinder : MonoBehaviour
{
	[Header("RangeFinder: Inspector Set Fields")]
	public bool ignoreMercenaries = false;

	[Header("RangeFinder: Dynamically Set Fields")]
	public List<GameObject> inRange;
	public HealthSystem parentHealthSystem;

	void Awake()
	{
		inRange = new List<GameObject>();

		Transform parent = transform;
		while (parent.parent != null)
		{
			parent = parent.parent;
		}

		AI parentAI = parent.GetComponent<AI>();
		if (parent != null)
		{
			parentHealthSystem = parentAI;
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

		HealthSystem otherHealthSystem = parent.GetComponent<HealthSystem>();

		if (otherHealthSystem == null)
		{
			return;
		}

		if (ignoreMercenaries)
		{
			AI otherAI = otherHealthSystem as AI;
			if ((otherAI != null) && otherAI.teamSwapDestruction)
			{
				return;
			}
		}

		otherHealthSystem.OnSwapEvent += OnInRangeSwap;

		if (otherHealthSystem.teamNumber == parentHealthSystem.teamNumber)
		{
			return;
		}

		if (inRange.Find(go => go == parent.gameObject) != null)
		{
			return;
		}

		inRange.Add(parent.gameObject);
		otherHealthSystem.OnDeathEvent += OnInRangeDeath;

		return;
	}

	void OnTriggerExit(Collider other)
	{
		Transform parent = other.transform;
		while (parent.parent != null)
		{
			parent = parent.parent;
		}

		HealthSystem otherHealthSystem = parent.GetComponent<HealthSystem>();

		if (otherHealthSystem == null)
		{
			return;
		}
		otherHealthSystem.OnSwapEvent -= OnInRangeSwap;

		if (inRange.Find(go => go == parent.gameObject) == null)
		{
			return;
		}

		inRange.Remove(parent.gameObject);
		otherHealthSystem.OnDeathEvent -= OnInRangeDeath;

		return;
	}

	void OnInRangeDeath(object sender, EventArgs e)
	{
		HealthSystem senderHealth = sender as HealthSystem;
		if (senderHealth == null)
		{
			return;
		}

		if (inRange.Find(go => go == senderHealth.gameObject) == null)
		{
			return;
		}

		inRange.Remove(senderHealth.gameObject);
		return;
	}

	void OnInRangeSwap(object sender, EventArgs e)
	{
		HealthSystem senderHealth = sender as HealthSystem;
		if (senderHealth == null)
		{
			return;
		}

		if (senderHealth.teamNumber == parentHealthSystem.teamNumber)
		{
			if (inRange.Find(go => go == senderHealth.gameObject) != null)
			{
				inRange.Remove(senderHealth.gameObject);
			}
		}
		else
		{
			if (inRange.Find(go => go == senderHealth.gameObject) == null)
			{
				inRange.Add(senderHealth.gameObject);
			}
		}

		return;
	}
}