using System.Collections.Generic;
using UnityEngine;

public class CarExperienceManager : MonoBehaviour
{
    private static CarExperienceManager instance;
    public static CarExperienceManager Instance => instance;
    
    private UIManager UIManager;
    [SerializeField] public int playerLevel;
    [SerializeField] private AnimationCurve expCurve;
    [SerializeField] private List<int> xpPerLevel = new();
    [SerializeField] private int healToAddWhenLvlUp = 10;
    [SerializeField] private int currentExperienceAmount;
    [SerializeField] public int levelUpTokensAvailable;
    [SerializeField] public TestShop shop = null;

    [SerializeField] public float nbrOfDelivery = 1;
    
    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UIManager = UIManager.instance;
        UpdateUIData();
    }

    public void GetExp(int i) {
        currentExperienceAmount += i;
        if(CheckLevelUp()) LevelUp();
        UpdateUIData();
    }

    /// <summary>
    /// Calculate the sum of all experience for the previous levels
    /// </summary>
    /// <param name="maxLevel"></param>
    /// <returns></returns>
    private int SumOfPreviousLevel(int maxLevel) {
        int sumXP = 0;
        for (int level = 0; level < maxLevel; level++) {
            sumXP += xpPerLevel[level];
        }

        return sumXP;
    }

    /// <summary>
    /// Check if the player needs to level up
    /// </summary>
    /// <returns></returns>
    private bool CheckLevelUp() => currentExperienceAmount >= SumOfPreviousLevel(playerLevel + 1);

    /// <summary>
    /// Make the player level up
    /// </summary>
    private void LevelUp() {
        AddToken(1);
        CarHealthManager.instance.TakeHeal(healToAddWhenLvlUp);
        shop.StartShopUI();
        playerLevel++;
        UpdateUIData();

        if(CheckLevelUp()) LevelUp();
    }

    /// <summary>
    /// Add a token for upgrades and new items
    /// </summary>
    /// <param name="i"></param>
    public void AddToken(int i) {
        levelUpTokensAvailable += i;
    }
    
    /// <summary>
    /// Update the UI visuals of the level slider
    /// </summary>
    private void UpdateUIData() => UIManager.UpdateExperienceData((float)(currentExperienceAmount - (playerLevel == 0 ? 0 : SumOfPreviousLevel(playerLevel))) /xpPerLevel[playerLevel], $"LEVEL {(playerLevel+1<10?"0":"") + (playerLevel + 1)}");
}
