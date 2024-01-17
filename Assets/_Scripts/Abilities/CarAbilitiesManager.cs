using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
                    AddPassiveAbilityIconOnSlot(abilitySo.abilitySprite);
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

        [Space] 
        [SerializeField] private Color passiveAbilityUnlockedIconBackgroundColor;
        [SerializeField] private Color statAbilityUnlockedIconBackgroundColor;
        [SerializeField] private Color abilitiesUnlockedIconBackgroundColor;
        
        public void AddPassiveAbilityIconOnSlot(Sprite sprite)
        {
            UIManager.instance.abilitiesSlots[abilities.Count-1].passiveAbilityIcon.sprite = sprite;
            UIManager.instance.abilitiesSlots[abilities.Count-1].passiveAbilityIcon.gameObject.SetActive(true);
            
            // Parent transparence -> 255
            UIManager.instance.abilitiesSlots[abilities.Count - 1].passiveAbilityIcon.transform.parent.GetComponent<Image>().color = passiveAbilityUnlockedIconBackgroundColor;
            // Parent.parent transparence -> 255
            UIManager.instance.abilitiesSlots[abilities.Count - 1].passiveAbilityIcon.transform.parent.parent.GetComponent<Image>().color = abilitiesUnlockedIconBackgroundColor;
        }
        
        public void AddStatAbilityIconOnSlot(Sprite sprite)
        {
            UIManager.instance.abilitiesSlots[abilities.Count-1].statAbilityIcon.sprite = sprite;
            UIManager.instance.abilitiesSlots[abilities.Count-1].statAbilityIcon.gameObject.SetActive(true);
            
            // Parent transparence -> 255
            UIManager.instance.abilitiesSlots[abilities.Count - 1].passiveAbilityIcon.transform.parent.GetComponent<Image>().color = statAbilityUnlockedIconBackgroundColor;
            // Parent.parent transparence -> 255
            UIManager.instance.abilitiesSlots[abilities.Count - 1].passiveAbilityIcon.transform.parent.parent.GetComponent<Image>().color = abilitiesUnlockedIconBackgroundColor;
        }
        
        [ContextMenu("UnlockAbilitySlot")]
        public void UnlockAbilitySlot()
        {
            slotAbilitiesAmount++;
            UIManager.instance.UnlockAbilitySlot(abilities.Count);
        }
    }
}

public enum AbilitySocket
{
    AbilityNitro,
    AbilityDrift,
}
