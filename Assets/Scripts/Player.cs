using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Controls))]
public class Player : HealthSystem
{
    public enum State { Normal = 0, Dead, size };

    [Header("Player: Status")]
    public State currState = State.Normal;

    [Header("Player Weapon Config")]
    public GameObject[] weapons;
    public int selectedWeapon = 0;

    public Transform[] turretTransforms;
    private Controls controls;
	
	private float coolDownTimeRemaining = 0f;

    [Header("Player Respawn Config")]
    public Transform respawnLocation;
    public float respawnDelayTime = 3f;
    public float maxRandomOffset = .5f;
    private float currDelayTime;
    private Vector3 respawnLocationVector;

    protected override void OnAwake()
    {
        List<Transform> turrets = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.name == "Turret")
            {
                Transform barrel = child.Find("Barrel");
                if (barrel == null)
                {
                    print("Turret has no barrel :(");
                    return;
                }
                turrets.Add(barrel);
            }
        }
        turretTransforms = turrets.ToArray();

        controls = GetComponent<Controls>();

        currDelayTime = respawnDelayTime;
        respawnLocationVector = respawnLocation.position;
        currState = State.Normal;
    }

    protected override void DoOnUpdate()
    {
        coolDownTimeRemaining -= Time.deltaTime;
		if (coolDownTimeRemaining < 0f) coolDownTimeRemaining = 0f;

        switch (currState)
        {
            case State.Normal:
                if (controls.FireButtonIsPressed && weapons.Length > selectedWeapon)
                {
                    Fire(weapons[selectedWeapon]);
                }
                break;

            case State.Dead:
                ManageDeadState();
                break;

            default:
                break;
        }

		if (controls.CycleButtonWasPressed) {
			CycleWeapon();
		}
    }

    protected override void DoOnFixedUpdate()
    {
        if (tookDamage)
        {
            controls.VibrateFor(.25f, .1f);
        }
    }

    public override void DeathProcedure()
    {
        if (currState != State.Dead)
        {
            currState = State.Dead;
            tookDamage = false;
            beingHit = false;
            controls.VibrateFor(1f, .5f);
        }
    }

    void ManageDeadState()
    {
        if (currDelayTime > 0)
        {
            currDelayTime -= Time.deltaTime;

            transform.Find("PlayerShip").GetComponent<MeshRenderer>().enabled = false;
            transform.Find("PlayerShip").GetComponent<Collider>().enabled = false;
            transform.Find("Turret/Barrel").GetComponent<MeshRenderer>().enabled = false;
            transform.Find("Turret/Barrel").GetComponent<Collider>().enabled = false;
            transform.Find("HealthBar(Clone)").gameObject.SetActive(false);
        }
        else
        {
            transform.position = new Vector3(respawnLocationVector.x
                                             + Random.Range(-maxRandomOffset, maxRandomOffset),
                                             transform.position.y,
                                             respawnLocationVector.z
                                             + Random.Range(-maxRandomOffset, maxRandomOffset));
            //renable the player
            currState = State.Normal;
            currHealth = maxHealth;
            transform.Find("PlayerShip").GetComponent<MeshRenderer>().enabled = true;
            transform.Find("PlayerShip").GetComponent<Collider>().enabled = true;
            transform.Find("Turret/Barrel").GetComponent<MeshRenderer>().enabled = true;
            transform.Find("Turret/Barrel").GetComponent<Collider>().enabled = true;
            transform.Find("HealthBar(Clone)").gameObject.SetActive(true);
            currDelayTime = respawnDelayTime;
        }
    }

    void Fire(GameObject weapon)
    {

		if (coolDownTimeRemaining > 0f) return;

        foreach (Transform turret in turretTransforms)
        {
            GameObject go = Instantiate(weapon) as GameObject;
            Weapon weaponScript = go.GetComponent<Weapon>();
            weaponScript.teamNumber = teamNumber;
            weaponScript.startingVelocity = turret.forward;
            go.transform.position = turret.position;
        }

		coolDownTimeRemaining += weapon.GetComponent<Weapon>().coolDownTime;
    }

	void CycleWeapon() {
		selectedWeapon++;
		selectedWeapon %= weapons.Length;
	}
}