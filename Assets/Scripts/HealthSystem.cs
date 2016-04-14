using System;
using UnityEngine;
using System.Collections;

public class HealthSystem : MonoBehaviour {

    [Header("Health Sys: Config")]
    public Team teamNumber = Team.Neutral;
    public float maxHealth = 50f;
    public float damageTimeout = 0.25f;

	public float deathHealAmount = 1f;
	public int deathUltChargeAmount = 1;

    [Header("Health Bar: Config")]
    public bool enableHealthBar = true;
    public float healthBarLen = 2f;
    public Vector3 healthBarOffset = Vector3.zero;
    public GameObject HealthBarPrefab;
    protected HealthBar HPBar;

	[Header("Health Sys: Status")]
	public bool useInvulnTime;

    public float currHealth;
    public float damagePower;

    public bool beingHit;
    public bool tookDamage;
    public float elapsedDamageTime;

	public bool lastAttackWasUlt;
	public HealthSystem lastAttacker;

	public event EventHandler OnDeathEvent;
	public event EventHandler OnSwapEvent;

	//override this for custom behavior when this object is "killed"
	public virtual void DeathProcedure()
	{
        BroadcastDeathEvent();
        Destroy(gameObject);
		return;
    }

	protected void KillHealAndCharge()
	{
		Player player = lastAttacker as Player;
		if (player != null)
		{
			if (player.currState != Player.State.Dead)
			{
				player.currHealth += deathHealAmount;
				if (player.currHealth > player.maxHealth)
				{
					player.currHealth = player.maxHealth;
				}

				player.ultCharges += deathUltChargeAmount;
				if (player.ultCharges > player.chargesNeededForUlt)
				{
					player.ultCharges = player.chargesNeededForUlt;
				}
			}
		}

		return;
	}

    protected void BroadcastDeathEvent()
    {
        if (OnDeathEvent != null)
        {
            OnDeathEvent(this, EventArgs.Empty);
        }
    }

    protected void SwapProcedure()
	{
		if (OnSwapEvent != null)
		{
			OnSwapEvent(this, EventArgs.Empty);
		}
	}

    protected virtual void DoOnDamage()
    {

    }

    protected virtual void DoOnFixedUpdate()
    {

    }

    protected virtual void DoOnUpdate()
    {

    }

    protected virtual void OnAwake()
    {

    }

    protected virtual void OnStart()
    {

    }

    void Awake()
    {
		useInvulnTime = GameObject.Find("GameManager").
			GetComponent<GameManager>().useInvulnTime;

        currHealth = maxHealth;
        damagePower = 0;

        beingHit = false;
        tookDamage = false;
        elapsedDamageTime = 0f;
		lastAttacker = null;

        if(HealthBarPrefab != null && enableHealthBar)
        {
            GameObject hpobj = Instantiate(HealthBarPrefab);
            hpobj.transform.position = gameObject.transform.position + healthBarOffset;
            hpobj.transform.parent = gameObject.transform;
            HPBar = hpobj.GetComponent<HealthBar>();
            HPBar.maxLen = healthBarLen;
        }

        OnAwake();
        return;
    }

    void Start()
    {
        OnStart();
    }

    void Update()
    {
        DoOnUpdate();
    }

    void FixedUpdate()
    {
        DoOnFixedUpdate();
		
		if (useInvulnTime)
		{
			InvulnHealthFixedUpdate();
		}

        if (currHealth <= 0 || Mathf.Approximately(currHealth, 0f))
        {
            DeathProcedure();
        }
        return;
    }

    void OnTriggerStay(Collider other)
    {
        if (useInvulnTime)
		{
			InvulnHealthTriggerStay(other);
		}
		else
		{
			ImmediateHealthTriggerStay(other);
		}

        return;
    }

	void InvulnHealthFixedUpdate()
	{
		if (beingHit)
		{
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
			}
			else
			{
				currHealth -= damagePower;
				tookDamage = true;
			}
		}
	}

	void InvulnHealthTriggerStay(Collider other)
	{
		if (beingHit)
		{
			return;
		}

		Transform parent = other.transform;
		Weapon otherWeapon = parent.GetComponent<Weapon>();
		while (parent.parent != null && otherWeapon == null) {
			parent = parent.parent;
			otherWeapon = parent.GetComponent<Weapon>();
		}

		if (otherWeapon == null)
		{
			return;
		}

		if (otherWeapon.originator.teamNumber == teamNumber)
		{
			return;
		}

		lastAttacker = otherWeapon.originator;
		Weapon_LaserBeam laser = otherWeapon as Weapon_LaserBeam;
		lastAttackWasUlt = (laser != null);

		damagePower = otherWeapon.damagePower;
		beingHit = true;
	}

	void ImmediateHealthTriggerStay(Collider other)
	{
		Transform parent = other.transform;
		Weapon otherWeapon = parent.GetComponent<Weapon>();
		while (parent.parent != null && otherWeapon == null)
		{
			parent = parent.parent;
			otherWeapon = parent.GetComponent<Weapon>();
		}

		
		if (otherWeapon == null)
		{
			return;
		}

		lastAttacker = otherWeapon.originator;
		Weapon_LaserBeam laser = otherWeapon as Weapon_LaserBeam;
		lastAttackWasUlt = (laser != null);

		currHealth -= otherWeapon.damagePower;
        DoOnDamage();

		return;
	}
}
