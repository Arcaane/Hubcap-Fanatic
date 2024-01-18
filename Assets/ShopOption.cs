using System;
using System.Collections;
using System.Collections.Generic;
using Abilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopOption : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text[] levelTxt;
    public TMP_Text[] description;
    public Image[] logo;
    public Image tag;
    public Color abilityColor, boostColor, goldColor;

    public void SetOption(AbilitiesSO ability)
    {
        title.text = ability.abilityName;
        levelTxt[0].text = levelTxt[1].text = ability.level == 0 ? "NEW" : "LEVEL " + ability.level;
        description[0].text = description[1].text = ability.description;
        logo[0].sprite = logo[1].sprite = ability.abilitySprite;
        levelTxt[0].color = tag.color = ability.type switch
        {
            AbilityType.ClassicAbilites => abilityColor,
            AbilityType.UpgrateStatsAbilites => boostColor,
            AbilityType.GoldGiver => goldColor,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
