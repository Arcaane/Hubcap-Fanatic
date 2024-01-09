using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NitroAbility : Ability
{
    [Header("ABILITY PARAMETERS")] 
    [SerializeField] private float nitroTime = 3;
    [SerializeField] private float nitroForce = 3;
    private float nitroTimer = 3;

    public override void StartAbility()
    {
        base.StartAbility();
        nitroTimer = nitroTime;
    }
    
   
}
