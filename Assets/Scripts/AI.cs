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

		switch (targetingSystem)
		{
		case TargetingSystem.FirstInRange:
		{
			target = potentialTargets[0];
			break;
		}

		case TargetingSystem.FirstInRange_PreferAI:
		{
			List<GameObject> potentialAITargets =
				potentialTargets.FindAll(go => go.GetComponent<AI>() != null);
			if (potentialAITargets.Count != 0)
			{
				target = potentialAITargets[0];
			}
			else
			{
				target = potentialTargets[0];
			}

			break;
		}

		case TargetingSystem.Closest_TiePreferAI:
		{
			float smallestDistance = float.MaxValue;
			foreach (GameObject go in potentialTargets)
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