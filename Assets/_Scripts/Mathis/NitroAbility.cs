using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NitroAbility : CarAbility
{
    [Header("ABILITY PARAMETERS")] 
    [SerializeField] private float nitroTime = 3;
    [SerializeField] private float nitroForce = 3;
    [SerializeField] private WheelCarController controller;
    private float nitroTimer = 3;

    public override void Activate()
    {
        base.Activate();
        nitroTimer = nitroTime;
    }
    
    public override void Execute()
    {
        if (nitroTimer > 0)
        {
            nitroTimer -= Time.deltaTime;
            Vector3 fwd = new Vector3(controller.transform.forward.x, 0, controller.transform.forward.z).normalized;
            controller.rb.AddForce(fwd*nitroForce);
        }
        else
        {
            activated = false;
        }
    }
}
