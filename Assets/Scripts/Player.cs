﻿using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Controls))]
public class Player : HealthSystem
{
    public enum State { Normal = 0, Dead, Invuln, size };

    [Header("Player: Status")]
    public State currState = State.Normal;
    public float currRespawnDelayTime = 0f;
    public float currDelayTime; //this is timer you want for UI

    float currEffectTime = 0f;

    [Header("Weapon Prefabs")]
    public GameObject primaryWeapon;
    public GameObject secondaryWeapon;
    public GameObject ultWeapon;

	[Header("Weapon Parameters")]
    public int chargesNeededForUlt = 5;
    public int ultCharges = 0;
	public int grenadeAmmo = 3;
	public int maxGrenadeAmmo = 3;
	public float grenadeRefillTime = 5f;
	private float timeSinceGrenadeRefill = 0f;
	private float primaryCoolDownRemaining = 0f;
    private float grenadeCooldownRemaining = 0f;

	[Header("Weapon Spawn Transforms")]
	public List<Transform> turretTransforms;
	public List<Transform> altTurretTransforms;

    [Header("Player Respawn Config")]
	public Transform[] respawnLocation;
    public float respawnDelayTimeMin = 6f;
    public float respawnDelayTimeMax = 20f;
    public float respawnIncrement = 2f;
    public float maxRandomOffset = .5f;

	//private Vector3 respawnLocationVector;
	private Controls controls;
	private GameObject effects;

    void SetLayers()
    {
        if (teamNumber == Team.Team1)
        {
            gameObject.layer = LayerMask.NameToLayer("BluePlayer");
            foreach (Transform tr in transform)
            {
                tr.gameObject.layer = LayerMask.NameToLayer("BluePlayer");
            }
        }

        if (teamNumber == Team.Team2)
        {
            gameObject.layer = LayerMask.NameToLayer("RedPlayer");
            foreach (Transform tr in transform)
            {
                tr.gameObject.layer = LayerMask.NameToLayer("RedPlayer");
            }
        }

        transform.Find("GhostShip").gameObject.layer = LayerMask.NameToLayer("Ghost");
    }

    protected override void OnAwake()
    {
        SetLayers();

        foreach (Transform child in transform)
        {
            if (child.name == "Turret") turretTransforms.Add(child);
            if (child.name == "AltTurret") altTurretTransforms.Add(child);
        }
        altTurretTransforms.AddRange(turretTransforms);

        controls = GetComponent<Controls>();
        effects = transform.Find("Effects").gameObject;
        
        currRespawnDelayTime = respawnDelayTimeMin;
        currDelayTime = currRespawnDelayTime;
        //respawnLocationVector = respawnLocation.position;
        currState = State.Normal;
    }

    protected override void OnStart()
    {
        Color tcolor = GameManager.GM.teamColors[(int)teamNumber];
        Color ecolor = GameManager.GM.getTeamColor(teamNumber, enemyColor: true);
        Color tGColor = new Color(tcolor.r, tcolor.g, tcolor.b, .2f);
        transform.Find("GhostShip").GetComponent<Renderer>().material.color = tGColor;

        // Material[] shipMats = transform.Find("PlayerShip").GetComponent<Renderer>().materials;
        // shipMats[1].color = tcolor;
        effects.GetComponent<ParticleSystem>().startColor = new Color(ecolor.r, ecolor.g, ecolor.b, .75f);

    }

    protected override void DoOnUpdate()
    {
		timeSinceGrenadeRefill += Time.deltaTime;
		if (timeSinceGrenadeRefill >= grenadeRefillTime){
			if (grenadeAmmo < maxGrenadeAmmo) grenadeAmmo++;
			timeSinceGrenadeRefill = 0f;
		}

        if(currEffectTime > 0)
        {
            currEffectTime -= Time.deltaTime;
            if(!effects.GetComponent<ParticleSystem>().isPlaying)
                effects.GetComponent<ParticleSystem>().Play();
        } else
        {
            effects.GetComponent<ParticleSystem>().Stop();
        }

        primaryCoolDownRemaining -= Time.deltaTime;
        if (primaryCoolDownRemaining < 0f) primaryCoolDownRemaining = 0f;
        grenadeCooldownRemaining -= Time.deltaTime;
        if (grenadeCooldownRemaining < 0f) grenadeCooldownRemaining = 0f;

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
		if (controls.FireButtonIsPressed) Fire(primaryWeapon, altTurretTransforms);
		if (controls.SecondFireButtonIsPressed && grenadeAmmo > 0) Fire(secondaryWeapon, turretTransforms);
		if (controls.UltButtonWasPressed && ultCharges >= chargesNeededForUlt) {
			Fire(ultWeapon, turretTransforms);
			ultCharges = 0;
		}
    }

    protected override void DoOnDamage()
    {
        controls.VibrateFor(.25f, .2f);
        currEffectTime = .2f;
    }

    public override void DeathProcedure()
    {
        if (currState != State.Dead)
        {
            KillHealAndCharge();
            currState = State.Dead;
            currRespawnDelayTime += respawnIncrement;
            currRespawnDelayTime = currRespawnDelayTime > respawnDelayTimeMax ? respawnDelayTimeMax : currRespawnDelayTime;
            ultCharges = 0;
            tookDamage = false;
            beingHit = false;
            controls.VibrateFor(1f, .5f);
            DisableShip();
            BroadcastDeathEvent();
        }
    }

    void ManageDeadState()
    {
        if (currRespawnDelayTime - currDelayTime <= respawnIncrement)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        } else
        {
            GetComponent<ShipControls>().enabled = true;
        }

        if (currDelayTime > 0)
        {
            currDelayTime -= Time.deltaTime;
        }
        else
        {
            //RespawnShip();
        }

		for (int i = 0; i < respawnLocation.Length; i++) {
			if (distanceFromRespawn (i) <= GameManager.GM.regenRadius) {
				RespawnShip ();
			}
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
        transform.Find("GhostShip").gameObject.SetActive(true);
    }

    void RespawnShip()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        //transform.position = new Vector3(respawnLocationVector.x
        //                         + Random.Range(-maxRandomOffset, maxRandomOffset),
        //                         transform.position.y,
        //                         respawnLocationVector.z
        //                         + Random.Range(-maxRandomOffset, maxRandomOffset));
        //renable the player
        currState = State.Normal;
        currHealth = 1f;
        GetComponent<ShipTrails>().enabled = true;
        //GetComponent<ShipControls>().enabled = true;
        transform.Find("PlayerShip").GetComponent<MeshRenderer>().enabled = true;
        transform.Find("PlayerShip").GetComponent<Collider>().enabled = true;
        transform.Find("Turret").gameObject.SetActive(true);
        transform.Find("HealthBar(Clone)").gameObject.SetActive(true);
        currDelayTime = currRespawnDelayTime;
        transform.Find("GhostShip").gameObject.SetActive(false);
    }

    void Fire(GameObject weapon, List<Transform> turrets)
    {

        if (weapon == primaryWeapon && primaryCoolDownRemaining > 0f) return;
        if (weapon == secondaryWeapon && grenadeCooldownRemaining > 0f) return;

        foreach (Transform turret in turrets)
        {
            GameObject go = Instantiate(weapon) as GameObject;
            Weapon weaponScript = go.GetComponent<Weapon>();
            weaponScript.originator = this;
            weaponScript.startingVelocity = turret.forward;
            go.transform.position = turret.position;

            if (weaponScript is Weapon_LaserBeam) go.transform.parent = transform;

            if (teamNumber == Team.Team1)
            {
                go.layer = LayerMask.NameToLayer("BlueWeapon");
                foreach (Transform tr in go.transform)
                {
                    tr.gameObject.layer = LayerMask.NameToLayer("BlueWeapon");
                }
            }

            if (teamNumber == Team.Team2)
            {
                go.layer = LayerMask.NameToLayer("RedWeapon");
                foreach (Transform tr in go.transform)
                {
                    tr.gameObject.layer = LayerMask.NameToLayer("RedWeapon");
                }
            }
        }

        if (weapon == primaryWeapon)
            primaryCoolDownRemaining += weapon.GetComponent<Weapon>().coolDownTime;
		if (weapon == secondaryWeapon) {
			grenadeCooldownRemaining += weapon.GetComponent<Weapon> ().coolDownTime;
			grenadeAmmo--;
		}

    }

	float distanceFromRespawn(int respawnIndex)
    {
        return Vector3.Distance(transform.position, respawnLocation[respawnIndex].position);
    }

    void baseRegenHealth()
    {
		for (int i = 0; i < respawnLocation.Length; i++) {
			float distToRespawn = distanceFromRespawn (i);

			if (distToRespawn <= GameManager.GM.regenRadius) {
				regenHealth (GameManager.GM.regenRate, Time.deltaTime);
			}
		}
    }

    public void regenHealth(float regenRate, float deltaTime)
    {
        if (currHealth < maxHealth)
        {
            currHealth += regenRate * deltaTime;
            if (currHealth > maxHealth)
                currHealth = maxHealth;
        }
    }
}