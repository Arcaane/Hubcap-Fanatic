using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeliveryRessourcesManager : MonoBehaviour
{
    private static DeliveryRessourcesManager _instance;
    public static DeliveryRessourcesManager Instance => _instance;
    
    [Header("Setup Options Spawn")]
    public List<Transform> spawnPoints;
    public List<GameObject> prefabObjects;
    private int previousSpawnIndex = -1;
    private bool canSpawn = true;

    [Header("Setup Options Delivery")] 
    public List<Transform> deliveryPoints;

    [Header("Debug Options")] 
    public bool enableGizmos;
    public int numberOfPointsPerSide;
    public float distanceBetweenPoints;
    private Transform spawnPointContainer;
    private GameObject actualObjectPick;

    void Start()
    {
        SpawnRandomObject();
    }
    
    void Awake()
    {
        _instance = this;
    }

    public void SpawnRandomObject()
    {
        if (!canSpawn) return;
        Transform spawnPoint = GetRandomSpawnPoint();
        GameObject prefabToSpawn = prefabObjects[Random.Range(0, prefabObjects.Count)];
        
        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity);
        IPickupable pickupableComponent = spawnedObject.GetComponent<IPickupable>();
        if (pickupableComponent != null)
        { 
            previousSpawnIndex = spawnPoints.IndexOf(spawnPoint);
        }
        else
        {
            Debug.LogError("L'objet instancié ne contient pas de composant IPickupable.");
        }
    }

    public void RemoveDelivery()
    {
        actualObjectPick = null;
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
    
    [ContextMenu("Reset Pos Vector Container")]
    void ResetPosVectorContainer()
    {
        if (spawnPointContainer != null)
        {
            spawnPointContainer.position = transform.position;
        }
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
    
    [ContextMenu("Pickup Object")]
    public void ObjectPickedUp()
    {
        if (actualObjectPick != null)
        {
            actualObjectPick.GetComponent<IPickupable>().OnPickedUp();
        }
        SpawnRandomObject();
    }
    

    private void OnDrawGizmos()
    {
        if (!enableGizmos) return;
        foreach (Transform sp in spawnPoints)
        {
            Gizmos.DrawSphere(sp.position, 0.5f);
        }
    }
}