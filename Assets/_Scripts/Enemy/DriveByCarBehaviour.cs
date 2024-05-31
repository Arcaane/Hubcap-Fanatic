using UnityEngine;

namespace HubcapCarBehaviour {
    public class DriveByCarBehaviour : BasePoliceCarBehavior {
        [Header("DRIVEBY MOVEMENT")]
        [Tooltip("The dot value in which the car is allowed to shoot (0 = car is perpendicular to the player)")]
        [SerializeField] private Vector2 dotValueToShoot = new(-0.5f, 0.5f);
        [Tooltip("The distance to the player to be considered in range")]
        [SerializeField] protected float attractiveRadius = 11;
        [SerializeField] protected float alignementRadius = 10;
        [Tooltip("The distance to the player to be considerd to close")]
        [SerializeField] protected float repulsiveRadius = 5;
        
        [Header("DRIVEBY SHOOT")]
        [SerializeField, ReadOnly] private bool isShooting = false;
        [SerializeField, ReadOnly] private float shootingTimer = 0f;
        [SerializeField] protected float shootCooldown = 0.6f;
        [SerializeField] protected ParticleSystem shootFx = null;
        
        protected override void UpdateCarMovement() {
            base.UpdateCarMovement();

            if (currentTarget == null) return;
            
            Vector2 direction = new Vector2(currentTarget.position.x, currentTarget.position.z) - new Vector2(transform.position.x, transform.position.z);
            float sqrDist = direction.sqrMagnitude;
            float speedvalue = maxRoadSpeed;

            int nb = 2;
            float rot = (GetRotationValueToObject(currentTarget, attractiveRadius, alignementRadius, repulsiveRadius, true) +
                         GetRotationValueToObject(currentTarget, attractiveRadius, alignementRadius, 0, true));
            
            //Is the car in range to the player
            if (sqrDist < attractiveRadius * attractiveRadius) {
                float dot = Vector2.Dot(new Vector2(transform.forward.x, transform.forward.z).normalized, direction.normalized);
                speedvalue = Mathf.Lerp(0, maxRoadSpeed, (dot + 1) / 2);

                switch (isShooting) {
                    //Player is in range to be shot : start shoot
                    case false when IsDotInRange(dot):
                        shootFx.transform.rotation = Quaternion.LookRotation(direction);
                        EnableShoot();
                        break;
                    
                    //The car is currently shooting
                    case true when IsDotInRange(dot):
                        shootingTimer += Time.deltaTime;
                        shootFx.transform.rotation = Quaternion.Lerp(shootFx.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5);
                        
                        //If timer is above the cooldown value, deal damage to the player
                        if (shootingTimer >= shootCooldown) {
                            PlayerCarController.Instance.playerHealthManager.TakeDamage(currentDamages);
                            shootingTimer = 0;
                        }
                        break;
                    
                    case true when !IsDotInRange(dot):
                        DisableShoot();
                        break;
                }
            }
            //If the player is not in range of the car but the car is still trying to shoot
            else if (isShooting) DisableShoot();

            directionXInput = rot / nb;
            targetSpeed = Mathf.Lerp(targetSpeed, speedvalue, Time.deltaTime * 5);

            OnMovementUpdate();
            return;
            
            
            /*for (int i = 0; i < policeCars.Count; i++) {
                if (policeCars[i] == this || !policeCars[i].gameObject.activeSelf) continue;
                result = GetRotationValueToObject(policeCars[i].transform, attractiveRadius, alignementRadius,
                    repulsiveRadius);
                if (result > -10) {
                    rot += result;
                    nb++;
                }
            }*/
        }
        
        #region SHOOT METHODS

        /// <summary>
        /// Enable the car to shoot to its target
        /// </summary>
        private void EnableShoot() {
            isShooting = true;
            shootFx.Play();
        }
        
        /// <summary>
        /// Disable the car to shoot to its target
        /// </summary>
        private void DisableShoot() {
            isShooting = false;
            shootingTimer = 0;
            shootFx.Stop();
        }

        /// <summary>
        /// Check if the dot value is in range of the variable
        /// </summary>
        /// <param name="dot"></param>
        /// <returns></returns>
        private bool IsDotInRange(float dot) => dot >= dotValueToShoot.x && dot <= dotValueToShoot.y;

        #endregion SHOOT METHODS
        
        #region HELPER

        protected float GetRotationValueToObject(Transform obj, float attrRad, float alignRad, float repulRad, bool alwaysAttract = false, bool applyOffset = false) {
            Vector3 direction = obj.position +
                                (applyOffset ? obj.right * currentRandomOffset.x + obj.forward * currentRandomOffset.y : Vector3.zero) -
                                transform.position;
            float sqrDist = direction.sqrMagnitude;
            Vector2 targetDir;

            if (sqrDist > attrRad * attrRad) // En dehors des radius
            {
                //Debug.DrawLine(transform.position,obj.position,Color.magenta);
                if (alwaysAttract) {
                    targetDir = new Vector2(direction.x, direction.z).normalized;
                }
                else {
                    return -100;
                }
            }
            else if (sqrDist > alignRad * alignRad) // Radius attractif
            {
                targetDir = new Vector2(direction.x, direction.z).normalized;
                //Debug.DrawLine(transform.position,obj.position,Color.green);
            }
            else if (sqrDist > repulRad * repulRad) // Radius alignement
            {
                targetDir = new Vector2(obj.forward.x, obj.forward.z).normalized;
                //Debug.DrawLine(transform.position,obj.position,Color.yellow);
            }
            else // Radius repulsif
            {
                targetDir = new Vector2(-direction.x, -direction.z).normalized;
                //Debug.DrawLine(transform.position,obj.position,Color.red);
            }

            return GetRotationValueToAlign(targetDir);
        }

        private float GetRotationValueToAlign(Vector2 targetDir) {
            float angleToTarget = Vector2.SignedAngle(new Vector2(transform.forward.x, transform.forward.z), targetDir);
            return -Mathf.Clamp(angleToTarget / 10, -1, 1);
        }

        #endregion HELPER
    }
}