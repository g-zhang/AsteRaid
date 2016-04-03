using UnityEngine;

public class Weapon : MonoBehaviour
{
	[Header("Weapon: Inspector Set Fields")]
	public int damagePower = 1;
	public float coolDownTime = 0.5f;

	[Header("Weapon: Externally Set Fields")]
	public Team teamNumber;
	public Vector3 startingVelocity;

	// This represents different things based on weapon.
	//
	// Bullets: Max distance before disappearing.
	// Lasers: Length of the laser beam.
	public float maxDistance;
}