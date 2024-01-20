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
    public Color abilityColor, boostColor, goldColor,unavailableColor;
    public AbilitiesSO shopAbility;

    public void SetOption(AbilitiesSO ability)
    {
        title.text = ability.abilityName.ToUpper();
        shopAbility = ability;
        levelTxt[0].text = levelTxt[1].text = ability.level == -1 ? "NEW" : "LEVEL " + (ability.level + 1);
        description[0].text = description[1].text = ability.description;
        logo[0].sprite = logo[1].sprite = ability.abilitySprite;
        levelTxt[0].color = tag.color = ability.type switch
        {
            AbilityType.ClassicAbilites => abilityColor,
            AbilityType.UpgrateStatsAbilites => boostColor,
            AbilityType.GoldGiver => goldColor,
            _ => throw new ArgumentOutOfRangeException()
        };
        if (CarExperienceManager.Instance.levelUpTokensAvailable <= 0)
        {
            description[0].color = logo[1].color = unavailableColor;
        }
        else
        {
            description[0].color = logo[1].color = Color.white;
        }
    }

    public void Buy()
    {
        CarAbilitiesManager.instance.AddAbility(shopAbility);
        CarExperienceManager.Instance.levelUpTokensAvailable--;
        TestShop.instance.SetupItemsInShop();
    }
}
