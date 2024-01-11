using System;
using ManagerNameSpace;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    private Vector3[] positions;
    [SerializeField] private Transform carTransform;
    [SerializeField] private CarController car;
    
    [Header("Spawning Attributes")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Vector2Int enemyPerWaveBoundsCount;
    [SerializeField] private float maxDistFromSpawner = 4f;
    [SerializeField] private float spawningInterval = 3f;

    public float timer = 10;

    [SerializeField] private SpawningShape SpawningShape = SpawningShape.Rect;

    [Space]
    [Header("CircleSpawnAttributes")]
    [SerializeField] private int spawnPointsCount = 10;
    [SerializeField] private float spawningRadius = 5f;
    
    [Space]
    [Header("RectSpawnAttributes")]
    [SerializeField] private int spawningPointPerSideCount = 7;
    [SerializeField] private float height = 30f;
    [SerializeField] private float width = 20f;
    [SerializeField] private float addAngleToRectange = 0f;
    
    [Space]
    [Header("CameraBoundsAttributes")]
    [SerializeField] private int spawningPointPerSideForCameraBounds = 7;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float heightOffset = 5f;
    [SerializeField] private float widthOffset = 5f;
    
    public bool stopSpawn;

    private void Start()
    {
        car = carTransform.GetComponent<CarController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (stopSpawn) return;
        
        
        timer += Time.deltaTime;
        if (timer > spawningInterval)
        {
            SpawnWave();
        }
    }

    /// <summary>
    /// Method qui est appelée pour faire spawn une vague d'ennemis
    /// </summary>
    void SpawnWave()
    {
        UpdateEnemySpawingPos();
        timer = 0;
        var temp = Random.Range(0, spawnPointsCount);
        var spawnTr = positions[temp] + carTransform.InverseTransformPoint(positions[temp]);
        var nbThisWave = Random.Range(enemyPerWaveBoundsCount.x, enemyPerWaveBoundsCount.y + 1);
            
        for (int i = 0; i < nbThisWave; i++)
        {
            Vector2 spawnPos =  Random.insideUnitCircle * maxDistFromSpawner;
            Vector3 initPos = spawnTr + new Vector3(spawnPos.x, 0f, spawnPos.y);
            initPos.y = 0.75f;
            Pooler.instance.SpawnInstance(Key.OBJ_Foddler, initPos, Quaternion.identity);
        }
    }
    
    /// <summary>
    /// Calculer la position des points de spawn des entités ennemis
    /// </summary>
    void UpdateEnemySpawingPos()
    {
        var currentPos = carTransform.position;
        
        if (SpawningShape == SpawningShape.Circle)
        {
            positions = new Vector3[spawnPointsCount];
            
            for (int i = 0; i < spawnPointsCount; i++)
            {
                float angle = i * (2 * Mathf.PI / spawnPointsCount);
                float x = Mathf.Cos(angle) * spawningRadius;
                float z = Mathf.Sin(angle) * spawningRadius;

                positions[i] = new Vector3(currentPos.x + x, 0.75f, currentPos.z + z);
            }
        }
        else if (SpawningShape == SpawningShape.Rect)
        {
            positions = new Vector3[spawningPointPerSideCount*4];
            float rotationAngleRadians = addAngleToRectange * Mathf.Deg2Rad;
            Quaternion rotationQuaternion = Quaternion.Euler(0, rotationAngleRadians, 0);
            
            float semiHeight = height / 2;
            float semiWidth = width / 2;

            // Créer les points sur le côté supérieur
            for (int i = 0; i < spawningPointPerSideCount; i++)
            {
                float t = i / (float)(spawningPointPerSideCount - 1);
                float x = Mathf.Lerp(-semiHeight, semiHeight, t);
                float y = semiWidth;
                
                positions[i] = rotationQuaternion * new Vector3(currentPos.x + x, 0.75f, currentPos.z + y);
            }

            // Créer les points sur le côté droit
            for (int i = 0; i < spawningPointPerSideCount; i++)
            {
                float t = i / (float)(spawningPointPerSideCount - 1);
                float x = semiHeight;
                float y = Mathf.Lerp(semiWidth, -semiWidth, t);
                
                positions[i + spawningPointPerSideCount] = rotationQuaternion * new Vector3(currentPos.x + x, 0.75f, currentPos.z + y);
            }
            
            // Créer les points sur le côté inférieur
            for (int i = 0; i < spawningPointPerSideCount; i++)
            {
                float t = i / (float)(spawningPointPerSideCount - 1);
                float x = Mathf.Lerp(semiHeight, -semiHeight, t);
                float y = -semiWidth;

                positions[i + spawningPointPerSideCount * 2] = rotationQuaternion * new Vector3(currentPos.x + x, 0.75f, currentPos.z + y);
            }

            // Créer les points sur le côté gauche
            for (int i = 0; i < spawningPointPerSideCount; i++)
            {
                float t = i / (float)(spawningPointPerSideCount - 1);
                float x = -semiHeight;
                float y = Mathf.Lerp(-semiWidth, semiWidth, t);

                positions[i + spawningPointPerSideCount * 3] = rotationQuaternion * new Vector3(currentPos.x + x, 0.75f, currentPos.z + y);
            }
        }
        else if (SpawningShape == SpawningShape.CameraBounds)
        {
            //positions = new Vector3[4];
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
                float t = i / (float)(spawningPointPerSideCount - 1);
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
                float t = i / (float)(spawningPointPerSideCount - 1);
                Vector3 tempVec = Vector3.Lerp(bottomLeft, topLeft, t);
                positions[i + spawningPointPerSideCount * 3] = rotationQuaternion *tempVec;
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        UpdateEnemySpawingPos();
        
        for (int i = 0; i < positions.Length; i++)
        {
            Gizmos.color =  Color.green;
            Gizmos.DrawSphere(positions[i] /*- car.rb.velocity.normalized * car.dirCam * 0.5f*/, 1);
        }
    }
}

enum SpawningShape
{
    Circle,
    Rect,
    CameraBounds
}

