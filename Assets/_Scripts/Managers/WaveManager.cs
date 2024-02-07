using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ManagerNameSpace;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    public bool dontSpawn = false;

    [Serializable]
    public class Wave
    {
        public string waveName;
        public float waveDuration;
        public List<EnemyGroups> enemyGroupsList;
        public bool spawnConvoy;
        public int blockadeSpawn,maxBlockades;

        [HideInInspector] public float enemyGroupsSpawnRate = 0;
    }

    [Serializable]
    public class EnemyGroups
    {
        public EnemySpawnParam[] enemyInSpawnBurst;
    }

    [Serializable]
    public class EnemySpawnParam
    {
        public Key entityKey;
        public Vector2Int enemyCount;
    }

    public List<Wave> waves;
    public int currentWaveCount;
    public float intervalBetweenWaves;

    public int activeBlockades;

    //Spawning
    [SerializeField] private Transform carTransform;
    public List<Vector3> positions = new();

    [Space] [Header("CameraBoundsAttributes")] [SerializeField]
    private LayerMask groundMask;

    [SerializeField] private Vector3 offset;
    [SerializeField] private float heightOffset = 5f;
    [SerializeField] private float widthOffset = 5f;
    [SerializeField] private float addAngleToRectange = 0f;
    [SerializeField] private int spawningPointPerSideCount = 7;
    private UIManager uiManager;

    public static WaveManager instance;
    
    public float spawingTimer;
    public float waveSpawnTimer;
    public float enemiesSpawningTimer = 0f;
    public float spawnBurstInsideWave;
    public int spawnBurstCounter;
    [HideInInspector] public bool pauseWave = false;
        
    // Start is called before the first frame update

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        uiManager = UIManager.instance;
        CalculateWaveData();
        
        // CommandConsole SWITCHWAVE = new CommandConsole("SwitchToWave", "SwitchToWave <int>", new List<CommandClass>() {new(typeof(int))}, (value) => switchWave(int.Parse(value[0])));
        // CommandConsole WAVESTATE = new CommandConsole("WaveState", "WaveState <bool>", new List<CommandClass>() {new(typeof(bool))}, (value) => pauseWave = value[0] == "true");
        // CommandConsoleRuntime.Instance.AddCommand(SWITCHWAVE);
        // CommandConsoleRuntime.Instance.AddCommand(WAVESTATE);
    }

    private void CalculateWaveData()
    {
        if (currentWaveCount >= 15)
        {
            MemoryForVictoryScreen.instance.waveCount = currentWaveCount;
            MemoryForVictoryScreen.instance.victory = true;
            DontDestroyOnLoad(MemoryForVictoryScreen.instance.gameObject);
            CarHealthManager.instance.Victory();
            return;
        }
        
        uiManager.UpdateWaveCount(currentWaveCount + 1);
        if (currentWaveCount >= waves.Count) return;

        spawnBurstInsideWave = waves[currentWaveCount].enemyGroupsList.Count;
        //enemiesSpawningTimer = spawnBurstInsideWave / waves[currentWaveCount].waveDuration; // Spawn en fonction de la durée de la wave & le nombre de groupes d'ennemis
        enemiesSpawningTimer = waves[currentWaveCount].waveDuration / spawnBurstInsideWave;
        spawnBurstCounter = 0;
        waveSpawnTimer = 100;
        if(waves[currentWaveCount].spawnConvoy) ConvoyManager.instance.SpawnConvoy();

        int blockadeAmount = Mathf.Clamp(waves[currentWaveCount].blockadeSpawn, 0,
            (waves[currentWaveCount].maxBlockades - activeBlockades));
        for (int i = 0; i < blockadeAmount; i++)
        {
            
        }
    }

    void Update()
    {
        if (dontSpawn || pauseWave) return;
        if (currentWaveCount == waves.Count) return;

        spawingTimer += Time.deltaTime;
        waveSpawnTimer += Time.deltaTime;
        uiManager.UpdateWaveDuration(spawingTimer / (waves[currentWaveCount].waveDuration + intervalBetweenWaves));

        if (spawnBurstCounter < spawnBurstInsideWave)
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

    /// <summary>
    /// Switch to a specific wave
    /// </summary>
    /// <param name="wave"></param>
    public void switchWave(int wave) {
        currentWaveCount = wave;
        spawingTimer = 0f;
        CalculateWaveData();
    }

    private void SpawnEnemiesV2()
    {
        if (waveSpawnTimer < enemiesSpawningTimer) return;
        
        UpdateEnemySpawingPos();
        var randPos = Random.Range(0, positions.Count);
        Vector3 spawnPos = positions[randPos];
        spawnPos.y = 0.75f;
        
        foreach (var t in waves[currentWaveCount].enemyGroupsList[spawnBurstCounter].enemyInSpawnBurst)
        {
            var rand = Random.Range(t.enemyCount.x, t.enemyCount.y + 1);
            SpawnEntity(t.entityKey, rand, spawnPos);
        }
        
        waveSpawnTimer = 0;
        spawnBurstCounter++;
    }

    private void SpawnEntity(Key entityKey, int i, Vector3 spawnPos)
    {
        for (int j = 0; j < i; j++)
        {
            var randPosInsideCircle = Random.insideUnitCircle * 7;
            var unitSpawnPos = spawnPos + new Vector3(randPosInsideCircle.x, 0, randPosInsideCircle.y);

            Vector3 relativePos = carTransform.position - unitSpawnPos;
            Pooler.instance.SpawnInstance(entityKey, unitSpawnPos, Quaternion.LookRotation(relativePos));
        }
    }

    public Transform camholder;

    void UpdateEnemySpawingPos()
    {
        positions.Clear();
        positions = new List<Vector3>();
        
        var cam = Camera.main;

        RaycastHit hitInfo;
        Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, 1000f, groundMask);

        Vector3 bottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, hitInfo.distance)) + offset; // 0;0
        Vector3 topLeft = cam.ScreenToWorldPoint(new Vector3(0, cam.pixelHeight, hitInfo.distance)) + offset; // 0;1
        Vector3 topRight = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, hitInfo.distance)) + offset; // 1;1
        Vector3 bottomRight = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, 0, hitInfo.distance)) + offset; // 1;0

        bottomLeft.y = topLeft.y = topRight.y = bottomRight.y = 0.75f;

        bottomLeft += new Vector3(-widthOffset, 0, -heightOffset);
        topLeft += new Vector3(-widthOffset, 0, heightOffset);
        topRight += new Vector3(widthOffset, 0, heightOffset);
        bottomRight += new Vector3(widthOffset, 0, -heightOffset);
        
        // Créer les points sur le côté supérieur
        for (int i = 0; i < spawningPointPerSideCount; i++)
        {
            float t = i / (float)(spawningPointPerSideCount - 1);
            Vector3 tempVec = Vector3.Lerp(topLeft, topRight, t);

            if (CheckSpawn(tempVec)) positions.Add(tempVec);
        }

        // Créer les points sur le côté droit
        for (int i = 0; i < spawningPointPerSideCount; i++)
        {
            float t = i / (spawningPointPerSideCount / 2f - 1);
            Vector3 tempVec = Vector3.Lerp(topRight, bottomRight, t);
            
            if (CheckSpawn(tempVec)) positions.Add(tempVec);
            
        }

        // Créer les points sur le côté inférieur
        for (int i = 0; i < spawningPointPerSideCount; i++)
        {
            float t = i / (float)(spawningPointPerSideCount - 1);
            Vector3 tempVec = Vector3.Lerp(bottomRight, bottomLeft, t);

            if (CheckSpawn(tempVec)) positions.Add(tempVec);
        }

        // Créer les points sur le côté gauche
        for (int i = 0; i < spawningPointPerSideCount; i++)
        {
            float t = i / (spawningPointPerSideCount / 2f - 1);
            Vector3 tempVec = Vector3.Lerp(bottomLeft, topLeft, t);
            
            if (CheckSpawn(tempVec)) positions.Add(tempVec);
        }
    }

    [ContextMenu("Change Name")]
    private void ChangeName()
    {
        for (int i = 0; i < waves.Count; i++)
        {
            waves[i].waveName = "Wave " + (i + 1);
        }
    }

    public LayerMask enviroMask;
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        UpdateEnemySpawingPos();

        for (int i = 0; i < positions.Count; i++)
        {
            Gizmos.color = Physics.OverlapSphereNonAlloc(positions[i], 2, new Collider[1], enviroMask) > 0 ? Color.red : Color.green;
            Gizmos.DrawSphere(positions[i] /*+ transform.InverseTransformPoint(positions[i])*/, 2f);
        }
    }
#endif

    private bool CheckSpawn(Vector3 pos) => Physics.OverlapSphereNonAlloc(pos, 2, new Collider[1], enviroMask) <= 0;
    
}