using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarKitManager : MonoBehaviour
{
    [Header("KIT")]
    [SerializeField] private CarAbility xAbility;
    [SerializeField] private CarAbility bAbility;
    [SerializeField] private CarAbility nitroAbility;

    [Header("UI")] 
    [SerializeField] private Image[] abilityImages;
    [SerializeField] private TMP_Text[] abilityTexts;
    
    public void ActivateXAbility()
    {
        if(!xAbility.activable) return;
        xAbility.Activate();
        xAbility.activable = false;
        xAbility.cooldownTimer = xAbility.cooldown;
        abilityImages[1].fillAmount = 0;
    }
    
    public void ActivateBAbility()
    {
        if(!bAbility.activable) return;
        bAbility.Activate();
        bAbility.activable = false;
        bAbility.cooldownTimer = bAbility.cooldown;
        abilityImages[2].fillAmount = 0;
    }
    
    public void ActivateNitro()
    {
        if(!nitroAbility.activable) return;
        nitroAbility.Activate();
        nitroAbility.activable = false;
        nitroAbility.cooldownTimer = nitroAbility.cooldown;
        abilityImages[3].fillAmount = 0;
    }
    
    public void ActivateDash()
    {
        
    }

    private void Update()
    {
        if (xAbility.activated)
        {
            xAbility.Execute();
        }
        else if (!xAbility.activable)
        {
            xAbility.CoolDown(); 
            abilityImages[1].fillAmount = 1 - (xAbility.cooldownTimer / xAbility.cooldown);
        }

        if (bAbility.activated)
        {
            bAbility.Execute();
        }
        else if (!bAbility.activable)
        {
            bAbility.CoolDown(); 
            abilityImages[2].fillAmount = 1 - (bAbility.cooldownTimer / bAbility.cooldown);
        }
        
        if (nitroAbility.activated)
        {
            nitroAbility.Execute();
        }
        else if (!nitroAbility.activable)
        {
            nitroAbility.CoolDown(); 
            abilityImages[3].fillAmount = 1 - (nitroAbility.cooldownTimer / nitroAbility.cooldown);
        }
    }

    private void Start()
    {
        abilityTexts[1].text = xAbility.abilityName;
        abilityTexts[2].text = bAbility.abilityName;
    }
}
