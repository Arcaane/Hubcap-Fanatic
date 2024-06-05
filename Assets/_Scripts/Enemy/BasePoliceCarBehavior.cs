using System.Collections.Generic;
using HubcapManager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HubcapCarBehaviour {
    [RequireComponent(typeof(EnemyHealthManager), typeof(EnemyCarUpgrade))]
    public class BasePoliceCarBehavior : CarBehaviour, IPoolObject {
        private EnemyHealthManager health = null;
        private EnemyCarUpgrade upgrades = null;

        [Header("POLICE CARS DATA")] 
        [SerializeField, ReadOnly] private bool isActive = true;
        [SerializeField, ReadOnly] protected Transform currentTarget = null;
        [SerializeField] private Vector2 maxRandomOffset = new();
        [SerializeField, ReadOnly] protected Vector2 currentRandomOffset = new();

        [Header("CAR DAMAGE TO PLAYER")]
        [SerializeField] private int carDamage = 1;
        [SerializeField] private bool damagePlayerOnBounce = false;
        [SerializeField, ReadOnly] protected int currentDamages = 0;
        [Space]
        [Tooltip("The strength to apply to the player when bouncing with him")]
        [SerializeField] private float playerBounceStrenth = 10;


        /*[Header("CONVOY")] public ConvoyBehaviour convoyBehaviour;
        public bool attackMode;
        public Transform defensePoint;*/
        
        //public GameObjectDelgate OnPoliceCarDie = delegate { };


        #region INIT METHODS
        
        /// <summary>
        /// Method called when the Pooler retrieve the object and allow to init data for this object
        /// </summary>
        public virtual void InitPoolObject() {
            currentTarget = PlayerCarController.Instance.transform;
            if(health == null) health = GetComponent<EnemyHealthManager>();
            if(upgrades == null) upgrades = GetComponent<EnemyCarUpgrade>();

            PoliceCarManager.Instance.onPlayerDie += SwapTargetOnPlayerDeath;
            
            currentRandomOffset = new Vector2(Random.Range(-maxRandomOffset.x, maxRandomOffset.x), Random.Range(-maxRandomOffset.y, maxRandomOffset.y));
            accelerationInput = 1;
            
            health.InitLife(Mathf.FloorToInt( upgrades.HpToAddPerWave.Evaluate(WaveManager.Instance.currentWaveCount)));
            currentDamages = carDamage + Mathf.FloorToInt(upgrades.DamageToAddPerWave.Evaluate(WaveManager.Instance.currentWaveCount));
            maxRoadSpeed += Mathf.FloorToInt(upgrades.SpeedToAddPerWave.Evaluate(WaveManager.Instance.currentWaveCount));
        }

        /// <summary>
        /// Method called when the Pooler deactivate this object and allow to reset the object to its base stated
        /// </summary>
        public virtual void ResetPoolObject() {
            PoliceCarManager.Instance.onPlayerDie -= SwapTargetOnPlayerDeath;
            currentTarget = null;
            accelerationInput = 0;
            DisableCarScorch();
        }

        public void EnablePoliceCar() => isActive = true;
        public void DisablePoliceCar() => isActive = true;
        
        #endregion INIT METHODS
        
        #region HELPER
        
        /// <summary>
        /// Get the amount of XP to give to the player
        /// </summary>
        /// <returns></returns>
        public int GetXPForPlayer() => Mathf.RoundToInt(upgrades.ExpToGiveBasedOnLevel.Evaluate(PlayerCarController.Instance.playerExperienceManager.CurrentLevel));
        
        /// <summary>
        /// Swap the target of the car when the player die
        /// </summary>
        /// <param name="targets"></param>
        private void SwapTargetOnPlayerDeath(List<Transform> targets) => currentTarget = targets[Random.Range(0, targets.Count)];
        
        /// <summary>
        /// Method called in the UpdateTick method to update the movement of the car
        /// </summary>
        protected virtual void UpdateCarMovement(){}
        
        #endregion HELPER

        public override void UpdateTick() {
            if (!isActive) return;
            base.UpdateTick();
            
            UpdateCarMovement();
            
            /*if (convoyBehaviour) ConvoyUpdate();
            else if (driveByCar) DriveByUpdate();
            else SoloUpdate();*/
        }
        
        public override void FixedUpdateTick() {
            if (!isActive) return;
            base.FixedUpdateTick();
        }
        
        /*private void ConvoyUpdate() {
            if (!attackMode) BoidUpdate();
            else if (driveByCar) DriveByUpdate();
            else SoloUpdate();
        }
        private void BoidUpdate() {
            float rot = 0;
            float result;
            int nb = 2;

            rot += GetRotationValueToObject(convoyBehaviour.transform, convoyBehaviour.attractiveRadius,
                convoyBehaviour.alignementRadius, convoyBehaviour.repulsiveRadius, true, true);
            rot += GetRotationValueToObject(defensePoint, convoyBehaviour.defenseAttractiveRadius,
                convoyBehaviour.defenseAlignementRadius, 0, true, true) * 2;

            Vector3 direction = defensePoint.position - transform.position;
            float sqrDist = direction.sqrMagnitude;
            if (sqrDist < attractiveRadius * 2 * attractiveRadius * 2) {
                float dot = Vector2.Dot(
                    new Vector2(convoyBehaviour.transform.forward.x, convoyBehaviour.transform.forward.z),
                    new Vector2(transform.position.x, transform.position.z) -
                    new Vector2(defensePoint.position.x, defensePoint.position.z));

                float value = (Mathf.Clamp((dot * -1) / 2.5f, -1, 1) + 1) / 2;
                float speedvalue = Mathf.Lerp(0, maxRoadSpeed, value);
                targetSpeed = Mathf.Lerp(targetSpeed, speedvalue, Time.deltaTime * 5);
            }
            else {
                targetSpeed = Mathf.Lerp(targetSpeed, maxRoadSpeed, Time.deltaTime * 5);
            }


            for (int i = 0; i < convoyBehaviour.defenseCars.Length; i++) {
                if (convoyBehaviour.defenseCars[i] == this) continue;
                result = GetRotationValueToObject(convoyBehaviour.defenseCars[i].transform, attractiveRadius,
                    alignementRadius, repulsiveRadius);
                if (result > -10) {
                    rot += result;
                    nb++;
                }
            }

            directionXInput = rot / nb;

            OnMovementUpdate();
        }*/

        /// <summary>
        /// When car enter in collision with an element of the world
        /// </summary>
        /// <param name="other"></param>
        protected override void OnCollisionEnter(Collision other) {
            if (!other.gameObject.CompareTags(new[] {"Wall", "Enemy", "Player"})) return;

            if (other.gameObject.CompareTag("Player")) {
                base.OnCollisionEnter(other);
                if (damagePlayerOnBounce && Vector3.Dot((other.transform.position - transform.position).normalized, transform.forward.normalized) >= minDotAngleToDealDamage) {
                    OnPlayerEnterColision();
                }
            }
            
            MakeCarBounce(other.contacts, other.relativeVelocity.magnitude);
        }

        /// <summary>
        /// Method called when the car enter in collision with the player and need to apply damages
        /// </summary>
        private void OnPlayerEnterColision() {
            PlayerCarController.Instance.playerHealthManager.TakeDamage(currentDamages);
            PlayerCarController.Instance.Rigidbody.AddForceAtPosition(transform.forward * playerBounceStrenth, transform.position);
        }

        #region SCORCH

        public override void EnableCarScorch() {
            base.EnableCarScorch();
            fxFire.gameObject.SetActive(true);
            fxFire.Play(true);
        }

        public override void DisableCarScorch() {
            base.DisableCarScorch();
            fxFire.gameObject.SetActive(false);
            fxFire.Play(false);
        }
        
        #endregion SCORCH
    }
}