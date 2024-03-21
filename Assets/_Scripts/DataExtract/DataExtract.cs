using System;
using System.Collections.Generic;
using UnityEngine;

public class DataExtract : MonoBehaviour {
    [SerializeField] private float timeBetweenSave = 0.1f;
    [SerializeField] private List<PlayerDataSave> playerData = new();
    [SerializeField] private List<DeliveryDataSave> deliveryData = new();
    [Space] 
    [SerializeField] private Transform playerCar = null;
    private float currentTimer = 0;

    private void Start() {
        currentTimer = timeBetweenSave;
    }

    private void Update() {
        currentTimer += Time.deltaTime;
        if (currentTimer >= timeBetweenSave) {
            SaveData();
            currentTimer = 0;
        }
    }

    /// <summary>
    /// Save the player data
    /// </summary>
    private void SaveData() {
        playerData.Add(new PlayerDataSave(Time.time, playerCar.position, playerCar.rotation));
    }

    /// <summary>
    /// Save the delivery data
    /// </summary>
    /// <param name="position"></param>
    public void SaveDeliverySpawn(Vector3 position) {
        deliveryData.Add(new DeliveryDataSave(Time.time, position));
    }

    [ContextMenu("Save Data To JSON")]
    public void SaveToJSON() {
        JSONSaver jsonObj = new JSONSaver(playerData, deliveryData);
        string DataJSON = JsonUtility.ToJson(jsonObj);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/Data.json", DataJSON);
    }
}

[System.Serializable]
public class PlayerDataSave {
    public float timer = 0;
    public Vector3 position = new();
    public Quaternion rotation = new();

    public PlayerDataSave(float timer, Vector3 position, Quaternion rotation) {
        this.timer = timer;
        this.position = position;
        this.rotation = rotation;
    }
}

[System.Serializable]
public class DeliveryDataSave {
    public float timer = 0;
    public Vector3 position = new();

    public DeliveryDataSave(float timer, Vector3 position) {
        this.timer = timer;
        this.position = position;
    }
}

public class JSONSaver {
    public List<PlayerDataSave> playerData;
    public List<DeliveryDataSave> deliveryData;
    
    public JSONSaver(List<PlayerDataSave> playerData, List<DeliveryDataSave> deliveryData) {
        this.playerData = playerData;
        this.deliveryData = deliveryData;
    }
}
