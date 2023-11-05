using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveSystem : MonoBehaviour
{
    [Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemyGroups> enemyGroupsList;
        public float spawningInterval;
        [HideInInspector] public int waveQuota;
        [HideInInspector] public int enemySpawnedCount;
    }

    [Serializable]
    public class EnemyGroups
    {
        public GameObject enemyPrefab;
        public int enemyCount;
        [HideInInspector] public int spawnCount;
    }

    public List<Wave> waves;
    public int currentWaveCount;
    private Transform playerTransform;
    [SerializeField] private Transform[] spawnersTransform;

    [Header("Spawner Attributes")] 
    private float spawingTimer;
    public float waveInterval = 15;
    
    // Start is called before the first frame update
    void Start()
    {
        CalculateQuota();
        playerTransform = FindObjectOfType<DashSystem>().transform;
        SpawnEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        spawingTimer += Time.deltaTime;
        
        Debug.Log(waves[currentWaveCount].enemySpawnedCount);
        Debug.Log(waves[currentWaveCount].waveQuota);
        
        if (waves[currentWaveCount].enemySpawnedCount == waves[currentWaveCount].waveQuota)
        {
            if (spawingTimer > waveInterval)
            {
                currentWaveCount++;
                CalculateQuota();
            }
            return;
        }
        
        if (spawingTimer > waves[currentWaveCount].spawningInterval)
        {
            spawingTimer = 0;
            SpawnEnemies();
        }
    }

    private void CalculateQuota()
    {
        var currentQuota = 0;
        foreach (var t in waves[currentWaveCount].enemyGroupsList)
        {
            currentQuota += t.enemyCount;
        }

        waves[currentWaveCount].waveQuota = currentQuota;
        Debug.Log(currentQuota);
    }

    private void SpawnEnemies()
    {
        if (waves[currentWaveCount].enemySpawnedCount < waves[currentWaveCount].waveQuota)
        {
            foreach (var t in waves[currentWaveCount].enemyGroupsList.Where(t => t.spawnCount < t.enemyCount))
            {
                var spawnPos = spawnersTransform[Random.Range(0, spawnersTransform.Length)].position;
                // TODO - Faire du pooling
                var go =  Instantiate(t.enemyPrefab, spawnPos, Quaternion.identity);
                go.GetComponent<EnemyCannonFodder>().playerPos = playerTransform;
                t.spawnCount++;
                waves[currentWaveCount].enemySpawnedCount++;
            }
        }
    }
}
