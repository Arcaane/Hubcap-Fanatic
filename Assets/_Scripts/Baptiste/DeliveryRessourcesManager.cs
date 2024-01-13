using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class DeliveryRessourcesManager : MonoBehaviour
{
    private static DeliveryRessourcesManager _instance;
    public static DeliveryRessourcesManager Instance => _instance;
    
    [Header("Setup Options Spawn")]
    public List<Transform> spawnPoints;
    
    [Header("Capture Zone Options")]
    public GameObject captureZone;
    public int numberOfCapturesAtStart;
    [Range(0,65565)]
    public int randomSeed;
    [Header("Permently Capture Zone Options")]
    public float spawnInterval = 30f;
    public float timeSinceLastSpawn = 0f;
    private SpawnZoneDelivery spawnZoneInstance;

        
    public List<GameObject> prefabObjects;
    private int previousSpawnIndex = -1;
    private int selectedSpawnIndex = -1;
    private bool canSpawn = true;

    [Header("Setup Options Delivery")] 
    public List<Transform> deliveryPoints;

    [Header("Debug Options")] 
    public bool enableGizmos;
    public int numberOfPointsPerSide;
    public float distanceBetweenPoints;
    private Transform spawnPointContainer;
    private GameObject actualObjectPick;


    void Awake()
    {
        _instance = this;
    }
    
    void Start()
    {
        randomSeed = PlayerPrefs.GetInt("RandomSeed", randomSeed);
        numberOfCapturesAtStart = PlayerPrefs.GetInt("NumberOfCapturesAtStart", numberOfCapturesAtStart);
        Random.InitState(randomSeed);
        SpawnCaptureZones();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ReloadScene();
        }
        if (spawnZoneInstance != null && !spawnZoneInstance.HasDelivered)
        {
            return;
        }
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= spawnInterval)
        {
            if (spawnZoneInstance != null && spawnZoneInstance.currenSpawnState != SpawnDeliveryState.IsOrNotDelivered)
            {
                Destroy(spawnZoneInstance.gameObject);
            }
            SpawnCaptureZone();
            timeSinceLastSpawn = 0f;
        }
    }
    
    private void OnGUI()
    {
        /*
        GUI.Label(new Rect(50, 50, 2003, 1003), "Press F1 to reload scene");

        randomSeed = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(50, 70, 200, 20), randomSeed, 0f, 100000f));
        GUI.Label(new Rect(260, 70, 150, 20), "Seed Number: " + randomSeed.ToString());

        numberOfCapturesAtStart = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(50, 100, 200, 20), numberOfCapturesAtStart, 0f, 100f));
        GUI.Label(new Rect(260, 100, 150, 20), "Captures Number: " + numberOfCapturesAtStart.ToString());

        if (Event.current.type == EventType.Repaint)
        {
            PlayerPrefs.SetInt("RandomSeed", randomSeed);
            PlayerPrefs.SetInt("NumberOfCapturesAtStart", numberOfCapturesAtStart);
            PlayerPrefs.Save();
        }
        */
    }

    
    private void ReloadScene()
    {
        PlayerPrefs.DeleteKey("RandomSeed");
        PlayerPrefs.DeleteKey("NumberOfCapturesAtStart");
        SceneManager.LoadScene(0);
    }

    void SpawnCaptureZone()
    {
        int tempRandomSeed = System.Environment.TickCount;
        Random.InitState(tempRandomSeed);

        Transform randomSpawnPoint = GetRandomSpawnPoint();
        GameObject capturedZoneObject = Instantiate(captureZone, randomSpawnPoint.position, Quaternion.identity);
        spawnZoneInstance = capturedZoneObject.GetComponent<SpawnZoneDelivery>();    
    }
    
    void SpawnCaptureZones()
    {
        for (int i = 0; i < numberOfCapturesAtStart; i++)
        {
            Transform randomSpawnPoint = GetRandomSpawnPoint();
            selectedSpawnIndex = spawnPoints.IndexOf(randomSpawnPoint); 
            Instantiate(captureZone, randomSpawnPoint.position, Quaternion.identity);
        }
    }

    public GameObject SpawnObject(Vector3 spawnPoint)
    {
        GameObject spawnedObject = Instantiate(prefabObjects[0], spawnPoint + new Vector3(0, 1.5f, 0), Quaternion.identity);
        actualObjectPick = spawnedObject;  // Pour Debug
        return spawnedObject;
    }

    
    
    /*
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
    */

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
        //SpawnRandomObject();
    }
    void OnDrawGizmos()
    {
        if (!enableGizmos || spawnPoints.Count == 0 || numberOfCapturesAtStart <= 0) return;

        Random.InitState(randomSeed);

        // Iterate through the capture zones
        for (int i = 0; i < numberOfCapturesAtStart; i++)
        {
            // Ensure that selectedSpawnIndex is within the valid range
            selectedSpawnIndex = Random.Range(0, spawnPoints.Count);

            // Visualize spawn points
            foreach (Transform sp in spawnPoints)
            {
                Gizmos.color = (sp == spawnPoints[selectedSpawnIndex]) ? Color.red : Color.green;
                Gizmos.DrawSphere(sp.position, 2f);
            }
        }
    }
}