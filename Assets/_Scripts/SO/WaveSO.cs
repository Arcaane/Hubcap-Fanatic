using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HubcapEnemySpawn {
    [CreateAssetMenu(menuName = "Wave Data/Create new Wave file")]
    public class WaveSO : ScriptableObject {
        [SerializeField] private List<SpawnerData> waveData = new();
        public List<SpawnerData> WaveData => waveData;

        public void ReorderSpawnsBasedOnTimer() => waveData = waveData.OrderBy(obj => obj.spawnTimer).ToList();

        public void UpdateEnemyTypeBasedOnKey(int index) {
            switch (waveData[index].enemyToSpawn.Split("_")[0].ToUpper()) {
                case "ENEMYCAR":
                    waveData[index].spawnType = SpawnType.Enemy;
                    break;

                case "CONVOY":
                    waveData[index].spawnType = SpawnType.Convoy;
                    break;

                case "BLOCKADE":
                    waveData[index].spawnType = SpawnType.Blockade;
                    break;

                case "DELIVERY":
                    waveData[index].spawnType = SpawnType.Delivery;
                    break;

                default: break;
            }
        }

        public SpawnType GetObjectTypeAtIndex(int index) => waveData[index].spawnType;

#if UNITY_EDITOR
        private void OnValidate() {
            for (int index = 0; index < waveData.Count; index++) {
                UpdateEnemyTypeBasedOnKey(index);
            }
        }
    }
#endif  
    
    [System.Serializable]
    public class SpawnerData {
        [Tooltip("The time during the wave where the enemy should spawn (Be aware to not try to spawn enemies above the wave duration)")]
        public float spawnTimer = 0;

        [Tooltip("The key to spawn the enemy)")]
        [Pooler] public string enemyToSpawn = "";

        [Tooltip("This element allow to specify the spawn type and the way to choose the spawn position")]
        public SpawnType spawnType = SpawnType.Enemy;

        [Tooltip("The number of enemy to spawn)")]
        public int enemySpawnAmount = 0;

        [Tooltip("Set if all enemies from this round should spawn at the same position or not")]
        public bool spawnEnemiesAtSamePos = true;
        
        [Tooltip("Allow to set the duration to spawn all enemies (if value equals 0, then all enemies will spawn instantly)")]
        public float timeToSpawnEnemies = 0;
    }

    public enum SpawnType {
        Enemy,
        Delivery,
        Convoy,
        Blockade
    }
}