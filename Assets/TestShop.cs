using System.Collections.Generic;
using Abilities;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestShop : MonoBehaviour
{
    [SerializeField] private List<AbilitiesSO> allAbilities;
    [SerializeField] private List<AbilitiesSO> purchasableAbilities;
    [SerializeField] private AbilitiesSO gold;
    
    [SerializeField] private ButtonsItems[] buttonsItemsArray = new ButtonsItems[3];
    private bool isShopActive;

    private void Start()
    {
        UIManager.instance.shopScreen.SetActive(false);
        for (int i = 0; i < 3; i++)
        {
            buttonsItemsArray[i] = UIManager.instance.buttonsHandlers[i];
        }
        
        isShopActive = false;

        for (int i = 0; i < allAbilities.Count; i++) purchasableAbilities.Add(allAbilities[i]);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (CarExperienceManager.Instance.levelUpTokensAvailable > 0)
            {
                StartShopUI();
            }
            else
            {
                Debug.Log("Pas de thunes igo");
            }
        }
    }

    private void StartShopUI()
    {
        Time.timeScale = 0;
        UIManager.instance.shopScreen.SetActive(true);
        isShopActive = true;
        EventSystem.current.SetSelectedGameObject(buttonsItemsArray[0].gameObject);
        SetupItemsInShop();
    }

    private void SetupItemsInShop()
    {
        for (int i = 0; i < buttonsItemsArray.Length; i++) buttonsItemsArray[i].gameObject.SetActive(false);

        if (purchasableAbilities.Count == 0)
        {
            SetupGoldInShop();
        }
        
        if (purchasableAbilities.Count == 1)
        {
            SetupButton(0);
        }
        else
        {
            firstItemIndex = secondItemIndex = thirdItemIndex = -1;
            GetRandomsNumbers();

            if (firstItemIndex > -1) SetupButton(0);
            if (secondItemIndex > -1) SetupButton(1);
            if (thirdItemIndex > -1) SetupButton(2);
        }
    }

    private void SetupButton(int i)
    {
        if (!CarAbilitiesManager.instance.IsPlayerFullAbilities())
        {
            buttonsItemsArray[i].gameObject.SetActive(true);
        
            var index = i switch
            {
                0 => firstItemIndex,
                1 => secondItemIndex,
                2 => thirdItemIndex,
                _ => 0
            };

            buttonsItemsArray[i].powerUpTitle.text = purchasableAbilities[index].abilityName;
            buttonsItemsArray[i].powerUpDescription.text = purchasableAbilities[index].description;
            buttonsItemsArray[i].powerUpSprite.sprite = purchasableAbilities[index].abilitySprite;
            buttonsItemsArray[i].powerUpButton.onClick.RemoveAllListeners();
            buttonsItemsArray[i].isNew.SetActive(!CarAbilitiesManager.instance.abilities.Contains(purchasableAbilities[index]));
            buttonsItemsArray[i].powerUpButton.onClick.AddListener(ExitShop);
        
            // Sinon
            buttonsItemsArray[i].powerUpButton.onClick.AddListener(() =>
            {
                CarAbilitiesManager.instance.AddAbility(purchasableAbilities[index]);
                CarExperienceManager.Instance.levelUpTokensAvailable--;
                if (CarAbilitiesManager.instance.abilities.Find(so => purchasableAbilities[index]).level == 2) purchasableAbilities.Remove(purchasableAbilities[index]);
            });
        }
        else
        {
            SetupGoldInShop();
        }
    }
    
    private int firstItemIndex, secondItemIndex, thirdItemIndex;
    private int[] GetRandomsNumbers()
    {
        int count = purchasableAbilities.Count;
        if (count > 2) // Tout va bien 
        {
            firstItemIndex = Random.Range(0, count);
            secondItemIndex = Random.Range(0, count);
            thirdItemIndex = Random.Range(0, count);

            if (firstItemIndex != secondItemIndex && secondItemIndex != thirdItemIndex && firstItemIndex != thirdItemIndex)
            {
                int[] a = new int[] { firstItemIndex, secondItemIndex, thirdItemIndex };
                return a;
            }
            else
            {
                GetRandomsNumbers();
            }
        }
        else if(count == 2)
        {
            firstItemIndex = Random.Range(0, count);
            secondItemIndex = Random.Range(0, count);

            if (firstItemIndex != secondItemIndex)
            {
                int[] a = { firstItemIndex, secondItemIndex };
                return a;
            }
            else
            {
                GetRandomsNumbers();
            }
        }
        else if (count == 1)
        {
            int[] a = { 0 };
            return a;
        }
        else
        {
            ExitShop();
        }

        return null;
    }
    
    private void ExitShop()
    {
        Time.timeScale = 1;
        UIManager.instance.shopScreen.SetActive(false);
        isShopActive = false;
    }

    public void SetupGoldInShop()
    {
        for (int i = 0; i < buttonsItemsArray.Length; i++)
        {
            buttonsItemsArray[i].gameObject.SetActive(true);
            buttonsItemsArray[i].powerUpTitle.text = gold.abilityName;
            buttonsItemsArray[i].powerUpDescription.text = gold.description;
            buttonsItemsArray[i].powerUpSprite.sprite = gold.abilitySprite;
            buttonsItemsArray[i].powerUpButton.onClick.RemoveAllListeners();
            buttonsItemsArray[i].isNew.SetActive(false);
            buttonsItemsArray[i].powerUpButton.onClick.AddListener(ExitShop);

            buttonsItemsArray[i].powerUpButton.onClick.AddListener(() =>
            {
                CarAbilitiesManager.instance.goldAmountWonOnRun += gold.effectDamage;
            });
        }
    }
}
