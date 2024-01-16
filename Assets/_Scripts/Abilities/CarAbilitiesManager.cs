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

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Enemy")) return;

            OnEnemyCollision.Invoke(other.gameObject);
            
            if (Vector3.Dot(other.transform.position - transform.position, transform.forward) > 0.75f) // EnemyCollision
            {
                other.GetComponent<IDamageable>()?.TakeDamage(damageOnCollisionWithEnemy);
                if (car.nitroMode)
                {
                    //OnCollideWithNitro.Invoke();
                }
            }
        }

        public LayerMask enemyLayerMask;

        #region Abilities

        public void AddAbility(AbilitiesSO abilitySo)
        {
            if (abilities.Contains(abilitySo))
            {
                abilitySo.LevelUpStats();
            }
            else
            {
                abilities.Add(abilitySo);   
                abilitySo.Initialize();
            }
        }

        #endregion
        

        private void OnGUI()
        {
            GUI.Label(new Rect(50, 550, 500, 150), "Target Speed: " + car.targetSpeed);
            GUI.Label(new Rect(50, 570, 500, 150), "Max Speed: " + car.maxSpeed);
            GUI.Label(new Rect(50, 590, 500, 150), "Current Speed: " + car.rb.velocity.magnitude);
        }
    }

}

public enum AbilitySocket
{
    AbilityNitro,
    AbilityDrift,
}
