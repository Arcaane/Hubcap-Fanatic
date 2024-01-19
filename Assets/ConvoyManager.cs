using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ConvoyManager : MonoBehaviour
{
    public ConvoyBehaviour currentConvoy;
    public static ConvoyManager instance;
    public bool waitingForConvoy;
    public float timeBetweenConvoys;
    private float timer;
    public ConvoySpawnData[] convoys;
    public float mapSize = 730;
    public ConvoySpawnPoint[] spawnPoints;

    private void Awake()
    {
        instance = this;
        timer = timeBetweenConvoys;
    }

    private void Update()
    {
        if (waitingForConvoy)
        {
            if (timer > 0) timer -= Time.deltaTime;
            else
            {
                timer = timeBetweenConvoys;
                SpawnConvoy();
            }
        }
    }

    private void SpawnConvoy()
    {
        waitingForConvoy = false;
        List<GameObject> prefabs = new List<GameObject>();
        for (int i = 0; i < convoys.Length; i++)
        {
            if (Time.time > convoys[i].minTime && Time.time < convoys[i].maxTime)
            {
                prefabs.Add(convoys[i].convoyPrefab);
            }
        }
        
        GameObject obj = Instantiate(prefabs[Random.Range(0, prefabs.Count)]);
        currentConvoy = obj.transform.GetComponent<ConvoyBehaviour>();

        int rng = Random.Range(0, spawnPoints.Length);

        Vector3 start;

        start = spawnPoints[rng].spawnPoint.position;
        currentConvoy.agent.Warp(start);

        currentConvoy.target = spawnPoints[rng].possibleEndPoints[Random.Range(0, spawnPoints[rng].possibleEndPoints.Length)].position;
        
        currentConvoy.Initialize();
        if (UIIndic.instance.Obj.Count > 3)
        {
            UIIndic.instance.Obj[3] = obj;
        }
    }
}

[Serializable]
public struct ConvoySpawnData
{
    public GameObject convoyPrefab;
    public float minTime,maxTime;
}

[Serializable]
public struct ConvoySpawnPoint
{
    public Transform spawnPoint;
    public Transform[] possibleEndPoints;
}