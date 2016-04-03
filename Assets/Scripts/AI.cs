using UnityEngine;

public class AI : HealthSystem
{
	[Header("AI: Inspector Set Fields")]
	public bool teamSwapDestruction = false;

	public override void DeathProcedure()
	{
		if (teamSwapDestruction)
		{
			teamNumber = lastTeamToHit;
			currHealth = maxHealth;

			// TODO: Leaves an odd bug.
			// On destruction/swap, opposing team will
			// will not be in fire "range".
			// Need to change RangeFinder?
		}
		else
		{
			base.DeathProcedure();
		}

		return;
	}
}