using UnityEngine;
using System.Collections;

public class BaseHealth : HealthSystem {

    public bool isDestroyed = false;

	protected override void OnAwake(){
		if (teamNumber == Team.Team1)
			gameObject.layer = LayerMask.NameToLayer ("BlueBase");
		if (teamNumber == Team.Team2)
			gameObject.layer = LayerMask.NameToLayer ("RedBase");
	}

    public override void DeathProcedure()
    {
        if(!isDestroyed)
        {
            isDestroyed = true;
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;

			if (deathExplosion != null)
			{
				GameObject explosion = Instantiate(deathExplosion);
				explosion.transform.position = transform.position;
			}

            //begin end game stuff
            GameManager.GM.EndTheGame();
        }
    }
}
