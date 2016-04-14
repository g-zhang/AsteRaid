using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Controls))]
public class Player : HealthSystem
{
    public enum State { Normal = 0, Dead, Invuln, size };

    [Header("Player: Status")]
	public State currState = State.Normal;
	public Controls controls;

    [Header("Player Weapon Prefabs")]
	public GameObject primaryWeapon;
	public GameObject secondaryWeapon;
	public GameObject ultWeapon;
	public int chargesNeededForUlt = 5;
	public int ultCharges = 0;
	public List<Transform> turretTransforms;
	private float coolDownTimeRemaining = 0f;

    [Header("Player Respawn Config")]
    public Transform respawnLocation;
    public float respawnDelayTime = 3f;
    public float maxRandomOffset = .5f;
    private float currDelayTime;
    private Vector3 respawnLocationVector;

    protected override void OnAwake()
    {

		if (teamNumber == Team.Team1) {
			gameObject.layer = LayerMask.NameToLayer("BluePlayer");
			foreach (Transform tr in transform) {
				tr.gameObject.layer = LayerMask.NameToLayer("BluePlayer");
			}
		}

		if (teamNumber == Team.Team2) {
			gameObject.layer = LayerMask.NameToLayer("RedPlayer");
			foreach (Transform tr in transform) {
				tr.gameObject.layer = LayerMask.NameToLayer("RedPlayer");
			}
		}

        foreach (Transform child in transform) {
            if (child.name == "Turret") turretTransforms.Add(child);
        }

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
                baseRegenHealth();
                if (controls.InvincibliltyOn)
                {
                    Debug.Log("Player(" + controls.playerNum + ") is invulnerable!");
                    currState = State.Invuln;
                }
                break;

            case State.Dead:
                ManageDeadState();
                break;

            case State.Invuln:
                currHealth = maxHealth;
                if (controls.InvincibliltyOff)
                {
                    Debug.Log("Player(" + controls.playerNum + ") stopped cheating!");
                    currState = State.Normal;
                }
                break;

            default:
                break;
        }
    }

    protected override void DoOnFixedUpdate()
    {
		if (currState == State.Dead) return;
		if (controls.FireButtonIsPressed) Fire(primaryWeapon);
		if (controls.SecondFireButtonIsPressed) Fire(secondaryWeapon);
		if (controls.UltButtonIsPressed && ultCharges >= chargesNeededForUlt) {
			Fire(ultWeapon);
			ultCharges = 0;
		}
    }

    protected override void DoOnDamage()
    {
        controls.VibrateFor(.25f, .2f);
    }

    public override void DeathProcedure()
    {
		KillHealAndCharge();

        if (currState != State.Dead)
        {
            currState = State.Dead;
            tookDamage = false;
            beingHit = false;
            controls.VibrateFor(1f, .5f);
            DisableShip();
            BroadcastDeathEvent();
        }
    }

    void ManageDeadState()
    {
        if (currDelayTime > 0)
        {
            currDelayTime -= Time.deltaTime;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        else
        {
            RespawnShip();
        }
    }

    void DisableShip()
    {
        GetComponent<ShipTrails>().disableAllTrails();
        GetComponent<ShipTrails>().enabled = false;
        GetComponent<ShipControls>().enabled = false;
        transform.Find("PlayerShip").GetComponent<MeshRenderer>().enabled = false;
        transform.Find("PlayerShip").GetComponent<Collider>().enabled = false;
        transform.Find("Turret").gameObject.SetActive(false);
        transform.Find("HealthBar(Clone)").gameObject.SetActive(false);
    }

    void RespawnShip()
    {
        transform.position = new Vector3(respawnLocationVector.x
                                 + Random.Range(-maxRandomOffset, maxRandomOffset),
                                 transform.position.y,
                                 respawnLocationVector.z
                                 + Random.Range(-maxRandomOffset, maxRandomOffset));
        //renable the player
        currState = State.Normal;
        currHealth = maxHealth;
        GetComponent<ShipTrails>().enabled = true;
        GetComponent<ShipControls>().enabled = true;
        transform.Find("PlayerShip").GetComponent<MeshRenderer>().enabled = true;
        transform.Find("PlayerShip").GetComponent<Collider>().enabled = true;
        transform.Find("Turret").gameObject.SetActive(true);
        transform.Find("HealthBar(Clone)").gameObject.SetActive(true);
        currDelayTime = respawnDelayTime;
    }

    void Fire(GameObject weapon)
    {

        if (coolDownTimeRemaining > 0f) return;

        foreach (Transform turret in turretTransforms) {
            GameObject go = Instantiate(weapon) as GameObject;
            Weapon weaponScript = go.GetComponent<Weapon>();
            weaponScript.originator = this;
            weaponScript.startingVelocity = turret.forward;
            go.transform.position = turret.position;
            if (weaponScript is Weapon_LaserBeam) go.transform.parent = transform;
			if (teamNumber == Team.Team1) go.layer = LayerMask.NameToLayer ("BlueWeapon");
			if (teamNumber == Team.Team2) go.layer = LayerMask.NameToLayer ("RedWeapon");
		}
		coolDownTimeRemaining += weapon.GetComponent<Weapon>().coolDownTime;
    }
	
    void baseRegenHealth()
    {
        float distToRespawn = float.MaxValue;

        if (teamNumber == Team.Team1)
        {
            distToRespawn = Vector3.Distance(transform.position, GameManager.GM.base_team1.transform.Find("RespawnPoint").transform.position);
        }
        else if (teamNumber == Team.Team2)
        {
            distToRespawn = Vector3.Distance(transform.position, GameManager.GM.base_team2.transform.Find("RespawnPoint").transform.position);
        }

        if (distToRespawn <= GameManager.GM.regenRadius && currHealth < maxHealth)
        {
            currHealth += GameManager.GM.regenRate * Time.fixedDeltaTime;
            if (currHealth > maxHealth)
                currHealth = maxHealth;
        }
    }
}