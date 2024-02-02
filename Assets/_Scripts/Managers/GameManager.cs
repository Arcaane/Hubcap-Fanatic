using Abilities;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public CarController controller;
    public CarAbilitiesManager abilitiesManager;
    public CarHealthManager healthManager;
    public UIManager uiManager;

    public float gameTimer = 0;
    public int score = 0;

    private void Awake()
    {
        instance = this;
    }

    public void SetupGame()
    {
        //abilitiesManager.Setup();
    }
}
