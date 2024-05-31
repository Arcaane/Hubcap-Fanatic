using System.Threading.Tasks;
using UnityEngine;

namespace HubcapCarBehaviour {
    public class BaseCarHealthManager : MonoBehaviour, IDamageable {
        [Header("HEALTH INFORMATION")] 
        [SerializeField] protected int maxHealth = 100;
        [SerializeField, ReadOnly] protected int currentHealth = 0;
        public int CurrentHealth => currentHealth;
        [SerializeField, ReadOnly] protected bool isDead = false;
        [SerializeField, ReadOnly, Range(0,1)] protected float armorPercent = 0;

        [Header("CAR RENDERER")] 
        [SerializeField] protected MeshRenderer rend = null;
        protected Material[] carMats;


        protected virtual void Start() {
            currentHealth = maxHealth;
            InitRenderData();
        }

        /// <summary>
        /// Method called to apply damage to this car
        /// </summary>
        /// <param name="damage">The damage to apply to this target</param>
        /// <returns>Return true if the target die from the damage</returns>
        public virtual bool TakeDamage(int damage) {
            if (currentHealth < 1) {
                CarDeath();
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Method called when the car need to die
        /// </summary>
        protected virtual async void CarDeath() {}

        /// <summary>
        /// Method which can add a certain amount of heal to the car
        /// </summary>
        /// <param name="healAmount">The amount of heal to bring back to the car</param>
        public virtual void HealCar(int healAmount) {
            if (isDead) return;
            currentHealth = Mathf.Clamp(currentHealth + healAmount, 0, maxHealth);
        }

        /// <summary>
        /// Method which say if the target car take damage or not
        /// </summary>
        /// <returns></returns>
        public virtual bool IsDamageable() => !isDead;
        
        #region VISUALS
        
        /// <summary>
        /// Update the materials of the car when taking damage
        /// </summary>
        protected async void TakeDamageCarFeedback() {
            UpdateDamageMaterial(1);
            await Task.Delay(300);
            UpdateDamageMaterial(0);
        }
        
        /// <summary>
        /// Initialize new materials for the car
        /// </summary>
        protected virtual void InitRenderData() {
            carMats = new Material[rend.materials.Length];
            for (int i = 0; i < carMats.Length; i++) {
                carMats[i] = new Material(rend.materials[i]);
            }
            rend.materials = carMats;
        }
        
        /// <summary>
        /// Update all materials based on value
        /// </summary>
        /// <param name="amount"></param>
        protected void UpdateDamageMaterial(float amount) {
            foreach (Material mat in carMats) {
                mat.SetFloat("_UseDamage", amount);
            }
        }
        
        #endregion VISUALS
    }
}