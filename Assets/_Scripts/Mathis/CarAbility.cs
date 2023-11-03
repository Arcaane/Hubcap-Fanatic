using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarAbility : MonoBehaviour
{
    [Header("ABILITY MAIN PARAMETERS")]
    public float cooldown = 10;
    public string abilityName;
    [HideInInspector] public float cooldownTimer = 0;
    [HideInInspector] public bool activable = true;
    [HideInInspector] public bool activated = false;
    public virtual void Activate()
    {
        activated = true;
    }
    
    public virtual void Execute()
    {
        
    }

    public void CoolDown()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
        else
        {
            activable = true;
        }
    }
}
