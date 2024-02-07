using System;
using TMPro;
using UnityEngine;

public class PowerUpMenu : MonoBehaviour
{
    public MacroShopItem[] items;
    public MacroItem[] slots;

    private int index = 0;
    
    [ContextMenu("Setup")]
    public void SetupItems()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (slots[i]) continue;
            slots[i].image.sprite = items[i].icon;
            slots[i].text.text = slots[i].text2.text = items[i].name;
            slots[i].selectFeedbackObj.SetActive(false);
        }
        
        SetDescription();
    }

    public void StartShop()
    {
        index = 0;
        SetDescription();
    }
    
    public TextMeshProUGUI desc, desc1;  
    public TextMeshProUGUI price, price1;    
    public void SetDescription()
    {
        desc.text = desc1.text = items[index].description;
        price1.text = price.text = items[index].price[items[index].currentLevel].ToString();
    }
}

[Serializable]
public class MacroShopItem
{
    public string name;
    public string description;
    public Sprite icon;
    public int[] price;
    public int currentLevel;
}
