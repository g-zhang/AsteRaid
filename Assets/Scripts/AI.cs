using UnityEngine;
using System.Collections.Generic;

public enum TargetingSystem
{
	FirstInRange,
	FirstInRange_PreferAI,
	Closest_TiePreferAI
}

public class AI : HealthSystem
{
	[Header("AI: Inspector Set Fields")]
	public bool teamSwapDestruction = false;
	public TargetingSystem targetingSystem = TargetingSystem.FirstInRange;
	public LayerMask targetingRaycastMask;

	[Header("AI: Dynamically Set Fields")]
	public RangeFinder range;

	protected override void OnAwake()
	{
		range = transform.Find("Range").GetComponent<RangeFinder>();
		return;
	}

	public override void DeathProcedure()
	{
		KillHealAndCharge();

		if (teamSwapDestruction)
		{
			if (deathExplosion != null) {
				GameObject explosion = Instantiate (deathExplosion) as GameObject;
				explosion.transform.position = transform.position;
			}
			if (lastAttacker == null)
			{
				// This shouldn't happen, but....
				return;
			}

			teamNumber = lastAttacker.teamNumber;
			currHealth = maxHealth;

			range.inRange.Clear();

			// You are not expected to understand this.
			range.gameObject.SetActive(false);
			range.gameObject.SetActive(true);

			SwapProcedure();
		}
		else
		{
			base.DeathProcedure();
		}

		return;
	}

	protected GameObject GetTarget(List<GameObject> potentialTargets)
	{
		GameObject target = null;
		if (potentialTargets.Count == 0)
		{
			return target;
		}

		List<GameObject> nonBlockedTargets = new List<GameObject>();
		foreach (GameObject go in potentialTargets)
		{
			if (go == null)
			{
				continue;
			}

			Vector3 direction = go.transform.position - transform.position;
			float distance = Vector3.Magnitude(direction);

			if (!Physics.Raycast(transform.position,
				direction, distance, targetingRaycastMask))
			{
				nonBlockedTargets.Add(go);
			}
		}

		if (nonBlockedTargets.Count == 0)
		{
			return target;
		}

		switch (targetingSystem)
		{
		case TargetingSystem.FirstInRange:
		{
			target = nonBlockedTargets[0];
			break;
		}

		case TargetingSystem.FirstInRange_PreferAI:
		{
			List<GameObject> potentialAITargets =
				nonBlockedTargets.FindAll(go => go.GetComponent<AI>() != null);
			if (nonBlockedTargets.Count != 0)
			{
				target = potentialAITargets[0];
			}
			else
			{
				target = nonBlockedTargets[0];
			}

			break;
		}

		case TargetingSystem.Closest_TiePreferAI:
		{
			float smallestDistance = float.MaxValue;
			foreach (GameObject go in nonBlockedTargets)
			{
				if (go == null)
				{
					continue;
				}

				float distance = Vector3.Distance(transform.position, go.transform.position);
				if (Mathf.Approximately(distance, smallestDistance))
				{
					if (target.GetComponent<AI>() == null)
					{
						if (go.GetComponent<AI>() != null)
						{
							target = go;
						}
					}
				}
				else if (distance < smallestDistance)
				{
					target = go;
				}
			}

			break;
		}
		}

		return target;
	}
}