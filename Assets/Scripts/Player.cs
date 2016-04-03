using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Controls))]
[RequireComponent(typeof(PlayerHealth))]
public class Player : MonoBehaviour
{
	public GameObject[] weapons;
	public int selectedWeapon = 0;

	public Transform[] turretTransforms;
	private Controls controls;

	public float rateOfFire = 10f;
	private float timeSinceShot = 0f;

    public int teamNumber = 0;

    void Awake()
	{
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
        teamNumber = GetComponent<HealthSystem>().teamNumber;
		return;
	}

	void Update() {
        timeSinceShot += Time.deltaTime;
		if (controls.FireButtonIsPressed && weapons.Length > selectedWeapon) Fire(weapons[selectedWeapon]);
	}

	void FixedUpdate()
	{

	}


	void Fire(GameObject weapon) {

		if (timeSinceShot < 1f / rateOfFire) return;
		
		foreach (Transform turret in turretTransforms) {
			GameObject go = Instantiate(weapon) as GameObject;
			Weapon weaponScript = go.GetComponent<Weapon>();
			weaponScript.teamNumber = teamNumber;
			weaponScript.startingVelocity = turret.forward;
			weaponScript.maxDistance = 50f;
			go.transform.position = turret.position;
		}
		timeSinceShot = 0f;
	}

}