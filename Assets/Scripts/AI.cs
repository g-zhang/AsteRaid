using UnityEngine;

public class AI : HealthSystem
{
	[Header("AI: Inspector Set Fields")]
	public bool teamSwapDestruction = false;

	[Header("AI: Dynamically Set Fields")]
	public RangeFinder range;

	protected override void OnAwake()
	{
		range = transform.Find("Range").GetComponent<RangeFinder>();
		return;
	}

	public override void DeathProcedure()
	{
		if (teamSwapDestruction)
		{
			teamNumber = lastTeamToHit;
			currHealth = maxHealth;

			range.inRange.Clear();

			// You are not expected to understand this.
			range.gameObject.SetActive(false);
			range.gameObject.SetActive(true);
		}
		else
		{
			base.DeathProcedure();
		}

		return;
	}
}