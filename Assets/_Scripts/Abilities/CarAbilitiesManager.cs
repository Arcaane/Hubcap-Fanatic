using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DefaultDelegate();
public delegate void TransformDelegate(Transform tr);
public delegate void ObjectDelegate(GameObject obj);
public delegate void CollisionDelegate(Collision col);


namespace Abilities
{
    public class CarAbilitiesManager : MonoBehaviour
    {
        public static CarAbilitiesManager instance;

        public AbilitiesSO firstAbility;
        
        public CarController car;
        
        public ObjectDelegate OnEnemyCollision = delegate {  };
        public CollisionDelegate OnWallCollision = delegate {  };
        public DefaultDelegate OnStateEnter = delegate {  };
        public DefaultDelegate OnStateExit = delegate {  };
        public ObjectDelegate OnEnemyDamageTaken = delegate {  };
        public DefaultDelegate OnPlayerDamageTaken = delegate {  };
        public DefaultDelegate OnUpdate = delegate {  };

        public List<AbilitiesSO> abilities;
        
        public int slotAbilitiesAmount = 4;
        public int goldAmountWonOnRun;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            car = CarController.instance;
            if(firstAbility) AddAbility(firstAbility);
        }

        //[Header("KIT")]
        [SerializeField] public int damageOnCollisionWithEnemy;
        
        public void Update()
        {
            OnUpdate.Invoke();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.transform.CompareTag("Enemy")) return;

            OnEnemyCollision.Invoke(collision.transform.gameObject);
            
            var a = collision.transform.transform.position - transform.position; a.Normalize();
            var b = transform.forward; b.Normalize();
            
            if (Vector3.Dot(b,  a) < -0.70f) collision.transform.GetComponent<IDamageable>()?.TakeDamage(damageOnCollisionWithEnemy);
        }

        public LayerMask enemyLayerMask;

        #region Abilities

        public void AddAbility(AbilitiesSO abilitySo)
        {
            if (abilitySo.type == AbilityType.ClassicAbilites)
            {
                if (abilities.Contains(abilitySo))
                {
                    abilitySo.level++;
                    abilitySo.LevelUpStats();
                }
                else
                {
                    abilities.Add(abilitySo);   
                    abilitySo.Initialize();
                }
            }

            if (abilitySo.type == AbilityType.GoldGiver)
            {
                goldAmountWonOnRun += 50;
            }
        }

        #endregion
        
        private void OnGUI()
        {
            GUI.Label(new Rect(50, 550, 500, 150), "Target Speed: " + car.targetSpeed);
            GUI.Label(new Rect(50, 570, 500, 150), "Max Speed: " + car.maxSpeed);
            GUI.Label(new Rect(50, 590, 500, 150), "Current Speed: " + car.rb.velocity.magnitude);
        }

        public bool IsPlayerFullAbilities() => abilities.Count == slotAbilitiesAmount;
        
        public void AddAbilityIconOnSlot(Sprite sprite) => UIManager.instance.abilitiesSlots[abilities.Count].passiveAbilityIcon.sprite = sprite;
        
        [ContextMenu("UnlockAbilitySlot")]
        public void UnlockAbilitySlot(int i)
        {
            for (int j = 0; j < i; j++)
            {
                UIManager.instance.UnlockAbilitySlot(abilities.Count+1);
            }
        }
    }
}

public enum AbilitySocket
{
    AbilityNitro,
    AbilityDrift,
}
