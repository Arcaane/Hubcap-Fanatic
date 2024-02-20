using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    public static GameMaster instance;
    public GameData gameData = new GameData();

    public int PlayerGold => playerGold;
    private int playerGold;

    public bool[] UnlockedCars => unlockedCars;
    private bool[] unlockedCars = new[] { true, false, false };

    public int[] UnlockedPowerUps => unlockedPowerUps;
    private int[] unlockedPowerUps = new int[3];
    
    // Save Methods Library
    private void SaveGame() => SaveManager.Instance.SaveGame(gameData);
    private void LoadGame() => gameData = SaveManager.Instance.LoadGame();
    public void AddGold(int i) => playerGold += i;
    public void SubtractGold(int i) => playerGold -= i;
    public void UnlockCar(int i) => unlockedCars[i] = true;
    public void UnlockPUp(int i) => unlockedPowerUps[i]++;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    
    private void Start()
    {
        DontDestroyOnLoad(this);
        LoadGame();

        CommandConsole ADDGOLD = new CommandConsole("AddGold", "Add gold <int>", new List<CommandClass>() {new(typeof(int))}, (value) =>
        {
            AddGold(int.Parse(value[0]));
        });
        
        
        CommandConsole RESETGAMEDATA = new CommandConsole("ResetGameData", "Reset all game data", new List<CommandClass>() {new(null)}, (value) =>
        {
            ResetAllGame();
        });
        
        CommandConsole QUIT = new CommandConsole("QUIT", "Quit and Save game", new List<CommandClass>() {new(null)}, (value) =>
        {
            Application.Quit();
        });

        CommandConsoleRuntime.Instance.AddCommand(RESETGAMEDATA);
        CommandConsoleRuntime.Instance.AddCommand(QUIT);
        CommandConsoleRuntime.Instance.AddCommand(ADDGOLD);
        
        playerGold = gameData.saveGold;
        for (int i = 0; i < gameData.saveUnlockedCar.Length; i++) {
            unlockedCars[i] = gameData.saveUnlockedCar[i];
        }

        for (int i = 0; i < gameData.saveUnlockedPowerUps.Length; i++)
        {
            unlockedPowerUps[i] = gameData.saveUnlockedPowerUps[i];
        }
    }

    [ContextMenu("ResetAllGame")]
    public void ResetAllGame()
    {
        gameData.ResetData();
        SaveGame();
        SceneManager.LoadScene(0);
    }

    public void Save()
    {
        gameData.saveGold = playerGold; // Save golds

        gameData.saveUnlockedCar = new bool[unlockedCars.Length];
        gameData.saveUnlockedPowerUps = new int[unlockedPowerUps.Length];
        
        for (int i = 0; i < unlockedCars.Length; i++) {   // Save unlocked Cars
            gameData.saveUnlockedCar[i] = unlockedCars[i];
        }

        for (int i = 0; i < unlockedPowerUps.Length; i++) {   // Save unlocked PowersUps
            gameData.saveUnlockedPowerUps[i] = unlockedPowerUps[i];
        }
        
        SaveGame();
    }
    
    private void OnApplicationQuit()
    {
        Save();
    }
}