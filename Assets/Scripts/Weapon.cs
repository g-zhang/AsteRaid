using UnityEngine;

public class Weapon : MonoBehaviour
{
	[Header("Weapon: Inspector Set Fields")]
	public float damagePower = 1;
	public float coolDownTime = 0.5f;
	public float maxDistance;

	[Header("Weapon: Externally Set Fields")]
	public Team teamNumber;
	public Vector3 startingVelocity;
}