using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarAbilitiesManager : MonoBehaviour
{
    [Header("KIT")]
    [SerializeField] private Ability xAbility,bAbility,yAbility,aAbility;

    public void ActivateXAbility()
    {
        if(!xAbility.activable) return;
        xAbility.StartAbility();
    }
    
    public void ActivateBAbility()
    {
        if(!bAbility.activable) return;
        bAbility.StartAbility();
    }
    
    public void ActivateYAbility()
    {
        if(!yAbility.activable) return;
        yAbility.StartAbility();
    }
    
    public void ActivateAAbility()
    {
        if(!aAbility.activable) return;
        aAbility.StartAbility();
    }
    
    public void Setup()
    {
        aAbility.SetupAbility(AbilitySocket.ABILITY_A);
        xAbility.SetupAbility(AbilitySocket.ABILITY_X);
        bAbility.SetupAbility(AbilitySocket.ABILITY_B);
        yAbility.SetupAbility(AbilitySocket.ABILITY_Y);
    }

    private void Update()
    {
        aAbility.UpdateAbility();
        xAbility.UpdateAbility();
        bAbility.UpdateAbility();
        yAbility.UpdateAbility();
    }
}

public enum AbilitySocket
{
    ABILITY_X,
    ABILITY_Y,
    ABILITY_A,
    ABILITY_B
}
