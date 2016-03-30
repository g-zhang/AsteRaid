using UnityEngine;

public class Weapon : MonoBehaviour
{
	[Header("Weapon: Inspector Set Fields")]
	public int damagePower = 1;

	[Header("Weapon: Externally Set Fields")]
	public int teamNumber;
	public Vector3 startingDirection;

	// This represents different things based on weapon.
	//
	// Bullets: Max distance before disappearing.
	// Lasers: Length of the laser beam.
	public float maxDistance;

	protected void BaseOnTriggerStay(Collider other)
	{
		// Ignore RangeFinders.
		if (other.GetComponent<RangeFinder>() != null)
		{
			return;
		}

		Transform parent = other.transform;
		while (parent.parent != null)
		{
			parent = parent.parent;
		}

		Player otherPlayer = parent.GetComponent<Player>();
		AI otherAI = parent.GetComponent<AI>();

		if (otherPlayer != null)
		{
			if ((otherPlayer.teamNumber != teamNumber) && !otherPlayer.beingHit)
			{
				otherPlayer.damagePower = damagePower;
				otherPlayer.beingHit = true;
			}
		}
		if (otherAI != null)
		{
			if ((otherAI.teamNumber != teamNumber) && !otherAI.beingHit)
			{
				otherAI.damagePower = damagePower;
				otherAI.beingHit = true;
			}
		}

		return;
	}
}