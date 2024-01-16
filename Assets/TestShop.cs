using System.Collections.Generic;
using Abilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestShop : MonoBehaviour
{
    [SerializeField] private List<AbilitiesSO> allAbilities;
    [SerializeField] private List<AbilitiesSO> purchasableAbilities;
    
    
    [SerializeField] private ButtonsItems[] itemsHandler;
    private bool isShopActive;

    private void Start()
    {
        UIManager.instance.shopScreen.SetActive(false);
        isShopActive = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartShopUI();
        }
    }

    private void StartShopUI()
    {
        Time.timeScale = 0;
        UIManager.instance.shopScreen.SetActive(true);
        isShopActive = true;
        EventSystem.current.SetSelectedGameObject(itemsHandler[0].gameObject);
        SetupItemsInShop();
    }

    private void SetupItemsInShop()
    {
        firstItemIndex = secondItemIndex = thirdItemIndex = -1;
        GetRandomsNumbers();

        if (firstItemIndex < -1)
        {
            SetupButton(0);
            Debug.Log("First item setup");
        }

        if (secondItemIndex < -1)
        {
            SetupButton(1);
            Debug.Log("Second item setup");
        }

        if (thirdItemIndex < -1)
        {
            SetupButton(2);
            Debug.Log("Third item setup");
        }
    }

    private void SetupButton(int i)
    {
        var index = i switch
        {
            0 => firstItemIndex,
            1 => secondItemIndex,
            2 => thirdItemIndex,
            _ => 0
        };

        itemsHandler[i].powerUpTitle.text = purchasableAbilities[index].abilityName;
        itemsHandler[i].powerUpDescription.text = purchasableAbilities[index].description;
        itemsHandler[i].powerUpSprite.sprite = purchasableAbilities[index].abilitySprite;
        //itemsHandler[i].isNew.SetActive(CarAbilitiesManager.instance.); 
        //itemsHandler[i].powerUpButton
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
            Debug.Log("Pas d'items a vendre");
        }

        return null;
    }
    
    private void ExitShop()
    {
        Time.timeScale = 1;
        UIManager.instance.shopScreen.SetActive(false);
        isShopActive = false;
    }
}
