using UnityEngine;
using System.Collections;

public class PlayerStructure : MonoBehaviour {
    [Header("Player Structure: Inspector Set Fields")]
    public int teamNumber = 1;
    public int maxHealth = 50;
    public float damageTimeout = 0.5f;

    [Header("Player Structure: Dynamically Set Fields")]
    public int currHealth;
    public int damagePower;

    public bool beingHit;
    public bool tookDamage;
    public float elapsedDamageTime;

    void Awake()
    {
        currHealth = maxHealth;
        damagePower = 0;

        beingHit = false;
        tookDamage = false;
        elapsedDamageTime = 0f;

        return;
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
}
