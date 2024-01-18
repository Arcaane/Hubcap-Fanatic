using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public delegate void DefaultDelegate();
public delegate void GameObjectDelgate(GameObject tr);
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

        public List<AbilitiesSO> passiveAbilities;
        public List<AbilitiesSO> statsAbilities;
        
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
                if (passiveAbilities.Contains(abilitySo))
                {
                    abilitySo.level++;
                    abilitySo.LevelUpPassiveAbility();
                }
                else
                {
                    passiveAbilities.Add(abilitySo);   
                    abilitySo.Initialize();
                    AddPassiveAbilityIconOnSlot(abilitySo.abilitySprite);
                }
            }

            if (abilitySo.type == AbilityType.GoldGiver)
            {
                goldAmountWonOnRun += 50;
            }

            if (abilitySo.type == AbilityType.UpgrateStatsAbilites)
            {
                if (statsAbilities.Contains(abilitySo))
                {
                    abilitySo.level++;
                    abilitySo.LevelUpStatsAbility();
                }
                else
                {
                    statsAbilities.Add(abilitySo);   
                    abilitySo.Initialize();
                    AddStatAbilityIconOnSlot(abilitySo.abilitySprite);
                }
            }
        }

        #endregion
        
        private void OnGUI()
        {
            GUI.Label(new Rect(50, 550, 500, 150), "Target Speed: " + car.targetSpeed);
            GUI.Label(new Rect(50, 570, 500, 150), "Max Speed: " + car.maxSpeed);
            GUI.Label(new Rect(50, 590, 500, 150), "Current Speed: " + car.rb.velocity.magnitude);
        }

        public bool IsPlayerFullAbilities() => passiveAbilities.Count == slotAbilitiesAmount;

        [Space] 
        [SerializeField] private Color passiveAbilityUnlockedIconBackgroundColor;
        [SerializeField] private Color statAbilityUnlockedIconBackgroundColor;
        [SerializeField] private Color abilitiesUnlockedIconBackgroundColor;
        
        public void AddPassiveAbilityIconOnSlot(Sprite sprite)
        {
            UIManager.instance.abilitiesSlots[passiveAbilities.Count-1].passiveAbilityIcon.sprite = sprite;
            UIManager.instance.abilitiesSlots[passiveAbilities.Count-1].passiveAbilityIcon.gameObject.SetActive(true);
            
            // Parent transparence -> 255
            UIManager.instance.abilitiesSlots[passiveAbilities.Count-1].passiveAbilityIcon.transform.parent.GetComponent<Image>().color = passiveAbilityUnlockedIconBackgroundColor;
            // Parent.parent transparence -> 255
            UIManager.instance.abilitiesSlots[passiveAbilities.Count-1].passiveAbilityIcon.transform.parent.parent.GetComponent<Image>().color = abilitiesUnlockedIconBackgroundColor;
        }
        
        public void AddStatAbilityIconOnSlot(Sprite sprite)
        {
            UIManager.instance.abilitiesSlots[statsAbilities.Count-1].statAbilityIcon.sprite = sprite;
            UIManager.instance.abilitiesSlots[statsAbilities.Count-1].statAbilityIcon.gameObject.SetActive(true);
            
            // Parent transparence -> 255
            UIManager.instance.abilitiesSlots[statsAbilities.Count-1].statAbilityIcon.transform.parent.GetComponent<Image>().color = statAbilityUnlockedIconBackgroundColor;
            // Parent.parent transparence -> 255
            UIManager.instance.abilitiesSlots[statsAbilities.Count-1].statAbilityIcon.transform.parent.parent.GetComponent<Image>().color = abilitiesUnlockedIconBackgroundColor;
        }
        
        [ContextMenu("UnlockAbilitySlot")]
        public void UnlockAbilitySlot()
        {
            slotAbilitiesAmount++;
            UIManager.instance.UnlockAbilitySlot(passiveAbilities.Count);
        }
    }
}

public enum AbilitySocket
{
    AbilityNitro,
    AbilityDrift,
}
