using System.Collections.Generic;
using System.Threading.Tasks;
using HubcapManager;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace HubcapCarBehaviour {
    public class PlayerHealthManager : BaseCarHealthManager {
        [Header("EFFECTS DATA")] 
        [SerializeField, Pooler] private string explosionKey = "";
        [SerializeField] private ParticleSystem dieSmokeParticle = null;
        [SerializeField] private int lowLifeEffect = 15;
        private Animator vignetteAnim = null;
        
        [Header("VOLUME DATA")]
        [SerializeField] private Volume volume = null;
        [SerializeField] private VignetteData volumeData = null;
        private Vignette vt = null;

        public bool IsAlive => currentHealth > 1;

        protected override void Start() {
            base.Start();
            
            vignetteAnim = volume.GetComponent<Animator>();
            volume.profile.TryGet(out vt);
            UpdateLifeUI();
            
            CommandConsole TAKEDAMAGE = new CommandConsole(
                "TakeDamage", 
                "TakeDamage : deal damage to the player", 
                new List<CommandClass> { new(typeof(int)) }, 
                (value) => { TakeDamage(int.Parse(value[0])); }, 
                false);
            CommandConsoleRuntime.Instance.AddCommand(TAKEDAMAGE);
            
            CommandConsole HEALPLAYER = new CommandConsole(
                "HealPlayer", 
                "HealPlayer : heal the player", 
                new List<CommandClass> { new(typeof(int)) }, 
                (value) => { HealCar(int.Parse(value[0])); }, 
                false);
            CommandConsoleRuntime.Instance.AddCommand(HEALPLAYER);
        }
        
        public override bool TakeDamage(int damage) {
            if (!IsDamageable()) return false;
            int damageToDeal = Mathf.FloorToInt(damage - (damage * armorPercent));
            currentHealth -= damageToDeal;

            TakeDamageCarFeedback();
            UpdateLifeUI();
            
            //UI method to have feedback when taking damage
            
            return base.TakeDamage(damage);
        }
        protected override async void CarDeath() {
            isDead = true;
            PlayerCarController.Instance.targetSpeed = 0;
            PlayerCarController.Instance.maxRoadSpeed = 0;
            WaveManager.Instance.DisableWavesSpawn();

            dieSmokeParticle.gameObject.SetActive(true);
            dieSmokeParticle.Play();
            
            PoliceCarManager.Instance.CallOnPlayerDeath();
            //MemoryForVictoryScreen.instance.waveCount = WaveManager.Instance.currentWaveCount;
            //MemoryForVictoryScreen.instance.victory = false;
            //SaveGold();
            
            await Task.Delay(2500);
            PoolManager.Instance.RetrieveOrCreateObject(explosionKey, transform.position, Quaternion.identity);

            await Task.Delay(2300);
            GameManager.Instance.LoadNewScene(SceneLoader.victoryScene);
        }
        public override void HealCar(int healAmount) {
            base.HealCar(healAmount);
            UpdateLifeUI();
        }
        
        /// <summary>
        /// Update the interface visuals based on the current health
        /// </summary>
        private void UpdateLifeUI() {
            InGameUIManager.Instance.UpdateLifeSlider((float) currentHealth / maxHealth, currentHealth);
            vt.color.SetValue(new ColorParameter(Color.Lerp(volumeData.endColor, volumeData.basedColor, (float)currentHealth / maxHealth)));
            vt.intensity.SetValue(new ClampedFloatParameter(Mathf.Lerp(volumeData.endIntensity, volumeData.basedIntensity, (float) currentHealth / maxHealth),0,1));
            vignetteAnim.SetBool("IsLowLife", currentHealth <= lowLifeEffect);
        }
    }

    [System.Serializable]
    public class VignetteData {
        public Color basedColor = new();
        public Color endColor = new();
        public float basedIntensity = new();
        public float endIntensity = new();
    }
}