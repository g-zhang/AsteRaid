﻿using UnityEngine;

public enum WeaponName { Bullet = 0, Laser, Bomb }

public class Weapon : MonoBehaviour
{
	[Header("Weapon: Inspector Set Fields")]
	public float damagePower = 1;
	public float coolDownTime = 0.5f;
	public float maxDistance;

	[Header("Weapon: Externally Set Fields")]
	public HealthSystem originator;
	public Vector3 startingVelocity;
	public Vector3 beginVelocity;
}