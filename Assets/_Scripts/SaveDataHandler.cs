using UnityEngine;
using System.IO;
using HubcapManager;

public class SaveDataHandler : MonoBehaviour {
    private string saveFilePath = "";
    private static readonly string keyWord = "48fHaL4363Icvl0135Baols8763KparIc02";

    private void Awake() {
        saveFilePath = Application.persistentDataPath + "/savedData.json";
    }

    /// <summary>
    /// Save game data on the user pc
    /// </summary>
    /// <param name="data"></param>
    public void SaveGameData(SavedData data) {
        string dataJSON = JsonUtility.ToJson(data);
        if (GameManager.Instance.UseEncryptedData) dataJSON = EncryptDecryptData(dataJSON);
        File.WriteAllText(saveFilePath, dataJSON);
    }

    /// <summary>
    /// Load the saved data on disk
    /// </summary>
    /// <returns></returns>
    public SavedData LoadGameData() {
        if (!File.Exists(saveFilePath)) {
            SaveGameData(new SavedData());
            return new SavedData();
        }

        string dataJSON = File.ReadAllText(saveFilePath);
        SavedData data = JsonUtility.FromJson<SavedData>(GameManager.Instance.UseEncryptedData ? EncryptDecryptData(dataJSON) : dataJSON);
        return data;
    }

    /// <summary>
    /// Reset game data
    /// </summary>
    public void ResetGameData() => SaveGameData(new SavedData());

    /// <summary>
    /// Encrypt Data
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private string EncryptDecryptData(string data) {
        string result = "";
        for (int i = 0; i < data.Length; i++) {
            result += (char) (data[i] ^ keyWord[i % keyWord.Length]);
        }
        return result;
    }
}

[System.Serializable]
public class SavedData {
    public int goldAmount = 0;
    public string language = "";
}