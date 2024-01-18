using System;
using System.Collections.Generic;
using Abilities;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class TestShop : MonoBehaviour
{
    [SerializeField] private List<AbilitiesSO> allAbilities;
    [SerializeField] private List<AbilitiesSO> purchasableAbilities;
    [SerializeField] private AbilitiesSO gold;

    [SerializeField] private ShopOption[] buttonsItemsArray = new ShopOption[3];

    [SerializeField] private Camera cam;

    public bool playerInRange;
    
    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            buttonsItemsArray[i] = UIManager.instance.shopOptions[i];
        }


        for (int i = 0; i < allAbilities.Count; i++) allAbilities[i].level = -1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
           
        }
    }

    private void Update()
    {
        if (playerInRange)
        {
            UIManager.instance.shopIcon.position = cam.WorldToScreenPoint(transform.position);
            UIManager.instance.shopIcon.localScale = Vector3.Lerp(UIManager.instance.shopIcon.localScale,Vector3.one, Time.deltaTime*7);
        }
        else
        {
            UIManager.instance.shopIcon.localScale = Vector3.Lerp(UIManager.instance.shopIcon.localScale,Vector3.zero, Time.deltaTime*7);
        }
        
    }

    public void StartShopUI()
    {
        UIManager.instance.OpenShopScreen();
        
        SetupItemsInShop();
    }

    public void SetupItemsInShop()
    {
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
