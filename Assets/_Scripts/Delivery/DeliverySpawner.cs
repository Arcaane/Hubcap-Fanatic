using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverySpawner : MonoBehaviour
{
    [Header("Setup Options Spawn")] 
    public int numberOfPointsPerSide;
    public float distanceBetweenPoints;
    [Range(0,65565)]
    public int randomSeed;
    
    [Header("Timeline Spawn Options")]
    [SerializeField] private GameObject deliveryPrefab;
    [SerializeField] private List<DeliveryObject> deliveryObjects;
    
    
    //Setup Spawn Points
    private Transform spawnPointContainer;
    [SerializeField] private List<Transform> spawnPoints;
    private int previousSpawnIndex = -1;
    

    private void Start()
    {
        Random.InitState(randomSeed);
        StartCoroutine(SpawnDeliveryTimeline());
    }

    private IEnumerator SpawnDeliveryTimeline()
    {
        foreach (var deliveryObject in deliveryObjects)
        {
            yield return new WaitForSeconds(deliveryObject.X_DeliveryDuration__Y_TimeBeforeSpawn.y);
            SpawnDeliveryPrefab(deliveryObject.prefab, deliveryObject.X_DeliveryDuration__Y_TimeBeforeSpawn.x);
        }
    }
    
    private void SpawnDeliveryPrefab(GameObject prefab, float deliveryDuration)
    {
        Transform randomSpawnPoint = GetRandomSpawnPoint();
        GameObject deliveryZone = Instantiate(prefab, randomSpawnPoint.position, Quaternion.identity);
        SpawnZoneDelivery spawnZoneDelivery = deliveryZone.GetComponent<SpawnZoneDelivery>();
        UIIndic.instance.AddIndic(deliveryZone, TargetType.DropZone, out int index);
        spawnZoneDelivery.index = index;
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