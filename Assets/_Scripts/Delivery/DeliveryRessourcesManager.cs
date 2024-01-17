using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class DeliveryObject
{
    public GameObject prefab;
    public Vector2 X_DeliveryDuration__Y_TimeBeforeSpawn;
}

public class DeliveryRessourcesManager : MonoBehaviour
{
    private static DeliveryRessourcesManager _instance;
    public static DeliveryRessourcesManager Instance => _instance;
    
    [Header("Setup Options Spawn")] 
    public int numberOfPointsPerSide;
    public float distanceBetweenPoints;
    [Range(0,65565)]
    public int randomSeed;
    
    [Header("Timeline Spawn Options")]
    [SerializeField] private List<DeliveryObject> deliveryObjects; 
    public List<GameObject> deliveryPoints;
    
    //Setup Spawn Points
    private Transform spawnPointContainer;
    [SerializeField] private List<Transform> spawnPoints;
    private int previousSpawnIndex = -1;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        Random.InitState(randomSeed);
        foreach (var deliveryObject in deliveryObjects)
        {
            StartCoroutine(SpawnDeliveryTimeline(deliveryObject));
        }
    }

    private IEnumerator SpawnDeliveryTimeline(DeliveryObject deliveryObject)
    {
        yield return new WaitForSeconds(deliveryObject.X_DeliveryDuration__Y_TimeBeforeSpawn.y);
        SpawnDeliveryPrefab(deliveryObject.prefab, deliveryObject.X_DeliveryDuration__Y_TimeBeforeSpawn.x, deliveryObject.X_DeliveryDuration__Y_TimeBeforeSpawn.y);
    }
    
    private void SpawnDeliveryPrefab(GameObject prefab, float deliveryDuration, float timeBeforeSpawn)
    {
        Transform randomSpawnPoint = GetRandomSpawnPoint();
        GameObject deliveryZone = Instantiate(prefab, randomSpawnPoint.position, Quaternion.identity);
        SpawnZoneDelivery spawnZoneDelivery = deliveryZone.GetComponent<SpawnZoneDelivery>();
        UIIndic.instance.AddIndic(deliveryZone, TargetType.DropZone, out int index);
        spawnZoneDelivery.index = index;
        Debug.Log($"{spawnZoneDelivery.gameObject.name} - Delivery Duration: {deliveryDuration} - Time Before Spawn: {timeBeforeSpawn}");
        if (spawnZoneDelivery != null)
        {
            spawnZoneDelivery.DeliveryDuration = deliveryDuration;
        }
        else
        {
            Debug.LogError("Le prefab ne contient pas le script SpawnZoneDelivery.");
        }
    }
    
    Transform GetRandomSpawnPoint()
    {
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, spawnPoints.Count);
        } while (randomIndex == previousSpawnIndex);

        return spawnPoints[randomIndex];
    }
    
    [ContextMenu("Create Square Spawn Points")]
    void CreateSquareSpawnPoints()
    {
        ClearSpawnPoints();
        if (spawnPointContainer == null)
        {
            spawnPointContainer = new GameObject("SpawnPointContainer").transform;
            spawnPointContainer.transform.parent = transform;
        }
        
        spawnPointContainer.position = transform.position;
        
        float halfSide = (numberOfPointsPerSide - 1) * distanceBetweenPoints / 2f;

        for (int i = 0; i < numberOfPointsPerSide; i++)
        {
            for (int j = 0; j < numberOfPointsPerSide; j++)
            {
                Vector3 spawnPosition = new Vector3(
                    i * distanceBetweenPoints - halfSide + spawnPointContainer.position.x,
                    spawnPointContainer.position.y,
                    j * distanceBetweenPoints - halfSide + spawnPointContainer.position.z
                );

                Transform spawnPoint = new GameObject("SpawnPoint_" + i + "_" + j).transform;
                spawnPoint.position = spawnPosition;
                spawnPoint.parent = spawnPointContainer.transform;
                spawnPoints.Add(spawnPoint);
            }
        }
        Debug.Log("Square spawn points created.");
    }
    
    [ContextMenu("Clear Spawn Points")]
    void ClearSpawnPoints()
    {
        if (spawnPointContainer != null)
        {
            foreach (Transform spawnPoint in spawnPoints)
            {
                DestroyImmediate(spawnPoint.gameObject);
            }
        }

        spawnPoints.Clear();
        Debug.Log("Spawn points cleared.");
    }
}