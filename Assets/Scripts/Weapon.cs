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
}