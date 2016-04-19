using UnityEngine;
using System.Collections.Generic;

public enum IncentiveType
{
	HealOverTime
}

public class Incentive : MonoBehaviour
{
	[Header("Incentive: Inspector Set Fields")]
	public IncentiveType type = IncentiveType.HealOverTime;

	public bool enemyTeamBlocks = true;
	public float healthRegenRate = 5f;

	[Header("Incentive: Dynamically Set Fields")]
	public List<Player> playersInRange;

	void Awake()
	{
		playersInRange = new List<Player>();

		return;
	}

	void Update()
	{
		switch (type)
		{
		case IncentiveType.HealOverTime:
		{
			HealthRegen();
			break;
		}
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

		Player player = parent.GetComponent<Player>();
		if (player == null)
		{
			return;
		}

		if (playersInRange.Find(p => p == player) != null)
		{
			return;
		}
		playersInRange.Add(player);

		return;
	}

	void OnTriggerExit(Collider other)
	{
		Transform parent = other.transform;
		while (parent.parent != null)
		{
			parent = parent.parent;
		}

		Player player = parent.GetComponent<Player>();
		if (player == null)
		{
			return;
		}

		if (playersInRange.Find(p => p == player) == null)
		{
			return;
		}
		playersInRange.Remove(player);

		return;
	}

	void HealthRegen()
	{
		List<Player> team1 = new List<Player>(
			playersInRange.FindAll(p => p.teamNumber == Team.Team1));
		List<Player> team2 = new List<Player>(
			playersInRange.FindAll(p => p.teamNumber == Team.Team2));

		if ((team1.Count > 0) && (team2.Count == 0))
		{
			foreach (Player player in team1)
			{
				player.regenHealth(healthRegenRate, Time.deltaTime);
			}
		}
		else if ((team1.Count == 0) && (team2.Count > 0))
		{
			foreach (Player player in team2)
			{
				player.regenHealth(healthRegenRate, Time.deltaTime);
			}
		}

		return;
	}
}