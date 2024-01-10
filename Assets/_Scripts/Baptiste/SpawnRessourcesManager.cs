using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnRessourcesManager : MonoBehaviour
{
    public List<Transform> spawnPoints;
    public List<GameObject> prefabObjects;
    private int previousSpawnIndex = -1;

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

    void SpawnRandomObject()
    {
        Transform spawnPoint = GetRandomSpawnPoint();
        GameObject prefabToSpawn = prefabObjects[Random.Range(0, prefabObjects.Count)];

        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity);
        actualObjectPick = spawnedObject;
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
            GameObject spawnPointsParent = new GameObject("SpawnPointFolder");
            spawnPointsParent.transform.parent = transform;
            spawnPointContainer = spawnPointsParent.transform;
        }
        
        for (int i = 0; i < numberOfPointsPerSide; i++)
        {
            for (int j = 0; j < numberOfPointsPerSide; j++)
            {
                Vector3 spawnPosition = new Vector3(i * distanceBetweenPoints, 0f, j * distanceBetweenPoints);
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