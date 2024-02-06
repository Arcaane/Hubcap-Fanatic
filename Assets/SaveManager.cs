using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveManager : MonoBehaviour
{
    private static SaveManager instance;
    public static SaveManager Instance => instance;
    
    private string filePath;

    private void Awake()
    {
        filePath = Application.persistentDataPath + "/save.data";

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void SaveGame(GameData gameData)
    {
        FileStream dataStream = new FileStream(filePath, FileMode.Create);
        BinaryFormatter converter = new BinaryFormatter();
        converter.Serialize(dataStream, gameData);
        dataStream.Close();
    }

    public GameData LoadGame()
    {
        if(File.Exists(filePath))
        {
            // File exists 
            FileStream dataStream = new FileStream(filePath, FileMode.Open);

            BinaryFormatter converter = new BinaryFormatter();
            GameData saveData = converter.Deserialize(dataStream) as GameData;

            dataStream.Close();
            return saveData;
        }
        
        // File does not exist
        Debug.LogError("Save file not found in " + filePath);
        return null;
    }
}


[System.Serializable]
public class GameData
{
    public int saveGold = 0;
    
    public void AddGold(int amount) => saveGold += amount;
    public void SubtractGold(int amount) => saveGold -= amount;
    
    public void ResetData()
    {
        saveGold = 0;
    }
}
