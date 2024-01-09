using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    [Header("MAIN PARAMETERS")]
    public float cooldown = 10;
    public string abilityName;
    [TextArea] public string description;
    [HideInInspector] public AbilitySocket socket;
    [HideInInspector] public float cooldownTimer = 0;
    [HideInInspector] public bool activable = true;
    
    
    public virtual void SetupAbility(AbilitySocket currentSocket)
    {
        activable = true;
        cooldownTimer = 0;
        socket = currentSocket;
    }
    public virtual void StartAbility()
    {
        activable = false;
        cooldownTimer = cooldown;
    }
    
    public virtual void UpdateAbility()
    {
        CoolDown();
    }

    public void CoolDown()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            GameManager.instance.uiManager.SetAbilityCooldown(socket,1-cooldownTimer/cooldown);
        }
        else
        {
            activable = true;
            GameManager.instance.uiManager.SetAbilityCooldown(socket,1);
        }
    }
}
