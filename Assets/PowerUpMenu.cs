using System;
using UnityEngine;

public class PowerUpMenu : MonoBehaviour
{
    public MacroShopItem[] items;
}

[Serializable]
public class MacroShopItem
{
    public string name;
    [TextArea] public string description;
    public Sprite icon;
    public int[] price;
    public int currentLevel;
}
