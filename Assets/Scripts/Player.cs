using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Controls))]
public class Player : MonoBehaviour
{
	[Header("Player: Inspector Set Fields")]
	public int teamNumber = 1;
	public int maxHealth = 50;
	public float damageTimeout = 0.5f;

	[Header("Player: Dynamically Set Fields")]
	public int currHealth;
	public int damagePower;

	public bool beingHit;
	public bool tookDamage;
	public float elapsedDamageTime;

	public GameObject[] weapons;
	public int selectedWeapon = 0;

	public Transform[] turretTransforms;
	private Controls controls;

	void Awake()
	{
		currHealth = maxHealth;
		damagePower = 0;

		beingHit = false;
		tookDamage = false;
		elapsedDamageTime = 0f;

		List<Transform> turrets = new List<Transform>();
		foreach (Transform child in transform) {
			if (child.name == "Turret") {
				Transform barrel = child.Find("Barrel");
				if (barrel == null) {
					print("Turret has no barrel :(");
					return;
				}
				turrets.Add(barrel);
			}
		}
		turretTransforms = turrets.ToArray();

		controls = GetComponent<Controls>();
		return;
	}

	void Update() {
		if (controls.FireButtonWasPressed && weapons.Length > selectedWeapon) Fire(weapons[selectedWeapon]);
	}

	void FixedUpdate()
	{
		if (!beingHit)
		{
			return;
		}

		if (tookDamage)
		{
			elapsedDamageTime += Time.fixedDeltaTime;
			if (elapsedDamageTime >= damageTimeout)
			{
				elapsedDamageTime = 0f;
				beingHit = false;
				tookDamage = false;
				damagePower = 0;
			}

			return;
		}

		currHealth -= damagePower;
		if (currHealth <= 0)
		{
			// This will likely be changed.
			Destroy(gameObject);
		}
		tookDamage = true;

		return;
	}

	void OnTriggerStay(Collider other)
	{
		if (beingHit)
		{
			return;
		}

		Transform parent = other.transform;
		while (parent.parent != null)
		{
			parent = parent.parent;
		}

		Weapon otherWeapon = parent.GetComponent<Weapon>();
		if (otherWeapon == null)
		{
			return;
		}

		if (otherWeapon.teamNumber == teamNumber)
		{
			return;
		}

		damagePower = otherWeapon.damagePower;
		beingHit = true;

		return;
	}

	void Fire(GameObject weapon) {
		foreach (Transform turret in turretTransforms) {
			GameObject go = Instantiate(weapon) as GameObject;
			Weapon weaponScript = go.GetComponent<Weapon>();
			weaponScript.teamNumber = teamNumber;
			weaponScript.startingVelocity = turret.forward;
			weaponScript.maxDistance = 50f;
			go.transform.position = turret.position;
		}
	}

}