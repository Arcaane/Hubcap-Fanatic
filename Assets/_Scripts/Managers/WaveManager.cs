using System;
using System.Collections;
using System.Collections.Generic;
using Helper;
using HubcapCarBehaviour;
using HubcapEnemySpawn;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HubcapManager {
    public class WaveManager : SingletonUpdatesHandler<WaveManager>, IUpdate {
        public int currentWaveCount;
        
        public int activeBlockades;
        public List<DamBehavior> blockades;
        
        [Header("BASE INFORMATION")]
        [SerializeField] private LayerMask enviroMask = new();
        [SerializeField] private bool dontSpawn = false;
        [SerializeField, ReadOnly] private bool pauseWave = false;
        private Transform carTransform = null;
        private List<Transform> positions = new();
        private InGameUIManager uiManager = null;
        private Camera cam = null;
        
        [Header("WAVE DATA")]
        [SerializeField] private List<WaveSO> waves = new();
        [SerializeField] private int waveDuration = 60;
        [SerializeField, ReadOnly] private int currentWave = 0;
        [SerializeField, ReadOnly] private int currentSpawnWaveID = 0;
        [SerializeField, ReadOnly] private float currentWaveTimer = 0f;

        [Header("ENEMY SPAWN POSITION")]
        [SerializeField] private float sizeRatio = 0.1f;
        [SerializeField] private int spawnPointAmountWidth = 30;
        [SerializeField] private int spawnPointAmountHeight = 30;

        [Header("DELIVERY SPAWN POSITION")]
        [SerializeField] private List<Transform> deliverySpawnPos = new();
        [SerializeField] private Transform deliveryPointsContainer = null;
        [SerializeField] private int numberOfPointsPerSide = 10;
        [SerializeField] private float distanceBetweenPoints = 90f;

        protected override void Start() {
            base.Start();
            carTransform = PlayerCarController.Instance.transform;
            uiManager = InGameUIManager.Instance;
            cam = Camera.main;
            
            currentWave = 0;
            currentSpawnWaveID = 0;

            UpdateSpawnPosition(true);

            //CalculateWaveData();

            // CommandConsole SWITCHWAVE = new CommandConsole("SwitchToWave", "SwitchToWave <int>", new List<CommandClass>() {new(typeof(int))}, (value) => switchWave(int.Parse(value[0])));
            // CommandConsole WAVESTATE = new CommandConsole("WaveState", "WaveState <bool>", new List<CommandClass>() {new(typeof(bool))}, (value) => pauseWave = value[0] == "true");
            // CommandConsoleRuntime.Instance.AddCommand(SWITCHWAVE);
            // CommandConsoleRuntime.Instance.AddCommand(WAVESTATE);
        }

        /// <summary>
        /// Method called in the Update method of the UpdateManager script
        /// </summary>
        public void UpdateTick() {
            if (dontSpawn || pauseWave) return;
            if (currentWave == waves.Count) return;

            UpdateWaveCooldown();
        }

        #region WAVE METHODS
        /// <summary>
        /// Update the current timer of the wave and the slider visual in the UI
        /// </summary>
        private void UpdateWaveCooldown() {
            currentWaveTimer += Time.deltaTime;
            uiManager.UpdateWaveSlider(1 - currentWaveTimer / (waveDuration), currentWave + 1);
            
            if(currentSpawnWaveID < waves[currentWave].WaveData.Count) CheckForEnemyToSpawn(waves[currentWave].WaveData[currentSpawnWaveID]);
            TryStartNextWave();
        }

        /// <summary>
        /// Check if there is any round of enemies to spawn at the current timer
        /// </summary>
        private void CheckForEnemyToSpawn(SpawnerData data) {
            if (currentSpawnWaveID >= waves[currentWave].WaveData.Count) return;
            if (currentWaveTimer < data.spawnTimer) return;

            UpdateSpawnPosition();
            StartCoroutine(SpawnEnemies(data));

            currentSpawnWaveID++;
        }

        /// <summary>
        /// Spawn enemies over time
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private IEnumerator SpawnEnemies(SpawnerData data) {
            Transform randPos = GetPositionBasedOnObjectType(data.spawnType);
            
            for (int i = 0; i < data.enemySpawnAmount; i++) {
                if(data.timeToSpawnEnemies != 0) yield return new WaitForSeconds(data.timeToSpawnEnemies / data.enemySpawnAmount);
                if(!data.spawnEnemiesAtSamePos) randPos = GetPositionBasedOnObjectType(data.spawnType);
                GameObject obj = PoolManager.Instance.RetrieveOrCreateObject(data.enemyToSpawn, randPos.position, Quaternion.identity);
            }
        }

        /// <summary>
        /// Try to go to next wave if timer allow it
        /// </summary>
        private void TryStartNextWave() {
            if (currentWaveTimer < waveDuration) return;
            currentWave++;
            currentWaveTimer = 0;
            currentSpawnWaveID = 0;
        }
        
        /// <summary>
        /// Enable the spawner
        /// </summary>
        public void EnableWavesSpawn() => dontSpawn = false;
        /// <summary>
        /// Disable the spawner
        /// </summary>
        public void DisableWavesSpawn() => dontSpawn = true;
        #endregion WAVE METHODS

        private Transform GetPositionBasedOnObjectType(SpawnType type) {
            switch (type) {
                case SpawnType.Enemy: return GetRandomActivePosition();
                    
                case SpawnType.Delivery: return GetRandomDeliveryPos();
                    
                case SpawnType.Convoy: break;
                
                case SpawnType.Blockade: break;
                
                default: return null;
            }

            return null;
        }

        #region ENEMY SPAWN POS
        
        /// <summary>
        /// Update the spawn position of all points in the list of position
        /// </summary>
        private void UpdateSpawnPosition(bool firstIteration = false) {
            if(firstIteration) positions.Clear();
            
            Vector3 bottomLeft = carTransform.position + (cam.transform.right * (-cam.pixelWidth / 2) + cam.transform.up * (-cam.pixelHeight / 2))  * sizeRatio;
            Vector3 topLeft = carTransform.position+ (cam.transform.right * (-cam.pixelWidth / 2) + cam.transform.up * (cam.pixelHeight / 2))  * sizeRatio;
            Vector3 topRight = carTransform.position + (cam.transform.right * (cam.pixelWidth / 2) + cam.transform.up * (cam.pixelHeight / 2))  * sizeRatio;
            Vector3 bottomRight = carTransform.position + (cam.transform.right * (cam.pixelWidth / 2) + cam.transform.up * (-cam.pixelHeight / 2))  * sizeRatio;

            UpdateSpawnPos(topLeft, topRight, spawnPointAmountWidth, 0, firstIteration, "TopLeft-TopRight");
            UpdateSpawnPos(topRight, bottomRight, spawnPointAmountHeight, spawnPointAmountWidth, firstIteration, "TopRight-BottomRight");
            UpdateSpawnPos(bottomRight, bottomLeft, spawnPointAmountWidth, spawnPointAmountWidth + spawnPointAmountHeight, firstIteration, "BottomRight-BottomLeft");
            UpdateSpawnPos(bottomLeft, topLeft, spawnPointAmountHeight, spawnPointAmountWidth * 2 + spawnPointAmountHeight, firstIteration, "BottomLeft-TopLeft");
        }
        
        /// <summary>
        /// Update the position of a spawn point
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="amount"></param>
        /// <param name="baseValue"></param>
        /// <param name="firstIteration"></param>
        /// <param name="spawnName"></param>
        private void UpdateSpawnPos(Vector3 from, Vector3 to, int amount, int baseValue, bool firstIteration = false, string spawnName = "") {
            for (int iteration = 0; iteration < amount; iteration++) {
                if (firstIteration) CreateSpawnPosObj(spawnName, iteration);
                positions[baseValue + iteration].position = Vector3.Lerp(new Vector3(from.x, 0, from.z), new Vector3(to.x, 0, to.z), iteration / (float) (amount - 1));
                positions[baseValue + iteration].gameObject.SetActive(CheckSpawn(positions[baseValue + iteration].position));
            }
        }

        /// <summary>
        /// Create a new gameObject and add it to the list of positions
        /// </summary>
        /// <param name="spawnName"></param>
        /// <param name="iteration"></param>
        private void CreateSpawnPosObj(string spawnName, int iteration) {
            GameObject gam = new GameObject($"Spawn_{spawnName}_{iteration}");
            gam.transform.parent = transform;
            positions.Add(gam.transform);
        }

        /// <summary>
        /// Check if a spawnPos can actually spawn an enemy by checking the environment
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private bool CheckSpawn(Vector3 pos) => Physics.OverlapSphereNonAlloc(pos, 2, new Collider[1], enviroMask) <= 0;

        /// <summary>
        /// Return a position which is not disabled
        /// </summary>
        /// <returns></returns>
        private Transform GetRandomActivePosition() {
            Transform position = null;
            while (position == null) {
                int random = Random.Range(0, positions.Count);
                if (positions[random].gameObject.activeSelf) position = positions[random];
            }
            return position;
        }
        
        #endregion ENEMY SPAWN POS
        
        #region DELIVERY SPAWN POS

        [ContextMenu("Create Random Delivery SpawnPos")]
        private void CreateDeliverySpawnPos() {
            for (int i = 0; i < numberOfPointsPerSide; i++) {
                for (int j = 0; j < numberOfPointsPerSide; j++) {
                    Vector3 spawnPosition = new Vector3(
                        deliveryPointsContainer.position.x + i * distanceBetweenPoints,
                        deliveryPointsContainer.position.y,
                        deliveryPointsContainer.position.z + j * distanceBetweenPoints
                    );

                    Transform spawnPoint = new GameObject("SpawnPoint").transform;
                    spawnPoint.position = spawnPosition;
                    spawnPoint.parent = deliveryPointsContainer.transform;
                }
            }
            
            UpdateDeliverySpawnList();
        }

        [ContextMenu("Update Delivery Spawn List")]
        private void UpdateDeliverySpawnList() {
            deliverySpawnPos.Clear();
            for (int index = 0; index < deliveryPointsContainer.childCount; index++) {
                deliveryPointsContainer.GetChild(index).name = $"SpawnPoint_{(index < 10 ? $"0{index}" : $"{index}")}";
                deliverySpawnPos.Add(deliveryPointsContainer.GetChild(index));
            }
        }

        /// <summary>
        /// Get a random position based on all available delivery position
        /// </summary>
        /// <returns></returns>
        private Transform GetRandomDeliveryPos() => deliverySpawnPos[Random.Range(0, deliverySpawnPos.Count)];

        #endregion DELIVERY SPAWN POS


        
        
        /*
        private void CalculateWaveData() {
            if (currentWaveCount >= 15) {
                MemoryForVictoryScreen.instance.waveCount = currentWaveCount;
                MemoryForVictoryScreen.instance.victory = true;
                DontDestroyOnLoad(MemoryForVictoryScreen.instance.gameObject);
                CarHealthManager.instance.Victory();
                return;
            }

            //uiManager.UpdateWaveSlider(spawingTimer / (waves[currentWaveCount].waveDuration + intervalBetweenWaves), currentWaveCount + 1);
            if (currentWaveCount >= wavesOld.Count) return;

            spawnBurstInsideWave = wavesOld[currentWaveCount].enemyGroupsList.Count;
            //enemiesSpawningTimer = spawnBurstInsideWave / waves[currentWaveCount].waveDuration; // Spawn en fonction de la durée de la wave & le nombre de groupes d'ennemis
            enemiesSpawningTimer = wavesOld[currentWaveCount].waveDuration / spawnBurstInsideWave;
            spawnBurstCounter = 0;
            currentWaveTimer = 100;
            if (wavesOld[currentWaveCount].spawnConvoy) ConvoyManager.instance.SpawnConvoy();

            if (activeBlockades >= blockades.Count) return;

            int blockadeAmount = Mathf.Clamp(wavesOld[currentWaveCount].blockadeSpawn, 0,
                (wavesOld[currentWaveCount].maxBlockades - activeBlockades));
            for (int i = 0; i < blockadeAmount; i++) {
                if (activeBlockades >= blockades.Count) return;

                int rng = Random.Range(0, blockades.Count);

                for (int j = 0; j < blockades.Count; j++) {
                    if (!blockades[rng].isActive) {
                        blockades[rng].SpawnCars();
                        break;
                    }

                    rng = (rng + 1) % blockades.Count;
                }
            }
        }

        void Updater() {
            if (dontSpawn || pauseWave) return;
            if (currentWaveCount == wavesOld.Count) return;

            spawingTimer += Time.deltaTime;
            currentWaveTimer += Time.deltaTime;
            //uiManager.UpdateWaveSlider(spawingTimer / (waves[currentWaveCount].waveDuration + intervalBetweenWaves), currentWaveCount + 1);

            if (spawnBurstCounter < spawnBurstInsideWave) {
                SpawnEnemiesV2();
            }

            if (spawingTimer > wavesOld[currentWaveCount].waveDuration + intervalBetweenWaves) {
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

        private void SpawnEnemiesV2() {
            if (currentWaveTimer < enemiesSpawningTimer) return;

            UpdateEnemySpawingPos();
            var randPos = Random.Range(0, positions.Count);
            Vector3 spawnPos = positions[randPos];
            spawnPos.y = 0.75f;

            foreach (var t in wavesOld[currentWaveCount].enemyGroupsList[spawnBurstCounter].enemyInSpawnBurst) {
                var rand = Random.Range(t.enemyCount.x, t.enemyCount.y + 1);
                SpawnEntity(t.entityKey, rand, spawnPos);
            }

            currentWaveTimer = 0;
            spawnBurstCounter++;
        }

        //For CommandConsole
        public void SpawnNewEntity(Key entityKey, int i) {
            var randPos = Random.Range(0, positions.Count);
            Vector3 spawnPos = positions[randPos];
            spawnPos.y = 0.75f;
            SpawnEntity(entityKey, i, spawnPos);
        }

        private void SpawnEntity(Key entityKey, int i, Vector3 spawnPos) {
            for (int j = 0; j < i; j++) {
                var randPosInsideCircle = Random.insideUnitCircle * 7;
                var unitSpawnPos = spawnPos + new Vector3(randPosInsideCircle.x, 0, randPosInsideCircle.y);

                Vector3 relativePos = carTransform.position - unitSpawnPos;
                PoolManagerTest.instance.SpawnInstance(entityKey, unitSpawnPos, Quaternion.LookRotation(relativePos));
            }
        }

        public Transform camholder;

        void UpdateEnemySpawingPos() {
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
            for (int i = 0; i < spawningPointPerSideCount; i++) {
                float t = i / (float) (spawningPointPerSideCount - 1);
                Vector3 tempVec = Vector3.Lerp(topLeft, topRight, t);

                if (CheckSpawn(tempVec)) positions.Add(tempVec);
            }

            // Créer les points sur le côté droit
            for (int i = 0; i < spawningPointPerSideCount; i++) {
                float t = i / (spawningPointPerSideCount / 2f - 1);
                Vector3 tempVec = Vector3.Lerp(topRight, bottomRight, t);

                if (CheckSpawn(tempVec)) positions.Add(tempVec);

            }

            // Créer les points sur le côté inférieur
            for (int i = 0; i < spawningPointPerSideCount; i++) {
                float t = i / (float) (spawningPointPerSideCount - 1);
                Vector3 tempVec = Vector3.Lerp(bottomRight, bottomLeft, t);

                if (CheckSpawn(tempVec)) positions.Add(tempVec);
            }

            // Créer les points sur le côté gauche
            for (int i = 0; i < spawningPointPerSideCount; i++) {
                float t = i / (spawningPointPerSideCount / 2f - 1);
                Vector3 tempVec = Vector3.Lerp(bottomLeft, topLeft, t);

                if (CheckSpawn(tempVec)) positions.Add(tempVec);
            }
        }

        [ContextMenu("Change Name")]
        private void ChangeName() {
            for (int i = 0; i < wavesOld.Count; i++) {
                wavesOld[i].waveName = "Wave " + (i + 1);
            }
        }

        public LayerMask enviroMask;

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            if (!Application.isPlaying) return;

            UpdateEnemySpawingPos();

            for (int i = 0; i < positions.Count; i++) {
                Gizmos.color = Physics.OverlapSphereNonAlloc(positions[i], 2, new Collider[1], enviroMask) > 0 ? Color.red : Color.green;
                Gizmos.DrawSphere(positions[i] + transform.InverseTransformPoint(positions[i]), 2f);
            }
        }
#endif
*/
        
    }
}