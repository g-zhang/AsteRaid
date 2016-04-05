using UnityEngine;
using System.Collections;

public class BaseHealth : HealthSystem {

    public bool isDestroyed = false;

    public override void DeathProcedure()
    {
        if(!isDestroyed)
        {
            print("Base DESTROYED!");
            //begin end game stuff
            isDestroyed = true;
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
        }
    }

}
