using System;
using System.Collections.Generic;
using ManagerNameSpace;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class WaveManager : MonoBehaviour
{
    public bool dontSpawn = false;
    
    [Serializable]
    public class Wave
    {
        public string waveName;
        public float waveDuration;
        public List<EnemyGroups> enemyGroupsList;
        [HideInInspector] public int enemySpawnedCount = 0;
    }

    [Serializable]
    public class EnemyGroups
    {
        public Key entityKey;
        [FormerlySerializedAs("enemyCount")] public int enemyCountInWave;
        [HideInInspector] public float enemySpawnRate = 0;
    }
    
    public List<Wave> waves;
    public int currentWaveCount;
    public float intervalBetweenWaves;
    
    //Spawning
    [SerializeField] private Transform carTransform;
    private Vector3[] positions;
    
    [Space]
    [Header("CameraBoundsAttributes")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float heightOffset = 5f;
    [SerializeField] private float widthOffset = 5f;
    [SerializeField] private float addAngleToRectange = 0f;
    [SerializeField] private int spawningPointPerSideCount = 7;

    public float spawingTimer;
    public int counter;

    public float[] enemiesSpawningTimer = new float[0];

    
    // Start is called before the first frame update
    void Start()
    {
        CalculateWaveData();
    }

    private void CalculateWaveData()
    {
        if (currentWaveCount >= waves.Count) return;
        
        enemiesSpawningTimer = new float[waves[currentWaveCount].enemyGroupsList.Count];
        
        for (int i = 0; i < waves[currentWaveCount].enemyGroupsList.Count; i++)
        {
            waves[currentWaveCount].enemyGroupsList[i].enemySpawnRate =  waves[currentWaveCount].waveDuration / waves[currentWaveCount].enemyGroupsList[i].enemyCountInWave;
            enemiesSpawningTimer[i] = waves[currentWaveCount].enemyGroupsList[i].enemySpawnRate;
        }
    }

    void Update()
    {
        if (dontSpawn) return;
        
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SceneManager.LoadScene(0);
        }
        
        if(currentWaveCount == waves.Count) return;
        spawingTimer += Time.deltaTime;

        for (int i = 0; i < waves[currentWaveCount].enemyGroupsList.Count; i++)
        {
            enemiesSpawningTimer[i] += Time.deltaTime;
        }
        
        if (spawingTimer < waves[currentWaveCount].waveDuration)
        {
            SpawnEnemiesV2();
        }

        if (spawingTimer > waves[currentWaveCount].waveDuration + intervalBetweenWaves)
        {
            currentWaveCount++;
            spawingTimer = 0f;
            CalculateWaveData();
        }
    }

    private void SpawnEnemiesV2()
    {
        for (int i = 0; i < waves[currentWaveCount].enemyGroupsList.Count; i++)
        {
            if (enemiesSpawningTimer[i] > waves[currentWaveCount].enemyGroupsList[i].enemySpawnRate)
            {
                SpawnEntity(waves[currentWaveCount].enemyGroupsList[i].entityKey);
                enemiesSpawningTimer[i] = 0;
                counter++;
            }
        }
    }

    private void SpawnEntity(Key entityKey)
    {
        UpdateEnemySpawingPos();
        var rand = UnityEngine.Random.Range(0, positions.Length);
        Vector3 spawnPos = positions[rand];
        spawnPos.y = 0.75f;
        Pooler.instance.SpawnInstance(entityKey, spawnPos, Quaternion.identity);
    }
    
    
    void UpdateEnemySpawingPos()
    {
        positions = new Vector3[spawningPointPerSideCount * 4 + 4 - 1];
            var cam = Camera.main;

            RaycastHit hitInfo;
            Vector3 camPosReal = cam.transform.position + new Vector3(0, -1, 0);
            Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, 1000f, groundMask);
            
            Vector3 bottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, hitInfo.distance)) + offset; // 0;0
            Vector3 topLeft = cam.ScreenToWorldPoint(new Vector3(0, cam.pixelHeight, hitInfo.distance)) + offset ; // 0;1
            Vector3 topRight = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, hitInfo.distance)) + offset; // 1;1
            Vector3 bottomRight = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, 0, hitInfo.distance)) + offset; // 1;0

            bottomLeft.y = topLeft.y = topRight.y = bottomRight.y = 0.75f;

            bottomLeft += new Vector3(-widthOffset, 0, -heightOffset);
            topLeft += new Vector3(-widthOffset, 0, heightOffset);
            topRight += new Vector3(widthOffset, 0, heightOffset);
            bottomRight += new Vector3(widthOffset, 0, -heightOffset);
            
            Quaternion rotationQuaternion = Quaternion.Euler(0, addAngleToRectange, 0);
            
            // Créer les points sur le côté supérieur
            for (int i = 0; i < spawningPointPerSideCount; i++)
            {
                float t = i / (float)(spawningPointPerSideCount - 1);
                Vector3 tempVec = Vector3.Lerp(topLeft, topRight, t);
                positions[i] = rotationQuaternion * tempVec;
            }

            // Créer les points sur le côté droit
            for (int i = 0; i < spawningPointPerSideCount; i++)
            {
                float t = i / (float)(spawningPointPerSideCount / 2f - 1);
                Vector3 tempVec = Vector3.Lerp(topRight, bottomRight, t);
                positions[i + spawningPointPerSideCount] = rotationQuaternion *tempVec;
            }
            
            // Créer les points sur le côté inférieur
            for (int i = 0; i < spawningPointPerSideCount; i++)
            {
                float t = i / (float)(spawningPointPerSideCount - 1);
                Vector3 tempVec = Vector3.Lerp(bottomRight, bottomLeft, t);
                positions[i + spawningPointPerSideCount * 2] = rotationQuaternion *tempVec;
            }

            // Créer les points sur le côté gauche
            for (int i = 0; i < spawningPointPerSideCount; i++)
            {
                float t = i / (float)(spawningPointPerSideCount / 2f - 1);
                Vector3 tempVec = Vector3.Lerp(bottomLeft, topLeft, t);
                positions[i + spawningPointPerSideCount * 3] = rotationQuaternion *tempVec;
            }
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UpdateEnemySpawingPos();
        
        for (int i = 0; i < positions.Length; i++)
        {
            Gizmos.color =  Color.green;
            Gizmos.DrawSphere(positions[i] /*- car.rb.velocity.normalized * car.dirCam * 0.5f*/, 1);
        }
    }
#endif
}
