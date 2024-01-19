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

        if (spawnZoneDelivery != null)
        {
            UIIndic.instance.AddIndic(deliveryZone, TargetType.DropZone, out int index);
            spawnZoneDelivery.index = index;
            
            int objectIndex = deliveryObjects.FindIndex(obj => obj.prefab == prefab);

            if (objectIndex != -1)
            {
                objectIndex++;
                deliveryZone.name = $"{prefab.name}_Index_{objectIndex}";
            }
            else
            {
                Debug.LogError("Prefab not found in the deliveryObjects list.");
            }

            spawnZoneDelivery.DeliveryDuration = deliveryDuration;
        }
        else
        {
            Debug.LogError("The prefab does not contain the SpawnZoneDelivery script.");
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
    
    [ContextMenu("Delivery Zone -> Add 10 Random Delivery Objects")]
    void Add10RandomDeliveryZone()
    {
        AddRandomDeliveryObjects(10); 
    }
    
    [ContextMenu("Delivery Zone -> Add 100 Random Delivery Objects")]
    void Add100RandomDeliveryZone()
    {
        AddRandomDeliveryObjects(100); 
    }

    void AddRandomDeliveryObjects(int numberOfObjects)
    {
        GameObject deliveryZonePrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Prefabs/Player Attached/Event_SpawnDeliveryZone.prefab");
        if (deliveryZonePrefab == null)
        {
            Debug.LogError("Prefab not found. Make sure the path is correct.");
            return;
        }

        for (int i = 0; i < numberOfObjects; i++)
        {
            DeliveryObject randomDeliveryObject = new DeliveryObject
            {
                prefab = deliveryZonePrefab,
                X_DeliveryDuration__Y_TimeBeforeSpawn = new Vector2(Random.Range(1.0f, 5.0f), Random.Range(0.0f, 30.0f))
            };

            deliveryObjects.Add(randomDeliveryObject);
        }

        Debug.Log($"Added {numberOfObjects} random Delivery Objects.");
    }


    [ContextMenu("Delivery Zone -> Clear Delivery Objects")]
    void ClearDeliveryZone()
    {
        deliveryObjects.Clear();
        Debug.Log("Delivery objects cleared.");
    }
    
    [ContextMenu("Clear Delivery Objects")]
    void ClearDeliveryObjects()
    {
        foreach (var deliveryPoint in deliveryPoints)
        {
            DestroyImmediate(deliveryPoint);
        }
        deliveryPoints.Clear();
        Debug.Log("Delivery objects cleared.");
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