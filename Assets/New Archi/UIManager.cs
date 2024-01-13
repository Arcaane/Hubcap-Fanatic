using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image xAbilityImage, yAbilityImage, aAbilityImage, bAbilityImage;
    [SerializeField] private Image healthJauge;

    // public void SetAbilityCooldown(AbilitySocket socket,float value)
    // {
    //     switch (socket)
    //     {
    //         case AbilitySocket.ABILITY_A:
    //             aAbilityImage.fillAmount = value;
    //             break;
    //         case AbilitySocket.ABILITY_B:
    //             bAbilityImage.fillAmount = value;
    //             break;
    //         case AbilitySocket.ABILITY_X:
    //             xAbilityImage.fillAmount = value;
    //             break;
    //         case AbilitySocket.ABILITY_Y:
    //             yAbilityImage.fillAmount = value;
    //             break;
    //     }
    // }

    public void SetHealthJauge(float value)
    {
        healthJauge.fillAmount = value;
    }
}
