using System.Collections.Generic;
using System.Linq;
using Abilities;
using HubcapCarBehaviour;
using HubcapManager;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HubcapAbility {
    public class ShotgunHandler : UpdatesHandler, IUpdate {
        [SerializeField, ReadOnly] private List<Transform> enemyCars = new();
        [SerializeField, ReadOnly] private List<float> shootTimes = new();
        [SerializeField, ReadOnly] private Transform currentTarget = null;
        private PlayerCarController player = null;
        private Camera cam = null;

        [Header("SHOTGUN DATA")] 
        [SerializeField] private int numberOfShotAtStart = 2;
        [SerializeField] private float baseShootTime = 2;
        [SerializeField] public float shootCooldown = 4;
        [SerializeField] private ParticleSystem shotgunParticles = null;
        [SerializeField] public int shotgunDamages = 50;
        private int shotBeforeCrit = 0;
        private int currentShotLeftBeforeCrit = 0;
        
        #region INIT DATA

        protected override void Start() {
            base.Start();
            player = PlayerCarController.Instance;
            cam = Camera.main;
            
            InitNumberOfShot();
            InGameUIManager.Instance.DisableShootIcon();
            InputManager.Instance.Inputs.Player.Shoot.started += TryShootShotgun;
        }

        /// <summary>
        /// Remove input methods from the InputManager when the object is disable
        /// </summary>
        protected override void OnDisable() {
            base.OnDisable();
            InputManager.Instance.Inputs.Player.Shoot.started -= TryShootShotgun;
        }

        /// <summary>
        /// Initialize the number of shot at start
        /// </summary>
        private void InitNumberOfShot() {
            shootTimes.Clear();
            for (int i = 0; i < numberOfShotAtStart; i++) {
                shootTimes.Add(baseShootTime);
            }
        }
        
        #endregion INIT DATA
        
        public void UpdateTick() {
            UpdateShotCooldown();
            UpdateShootIconVisual();
        }
        
        #region SHOT
        
        /// <summary>
        /// Method called when the shoot input was pressed and check if the player can shoot
        /// </summary>
        /// <param name="obj"></param>
        private void TryShootShotgun(InputAction.CallbackContext obj) {
            if (!CanShoot()) return;
            
            if (!HasEnemyInRange()) {
                //CarAbilitiesManager.instance.OnShotgunUsedWithoutTarget.Invoke();
                PlayShotgunEffect(Random.Range(0,2) == 0 ? transform.right : -transform.right);
            }
            else {
                ShootShotgun();
                //CarAbilitiesManager.instance.OnShotgunUsed.Invoke();
            }
            
            ResetCooldownOfFirstBullet();
            //CameraShake.instance.SetShake(0.3f);
        }

        /// <summary>
        /// Shoot a bullet of the shotgun to the targeted enemy
        /// </summary>
        private void ShootShotgun() {
            Vector3 direction = currentTarget.position - transform.position;
            player.Rigidbody.AddForce(transform.right * 100 * (Vector3.Dot(direction, transform.right) > 0 ? -1 : 1));

            PlayShotgunEffect(direction);

            if (player.gotVayneUpgrade) currentShotLeftBeforeCrit--;
            int damageToDeal = Mathf.FloorToInt((shotgunDamages * player.mightPowerUpLevel) * (NeedToApplyCritDamage() ? player.vaynePassiveMultiplier : 1));

            if (currentTarget.TryGetComponent(out IDamageable damageable)) {
                bool enemyDie = damageable.TakeDamage(damageToDeal);
                
                CarAbilitiesManager.instance.OnEnemyDamageTaken.Invoke(currentTarget.gameObject);
                CarAbilitiesManager.instance.OnEnemyHitWithShotgun.Invoke(currentTarget.gameObject);
                
                if(enemyDie) RemoveEnemyCar(currentTarget);
            }

            if (player.gotVayneUpgrade && currentShotLeftBeforeCrit == 0) currentShotLeftBeforeCrit = shotBeforeCrit;
        }

        /// <summary>
        /// Play the shotgun Effect in a certain direction
        /// </summary>
        private void PlayShotgunEffect(Vector3 direction) {
            shotgunParticles.transform.rotation = Quaternion.LookRotation(direction);
            shotgunParticles.Play();
        }
        
        /// <summary>
        /// Update the timer of the first bullet available
        /// </summary>
        private void ResetCooldownOfFirstBullet() {
            for (int i = 0; i < shootTimes.Count; i++) {
                if (shootTimes[i] <= shootCooldown) continue;
                shootTimes[i] = 0;
                break;
            }
        }

        /// <summary>
        /// Update the number of shot between critical damage
        /// </summary>
        /// <param name="amount"></param>
        public void UpdateShotBeforeCrit(int amount) {
            shotBeforeCrit = amount;
            currentShotLeftBeforeCrit = shotBeforeCrit;
        }

        /// <summary>
        /// Update cooldown of all shots
        /// </summary>
        private void UpdateShotCooldown() {
            for (int i = 0; i < shootTimes.Count; i++) {
                if (shootTimes[i] >= shootCooldown) continue;
                shootTimes[i] += Time.deltaTime;
                InGameUIManager.Instance.UpdateShotgunSlider(shootTimes[i] / shootCooldown, i);
                
                if (shootTimes[i] >= shootCooldown) TryToTargetEnemy();
            }
        }
        
        #endregion SHOT
        
        #region ENEMY HANDLER
        
        /// <summary>
        /// Try to update the current targeted enemy
        /// </summary>
        /// <param name="forceNewTarget"></param>
        private void TryToTargetEnemy(bool forceNewTarget = false) {
            if (enemyCars.Count == 0 || !CanShoot()) {
                InGameUIManager.Instance.DisableShootIcon();
                currentTarget = null;
                return;
            }

            if (currentTarget != null && !forceNewTarget) return;
            currentTarget = enemyCars[0];
        }
        
        /// <summary>
        /// Add the convoy to the top of the list of enemies inside the shotgun trigger
        /// </summary>
        /// <param name="convoy"></param>
        public void AddConvoy(Transform convoy) {
            enemyCars.Insert(0, convoy);
            TryToTargetEnemy(true);
        }
        
        /// <summary>
        /// Add an enemy to the list of enemies inside the shotgun trigger
        /// </summary>
        /// <param name="enemy"></param>
        public void AddEnemyCar(Transform enemy) {
            enemyCars.Add(enemy);
            TryToTargetEnemy();
        }
        
        /// <summary>
        /// Remove an enemy from the list of enemies inside the shotgun trigger
        /// </summary>
        /// <param name="enemy"></param>
        public void RemoveEnemyCar(Transform enemy) {
            if (!enemyCars.Contains(enemy)) return;
            if (currentTarget == enemy) currentTarget = null;
            enemyCars.Remove(enemy);
            TryToTargetEnemy();
        }
        
        #endregion ENEMY HANDLER
        
        #region SHOOT ICON
        
        /// <summary>
        /// Update the position of the shootIcon
        /// </summary>
        private void UpdateShootIconVisual() {
            if (currentTarget == null) return;
            InGameUIManager.Instance.UpdateShootIcon(cam.WorldToScreenPoint(currentTarget.position), Vector3.one);
        }

        #endregion SHOOT ICON
        
        #region HELPER
        
        /// <summary>
        /// Check if the player can shoot with his shotgun
        /// </summary>
        /// <returns></returns>
        private bool CanShoot() => shootTimes.Any(time => time >= shootCooldown);

        /// <summary>
        /// Check if there 
        /// </summary>
        /// <returns></returns>
        private bool HasEnemyInRange() => currentTarget != null;

        /// <summary>
        /// Check if the player need to apply crit damages
        /// </summary>
        /// <returns></returns>
        private bool NeedToApplyCritDamage() => player.gotVayneUpgrade && currentShotLeftBeforeCrit == 0;
        
        #endregion HELPER
    }
}