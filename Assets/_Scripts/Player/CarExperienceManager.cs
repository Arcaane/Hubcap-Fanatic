using System.Collections.Generic;
using Helper;
using HubcapCarBehaviour;
using UnityEngine;

public class CarExperienceManager : Singleton<CarExperienceManager> {
    [SerializeField] private UIManager uiManager = null;
    [SerializeField] public int playerLevel = 0;
    [SerializeField] private AnimationCurve expCurve = new();
    [SerializeField] private List<int> xpPerLevel = new();
    [SerializeField] private int healToAddWhenLvlUp = 10;
    private int currentExperienceAmount = 0;
    [HideInInspector] public int levelUpTokensAvailable = 0;
    [SerializeField] public TestShop shop = null;

    [SerializeField] public float nbrOfDelivery = 1;
    
    private void Start() {
        uiManager = UIManager.Instance;
        uiManager.UpdateLevelSlider(0, playerLevel + 1);
    }
    
    /// <summary>
    /// Method called when player receive xp
    /// </summary>
    /// <param name="i"></param>
    public void GetPlayerExperience(int i) {
        currentExperienceAmount += i;
        uiManager.UpdateLevelSlider((float)getCurrentXpMinusPreviousLevel() / xpPerLevel[playerLevel + 1], playerLevel + 1);
        
        CheckLevelUp();
    }

    private int getCurrentXpMinusPreviousLevel() => currentExperienceAmount - GetXpAmountAtLevel(playerLevel);
    
    /// <summary>
    /// Get the amount of Xp to reach the next level
    /// </summary>
    /// <returns></returns>
    private int GetXpAmountAtLevel(int level) {
        int xp = 0;
        for (int i = 0; i < level; i++) xp += xpPerLevel[i];
        return xp;
    }

    #region Levels
    /// <summary>
    /// Check if the player needs to level up
    /// </summary>
    private void CheckLevelUp() {
        if (currentExperienceAmount <= GetXpAmountAtLevel(playerLevel + 1)) return;
        LevelUp();
    }

    /// <summary>
    /// Player needs to level up
    /// </summary>
    private void LevelUp() {
        playerLevel++;
        uiManager.UpdateLevelSlider(getCurrentXpMinusPreviousLevel() / xpPerLevel[playerLevel + 1], playerLevel + 1);
        
        CarHealthManagerOld.instance.TakeHeal(healToAddWhenLvlUp);
        
        AddToken(1);
        shop.StartShopUI();
        
        CheckLevelUp();
    }
    #endregion Levels
    
    /// <summary>
    /// Add a token to spare in the shop later
    /// </summary>
    /// <param name="i"></param>
    public void AddToken(int i) => levelUpTokensAvailable += i;
}
