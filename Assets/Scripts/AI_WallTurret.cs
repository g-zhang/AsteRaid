using UnityEngine;
using System.Collections;

public class AI_WallTurret : AI_Turret {

    [Header("Wall Turret: Status")]
    public bool isDestroyed = false;

    public override void DeathProcedure()
    {
		KillHealAndCharge();

		if (deathExplosion != null) {
			GameObject explosion = Instantiate (deathExplosion) as GameObject;
			explosion.transform.position = transform.position;
		}
        BroadcastDeathEvent();
        gameObject.SetActive(false);
        isDestroyed = true;
    }

    public void Respawn()
    {
        gameObject.SetActive(true);
        isDestroyed = false;
        currHealth = maxHealth;
    }
}
