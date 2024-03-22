using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DataExtract : MonoBehaviour {
    [SerializeField] private float timeBetweenSave = 0.1f;
    [SerializeField] private List<DataSave> datas = new();
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
        datas.Add(new DataSave(Time.time, playerCar.position, "Player", ""));
    }
    
    public void SaveNewData(Vector3 position, string eventType, string description = "") {
        datas.Add(new DataSave(Time.time, position, eventType, description));
    }

    [ContextMenu("Save Data To JSON")]
    public void SaveToJSON() {
        DataConverter jsonObj = new DataConverter(datas);
        string DataJSON = JsonUtility.ToJson(jsonObj);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/Data.json", DataJSON);
    }
}

[Serializable]
public class DataSave {
    public float timer = 0;
    public Vector3 position = new();
    public string eventType;
    public string description;

    public DataSave(float timer, Vector3 position, string eventType, string description) {
        this.timer = timer;
        this.position = position;
        this.eventType = eventType;
        this.description = description;
    }
}

public class DataConverter {
    public List<DataSave> datas;
    
    public DataConverter(List<DataSave> datas) {
        this.datas = datas;
    }
}
