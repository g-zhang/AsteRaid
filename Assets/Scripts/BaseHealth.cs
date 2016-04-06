using UnityEngine;
using System.Collections;

public class BaseHealth : HealthSystem {

    public bool isDestroyed = false;

    public override void DeathProcedure()
    {
        if(!isDestroyed)
        {
            isDestroyed = true;
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;

            //begin end game stuff
            GameManager.GM.EndTheGame();
        }
    }

}
