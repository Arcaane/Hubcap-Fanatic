using System;
using UnityEngine;

public class CarExperienceManager : MonoBehaviour
{
    private static CarExperienceManager instance;
    public static CarExperienceManager Instance => instance;

    [SerializeField] private UIManager uiManager;
    [SerializeField] private int currentPlayerLevel;
    [SerializeField] private AnimationCurve expCurve;
    [SerializeField] private int expBeforeNextLevelAmount;
    [SerializeField] private int currentExperienceAmount;
    [SerializeField] public int levelUpTokensAvailable;
    
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
    }

    private void SetupNextLevelData()
    {
        expBeforeNextLevelAmount += Mathf.FloorToInt(expCurve.Evaluate(currentPlayerLevel));
    }

    public void GetExp(int i)
    {
        currentExperienceAmount += i;
        CheckLevelUp();
        
        uiManager.SetExperienceFillAmount((float)currentExperienceAmount/expBeforeNextLevelAmount);
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
        currentPlayerLevel++;
        uiManager.SetLevelPlayerText(currentPlayerLevel + 1);
        SetupNextLevelData();
    }

    public void AddToken(int i) => levelUpTokensAvailable += i;
}
