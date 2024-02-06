using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster instance;
    public GameData gameData = new GameData();

    public int PlayerGold => playerGold;
    private int playerGold;
    
    // Save Methods Library
    private void SaveGame() => SaveManager.Instance.SaveGame(gameData);
    private void LoadGame() => gameData = SaveManager.Instance.LoadGame();
    public void AddGold(int i) => playerGold += i;
    public void SubtractGold(int i) => playerGold -= i;
    
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
        playerGold = gameData.saveGold;
    }

    private void OnApplicationQuit()
    {
        gameData.saveGold = playerGold;
        SaveGame();
    }
}