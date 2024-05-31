using UnityEngine;

namespace HubcapCarBehaviour {
    public class BerserkCarBehaviour : BasePoliceCarBehavior {
        [Header("BERSERK MOVEMENT")]
        [SerializeField] private float overShootDistance = 12f;
        [SerializeField] private float randomOvershoot = 2f;
        private float additiveOverShoot = 0;
        
        #region POOLER METHODS

        public override void InitPoolObject() {
            base.InitPoolObject();
            additiveOverShoot = GetRandomOvershoot();
            targetSpeed = maxRoadSpeed;
        }

        public override void ResetPoolObject() {
            base.ResetPoolObject();
            additiveOverShoot = 0;
            targetSpeed = 0;
        }

        #endregion POOOLER METHODS
        
        protected override void UpdateCarMovement() {
            base.UpdateCarMovement();
            
            if (currentTarget == null) return;
            
            Vector3 targetPos = GetTargetPosition(currentTarget);
            float angleToTarget = Vector2.SignedAngle(new Vector2(transform.forward.x, transform.forward.z), new Vector2(targetPos.x, targetPos.z) - new Vector2(transform.position.x, transform.position.z));
            directionXInput = -Mathf.Clamp(angleToTarget / 10, -1, 1);

            /*for (int i = 0; i < policeCars.Count; i++) {
                if (policeCars[i] == this || !policeCars[i].gameObject.activeSelf) continue;
                result = GetRotationValueToObject(policeCars[i].transform, attractiveRadius, alignementRadius,
                    repulsiveRadius);
                if (result > -10) {
                    rot += result;
                    nb++;
                }
            }  */

            OnMovementUpdate();
        }

        #region HELPER METHODS

        /// <summary>
        /// Get the position to aim for based on a target
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private Vector3 GetTargetPosition(Transform target) => target.position + target.forward * (overShootDistance + additiveOverShoot) * PlayerCarController.Instance.globalSpeedFactor;
        
        /// <summary>
        /// Randomize the distance to overshoot the player
        /// </summary>
        private float GetRandomOvershoot() => Random.Range(-randomOvershoot, randomOvershoot);

        #endregion HELPER METHODS
    }
}