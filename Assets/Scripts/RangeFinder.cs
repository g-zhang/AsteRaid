using UnityEngine;
using System.Collections.Generic;

public class RangeFinder : MonoBehaviour
{
	[Header("RangeFinder: Dynamically Set Fields")]
	public List<GameObject> inRange;
	public int parentTeamNumber;

	void Awake()
	{
		inRange = new List<GameObject>();
		parentTeamNumber = 0;

		Transform parent = transform;
		while (parent.parent != null)
		{
			parent = parent.parent;
		}

		AI parentAI = parent.GetComponent<AI>();
		if (parent != null)
		{
			parentTeamNumber = parentAI.teamNumber;
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

		Player otherPlayer = parent.GetComponent<Player>();
		AI otherAI = parent.GetComponent<AI>();

		if ((otherPlayer == null) && (otherAI == null))
		{
			return;
		}

		if (otherPlayer != null)
		{
			if (otherPlayer.teamNumber == parentTeamNumber)
			{
				return;
			}
		}
		if (otherAI != null)
		{
			if (otherAI.teamNumber == parentTeamNumber)
			{
				return;
			}
		}

		if (inRange.Find(go => go == parent.gameObject) != null)
		{
			return;
		}

		inRange.Add(parent.gameObject);
		return;
	}

	void OnTriggerExit(Collider other)
	{
		Transform parent = other.transform;
		while (parent.parent != null)
		{
			parent = parent.parent;
		}

		Player otherPlayer = parent.GetComponent<Player>();
		AI otherAI = parent.GetComponent<AI>();

		if ((otherPlayer == null) && (otherAI == null))
		{
			return;
		}

		if (inRange.Find(go => go == parent.gameObject) == null)
		{
			return;
		}

		inRange.Remove(parent.gameObject);
		return;
	}
}