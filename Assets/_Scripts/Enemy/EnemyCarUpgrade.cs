using UnityEngine;

namespace HubcapCarBehaviour {
    public class EnemyCarUpgrade : MonoBehaviour {
        [Header("CAR UPGRADES")] 
        [SerializeField] private AnimationCurve hpToAddPerWave = new();
        [SerializeField] private AnimationCurve damageToAddPerWave = new();
        [SerializeField] private AnimationCurve speedToAddPerWave = new();
        [SerializeField] private AnimationCurve expToGiveBasedOnLevel = new();
        public AnimationCurve HpToAddPerWave => hpToAddPerWave;
        public AnimationCurve DamageToAddPerWave => damageToAddPerWave;
        public AnimationCurve SpeedToAddPerWave => speedToAddPerWave;
        public AnimationCurve ExpToGiveBasedOnLevel => expToGiveBasedOnLevel;
    }
}