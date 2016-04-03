using UnityEngine;
using System.Collections;

public class HealthSystem : MonoBehaviour {

    [Header("Health Sys: Config")]
    public int teamNumber = 1;
    public int maxHealth = 50;
    public float damageTimeout = 0.25f;

    [Header("Health Bar: Config")]
    public bool enableHealthBar = true;
    public float healthBarLen = 2f;
    public Vector3 healthBarOffset = Vector3.zero;
    public GameObject HealthBarPrefab;
    protected HealthBar HPBar;

    [Header("Health Sys: Status")]
    public int currHealth;
    public int damagePower;

    public bool beingHit;
    public bool tookDamage;
    public float elapsedDamageTime;

    //override this for custom behavior when this object is "killed"
    public virtual void DeathProcedure()
    {
        Destroy(gameObject);
    }

    public virtual void DoOnFixedUpdate()
    {

    }

    public virtual void OnAwake()
    {

    }

    void Awake()
    {
        currHealth = maxHealth;
        damagePower = 0;

        beingHit = false;
        tookDamage = false;
        elapsedDamageTime = 0f;

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

    void FixedUpdate()
    {
        DoOnFixedUpdate();
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

                return;
            }

            currHealth -= damagePower;
            tookDamage = true;
        }

        if (currHealth <= 0)
        {
            DeathProcedure();
        }
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
}
