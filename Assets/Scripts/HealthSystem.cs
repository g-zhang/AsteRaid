using System;
using UnityEngine;
using System.Collections;

public class HealthSystem : MonoBehaviour {

    [Header("Health Sys: Config")]
    public Team teamNumber = Team.Neutral;
    public int maxHealth = 50;
    public float damageTimeout = 0.25f;

    [Header("Health Bar: Config")]
    public bool enableHealthBar = true;
    public float healthBarLen = 2f;
    public Vector3 healthBarOffset = Vector3.zero;
    public GameObject HealthBarPrefab;
    protected HealthBar HPBar;

	[Header("Health Sys: Status")]
	public bool useInvulnTime;

    public int currHealth;
    public int damagePower;

    public bool beingHit;
    public bool tookDamage;
    public float elapsedDamageTime;
	public Team lastTeamToHit;

	public event EventHandler OnDeathEvent;

	//override this for custom behavior when this object is "killed"
	public virtual void DeathProcedure()
	{
		OnDeathEvent(this, EventArgs.Empty);
		Destroy(gameObject);
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
		lastTeamToHit = Team.Neutral;

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

			if (currHealth <= 0)
			{
				DeathProcedure();
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

		lastTeamToHit = otherWeapon.teamNumber;
		damagePower = otherWeapon.damagePower;
		beingHit = true;
	}

	void ImmediateHealthTriggerStay(Collider other)
	{
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

		lastTeamToHit = otherWeapon.teamNumber;
		currHealth -= otherWeapon.damagePower;

		if (currHealth <= 0)
		{
			DeathProcedure();
		}

		return;
	}
}
