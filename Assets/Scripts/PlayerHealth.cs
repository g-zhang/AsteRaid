using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controls))]
public class PlayerHealth : HealthSystem {

    [Header("PlayerHealth: Config")]

    private Controls controls;

    public override void OnAwake()
    {
        controls = GetComponent<Controls>();
    }

    public override void DoOnFixedUpdate()
    {
        if(tookDamage)
        {
            controls.VibrateFor(.25f, .1f);
        }
    }

    public override void DeathProcedure()
    {
        enabled = false;
        transform.Find("Mesh1").GetComponent<MeshRenderer>().enabled = false;
        transform.Find("Mesh1").GetComponent<Collider>().enabled = false;
        transform.Find("Turret/Barrel").GetComponent<MeshRenderer>().enabled = false;
        transform.Find("Turret/Barrel").GetComponent<Collider>().enabled = false;
    }
}
