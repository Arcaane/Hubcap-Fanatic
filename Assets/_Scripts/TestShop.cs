using System;
using System.Collections.Generic;
using Abilities;
using UnityEngine;
using Random = UnityEngine.Random;

public class TestShop : MonoBehaviour
{
    [SerializeField] private List<AbilitiesSO> allAbilities;
    [SerializeField] private List<AbilitiesSO> purchasableAbilities;
    [SerializeField] private AbilitiesSO gold;

    [SerializeField] private ShopOption[] buttonsItemsArray = new ShopOption[3];
    public static TestShop instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            buttonsItemsArray[i] = UIManager.Instance.shopOptions[i];
        }
        
        for (int i = 0; i < allAbilities.Count; i++) allAbilities[i].level = -1;
    }
    
    public void StartShopUI()
    {
        UIManager.Instance.OpenShopScreen();
        SetupItemsInShop();
    }

    public void SetupItemsInShop()
    {
        if (CarExperienceManager.Instance.levelUpTokensAvailable <= 0)
        {
            UIManager.Instance.CloseShopScreen();
            return;
        }
        
        purchasableAbilities.Clear();

        bool noPassivePlace = CarAbilitiesManager.instance.passiveAbilities.Count >=
                             CarAbilitiesManager.instance.slotAbilitiesAmount;
        bool noBoostPlace = CarAbilitiesManager.instance.statsAbilities.Count >=
                           CarAbilitiesManager.instance.slotAbilitiesAmount;
        
        for (int i = 0; i < allAbilities.Count; i++)
        {
            if (allAbilities[i].level >= 3) continue;

            if (allAbilities[i].level == -1 && allAbilities[i].type == AbilityType.ClassicAbilites && noPassivePlace) continue;
            
            if (allAbilities[i].level == -1 && allAbilities[i].type == AbilityType.UpgrateStatsAbilites && noBoostPlace) continue;
            
            purchasableAbilities.Add(allAbilities[i]);
        }

        int availableAbilities = Mathf.Clamp(purchasableAbilities.Count, 0, 3);

        purchasableAbilities = ShuffleList(purchasableAbilities);
        
        for (int i = 0; i < 3; i++)
        {
            if (i < availableAbilities)
            {
                buttonsItemsArray[i].SetOption(purchasableAbilities[i]);
            }
            else
            {
                buttonsItemsArray[i].SetOption(gold);
            }
        }
    }

    public static List<T> ShuffleList<T>(List<T> list)
    {
        List<T> rngList = new List<T>(0);
        int x = list.Count;
        for (int i = 0; i < x; i++)
        {
            int rng = Random.Range(0, list.Count);
            rngList.Add(list[rng]);
            list.RemoveAt(rng);
        }
        return rngList;
    }
}
