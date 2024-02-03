using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CarExperienceManager : MonoBehaviour
{
    private static CarExperienceManager instance;
    public static CarExperienceManager Instance => instance;

    [SerializeField] private UIManager uiManager;
    [SerializeField] public int playerLevel;
    [SerializeField] private AnimationCurve expCurve;
    [SerializeField] private List<int> xpPerLevel = new();
    [SerializeField] private int expBeforeNextLevelAmount;
    [SerializeField] private int healToAddWhenLvlUp = 10;
    [SerializeField] private int currentExperienceAmount;
    [SerializeField] public int levelUpTokensAvailable;
    [SerializeField] public TestShop shop = null;
    
    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        uiManager = UIManager.instance;
        SetupNextLevelData();
        uiManager.SetLevelPlayerText(playerLevel + 1);
    }

    private void SetupNextLevelData()
    {
        expBeforeNextLevelAmount += Mathf.FloorToInt(xpPerLevel[Mathf.Clamp(playerLevel, 0, xpPerLevel.Count - 1)]);
    }

    public void GetExp(int i)
    {
        currentExperienceAmount += i;
        CheckLevelUp();
        
        if (playerLevel < 1)
        {
            uiManager.SetExperienceFillAmount((float)currentExperienceAmount/(expBeforeNextLevelAmount));
        }
        else
        {
            uiManager.SetExperienceFillAmount((float)(currentExperienceAmount - xpPerLevel[playerLevel-1])/(expBeforeNextLevelAmount - xpPerLevel[playerLevel-1]));
        }
    }

    private void CheckLevelUp()
    {
        if (currentExperienceAmount >= expBeforeNextLevelAmount)
        {
            LevelUp();
            CheckLevelUp();
        }
    }

    private void LevelUp()
    {
        AddToken(1);
        CarHealthManager.instance.TakeHeal(healToAddWhenLvlUp);
        shop.StartShopUI();
        playerLevel++;
        uiManager.SetLevelPlayerText(playerLevel + 1);
        SetupNextLevelData();
    }

    public void AddToken(int i)
    {
        levelUpTokensAvailable += i;
    }
}
