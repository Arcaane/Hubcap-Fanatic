using System.Collections.Generic;
using Abilities;
using HubcapManager;
using Unity.Mathematics;
using UnityEngine;

namespace HubcapCarBehaviour {
    public class EnemyHealthManager : BaseCarHealthManager {
        [SerializeField] private List<MeshRenderer> metalParts = new();

        [Header("FEEDBACKS")] 
        [SerializeField, Pooler] private string damageTxtKey = "";
        
        /// <summary>
        /// Init the life of the enemy
        /// </summary>
        public void InitLife(int healthAmountAdd) => currentHealth = maxHealth + healthAmountAdd;
        /// <summary>
        /// Init the renderer of the car
        /// </summary>
        protected override void InitRenderData() {
            base.InitRenderData();

            if (metalParts.Count == 0) return;
            Material metalMat = new Material(metalParts[0].material);
            foreach (MeshRenderer mesh in metalParts) {
                mesh.material = metalMat;
            }
        }
        
        public override bool TakeDamage(int damage) {
            if (!IsDamageable()) return false;
            currentHealth -= damage;
            TakeDamageCarFeedback();

            TextEffect txt = PoolManager.Instance.RetrieveOrCreateObject(damageTxtKey, transform.position + Vector3.up * 5, quaternion.identity).GetComponent<TextEffect>();
            txt.SetDamageText(damage);
            
            return base.TakeDamage(damage);
        }

        protected override void CarDeath() {
            isDead = true;
            
            /* DROP PICKABLE OBJECTS
             if (objectPickable != null) {
                objectPickable.GetComponent<ObjectPickable>().OnDrop();
                objectPickable.GetComponent<ObjectPickable>().rb.AddForce(Vector3.up * 100);
                objectPickable = null;
            }*/
            
            /* GOLD
            int gGive = Random.Range(1, 4);
            if (isAimEffect) gGive += 10;
            CarAbilitiesManager.instance.GoldAmountWonOnRun += gGive;
            TextEffect txt = PoolManager.Instance.RetrieveOrCreateObject(damageTextKey, transform.position + Vector3.up * 5, quaternion.identity).GetComponent<TextEffect>();
            txt.SetGoldText(gGive);*/
            
            CarAbilitiesManager.instance.OnEnemyKilled?.Invoke(gameObject);
            PlayerCarController.Instance.playerExperienceManager.GetPlayerExperience(GetComponent<BasePoliceCarBehavior>().GetXPForPlayer());
            Debug.Log(GetComponent<BasePoliceCarBehavior>().GetXPForPlayer());
            PoolManager.Instance.RemoveObjectFromScene(gameObject);
            
            /* JE SUIS EFFRAYE
            OnPoliceCarDie?.Invoke(gameObject);
            OnPoliceCarDie = null;*/
        }

        public override bool IsDamageable() => gameObject.activeSelf && currentHealth > 0;
    }
}