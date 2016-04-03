using UnityEngine;
using System.Collections;

public class BaseHealth : HealthSystem {

    public override void DeathProcedure()
    {
        print("Base DESTROYED!");
        //begin end game stuff
        Destroy(gameObject);
    }

}
