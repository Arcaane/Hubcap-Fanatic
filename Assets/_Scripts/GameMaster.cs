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
    
    // Save Methods Library
    private void SaveGame() => SaveManager.Instance.SaveGame(gameData);
    private void LoadGame() => gameData = SaveManager.Instance.LoadGame();
    public void AddGold(int i) => playerGold += i;
    public void SubtractGold(int i) => playerGold -= i;
    public void UnlockCar(int i) => unlockedCars[i] = true;
    
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
        
        CommandConsole RESETGAMEDATA = new CommandConsole("ResetGameData", "Reset all game data", new List<CommandClass>() {new(null)}, (value) =>
        {
            ResetAllGame();
        });

        CommandConsoleRuntime.Instance.AddCommand(RESETGAMEDATA);
        
        playerGold = gameData.saveGold;
        for (int i = 0; i < gameData.unlockedCar.Length; i++) {
            unlockedCars[i] = gameData.unlockedCar[i];
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
        
        for (int i = 0; i < gameData.unlockedCar.Length; i++) { // Save unlocked Cars
            gameData.unlockedCar[i] = unlockedCars[i];
        }
        
        SaveGame();
    }
    
    private void OnApplicationQuit()
    {
        Save();
    }
}