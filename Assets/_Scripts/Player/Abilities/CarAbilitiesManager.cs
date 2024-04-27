using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
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
        
        [FormerlySerializedAs("car")] public PlayerCarController playerCar;
        
        public ObjectDelegate OnEnemyCollision = delegate {  };
        public CollisionDelegate OnWallCollision = delegate {  };
        public DefaultDelegate OnStateExit = delegate {  };
        public ObjectDelegate OnEnemyDamageTaken = delegate {  };
        public DefaultDelegate OnPlayerDamageTaken = delegate {  };
        public DefaultDelegate OnUpdate = delegate {  };
        public ObjectDelegate OnEnemyHitWithShotgun = delegate {  };
        public DefaultDelegate OnShotgunUsed = delegate {  };
        public DefaultDelegate OnBeginNitro = delegate {  };
        public DefaultDelegate OnBeginDrift = delegate {  };
        public DefaultDelegate OnPill = delegate {  };
        public ObjectDelegate OnEnemyKilled = delegate {  };
        public DefaultDelegate OnShotgunUsedWithoutTarget = delegate {  };

        public List<AbilitiesSO> passiveAbilities;
        public List<AbilitiesSO> statsAbilities;
        
        public int slotAbilitiesAmount = 4;
        
        private int goldAmountWonOnRun;
        public int GoldAmountWonOnRun
        {
            get => goldAmountWonOnRun;
            set
            {
                goldAmountWonOnRun = value;
                UIManager.Instance.SetGoldText(goldAmountWonOnRun);
            }
        }

        [SerializeField] public int damageOnCollisionWithEnemy;
        [SerializeField] public float overallAbilitiesCooldown = 0;
        
        [HideInInspector] public float baseSpeedOnRoad;
        [HideInInspector] public float baseSpeedOnSand;
        [HideInInspector] public float baseNitroSpeed;
        [HideInInspector] public float baseNitroCooldown;
        [HideInInspector] public float baseShotgunDamage;
        [HideInInspector] public float baseCollisionDamage;
        [HideInInspector] public float baseCritDamage = 0;
        [HideInInspector] public float baseShotgunDuration = 0;
        [HideInInspector] public float baseMaxHealth = 0;
        [HideInInspector] public float baseArmorPercent = 0;
        [HideInInspector] public float baseAttackCooldown = 0;
        [HideInInspector] public float baseOverallAbilitiesCooldown = 0;
        [HideInInspector] public float baseHitBeforeDeliverDrop = 0;
        [HideInInspector] public float baseSpeedRetainedOnBounce = 0;

        [HideInInspector] public float powerUpMight;
        [HideInInspector] public float powerUpMoveSpeed;
        
        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            playerCar = PlayerCarController.Instance;
            if(firstAbility) AddAbility(firstAbility);

            baseSpeedOnRoad = playerCar.maxSpeed;
            baseSpeedOnSand = playerCar.offRoadSpeed;
            baseNitroSpeed = playerCar.speedWithNitro;
            baseNitroCooldown = playerCar.nitroRegen;
            baseShotgunDamage = playerCar.shotgunDamages;
            baseCollisionDamage = damageOnCollisionWithEnemy;
            baseShotgunDuration = playerCar.shootDuration;
            baseMaxHealth = CarHealthManager.instance.maxLifePoints;
            baseAttackCooldown = playerCar.shootDuration;
            baseOverallAbilitiesCooldown = 0;
            baseArmorPercent = CarHealthManager.instance.armorInPercent;
            baseHitBeforeDeliverDrop = playerCar.CollsionBeforeDropDeliver;
            baseSpeedRetainedOnBounce = playerCar.speedRetained;
            goldAmountWonOnRun = 0;
            
            powerUpMight = 1 /*+ GameMaster.instance.UnlockedPowerUps[0] * 0.05f*/;
            playerCar.mightPowerUpLevel = powerUpMight;
            
            powerUpMoveSpeed = 1 /*+ GameMaster.instance.UnlockedPowerUps[2] * 0.05f*/;
            Debug.Log(" Power up speed " + powerUpMoveSpeed);
            playerCar.maxSpeed *= powerUpMoveSpeed;
            playerCar.offRoadSpeed *= powerUpMoveSpeed;
        }

        public void Update()
        {
            OnUpdate.Invoke();
            overheatTimer += Time.deltaTime;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.transform.CompareTag("Enemy")) return;

            OnEnemyCollision.Invoke(collision.transform.gameObject);
            
            var a = collision.transform.transform.position - transform.position; a.Normalize();
            var b = transform.forward; b.Normalize();

            if (Vector3.Dot(b, a) < -0.70f)
            {
                IDamageable damageable = collision.transform.GetComponent<IDamageable>();
                damageable?.TakeDamage(Mathf.FloorToInt(damageOnCollisionWithEnemy * powerUpMight));
                OnEnemyDamageTaken.Invoke(collision.gameObject);
            }
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
                    abilitySo.index = passiveAbilities.Count - 1;
                    abilitySo.level = 0;
                    abilitySo.Initialize();
                    AddPassiveAbilityIconOnSlot(abilitySo.abilitySprite);
                }
            }

            if (abilitySo.type == AbilityType.GoldGiver)
            {
                goldAmountWonOnRun += abilitySo.effectDamage;
                UIManager.Instance.SetGoldText(goldAmountWonOnRun);
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
                    abilitySo.level = 0;
                    statsAbilities.Add(abilitySo);   
                    abilitySo.Initialize();
                    AddStatAbilityIconOnSlot(abilitySo.abilitySprite);
                }
            }
        }
        #endregion
        
        public bool IsPlayerFullAbilities() => passiveAbilities.Count == slotAbilitiesAmount;

        [Space] 
        [SerializeField] private Color passiveAbilityUnlockedIconBackgroundColor;
        [SerializeField] private Color statAbilityUnlockedIconBackgroundColor;
        [SerializeField] private Color abilitiesUnlockedIconBackgroundColor;
        
        public void AddPassiveAbilityIconOnSlot(Sprite sprite)
        {
            UIManager.Instance.abilitiesSlots[passiveAbilities.Count-1].passiveAbilityIcon.sprite = sprite;
            UIManager.Instance.abilitiesSlots[passiveAbilities.Count-1].passiveAbilityIcon.gameObject.SetActive(true);
            
            // Parent transparence -> 255
            UIManager.Instance.abilitiesSlots[passiveAbilities.Count-1].passiveAbilityIcon.transform.parent.GetComponent<Image>().color = passiveAbilityUnlockedIconBackgroundColor;
            // Parent.parent transparence -> 255
            UIManager.Instance.abilitiesSlots[passiveAbilities.Count-1].passiveAbilityIcon.transform.parent.parent.GetComponent<Image>().color = abilitiesUnlockedIconBackgroundColor;
        }
        
        public void AddStatAbilityIconOnSlot(Sprite sprite)
        {
            UIManager.Instance.abilitiesSlots[statsAbilities.Count-1].statAbilityIcon.sprite = sprite;
            UIManager.Instance.abilitiesSlots[statsAbilities.Count-1].statAbilityIcon.gameObject.SetActive(true);
            
            // Parent transparence -> 255
            UIManager.Instance.abilitiesSlots[statsAbilities.Count-1].statAbilityIcon.transform.parent.GetComponent<Image>().color = statAbilityUnlockedIconBackgroundColor;
            // Parent.parent transparence -> 255
            UIManager.Instance.abilitiesSlots[statsAbilities.Count-1].statAbilityIcon.transform.parent.parent.GetComponent<Image>().color = abilitiesUnlockedIconBackgroundColor;
        }
        
        [ContextMenu("UnlockAbilitySlot")]
        public void UnlockAbilitySlot()
        {
            slotAbilitiesAmount++;
            UIManager.Instance.UnlockAbilitySlot(slotAbilitiesAmount-1);
        }

        public void LaunchCo(float timer, int index)
        {
            StartCoroutine(DecreaseTimer(timer, index));
        }
        
        IEnumerator DecreaseTimer(float timer, int index)
        {
            var a = timer;
            
            while (a > 0)
            {
                a -= Time.deltaTime;
                UIManager.Instance.abilitiesSlots[index].cdText.text = a.ToString(timer < 1 ? "F" : "G1");
                yield return null;
            }
        }
        
        public float overheatTimer;
        public float overheatCooldownDuration = 5f;
        public float overheatDuration = 20f;
        public void YButton(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                if (overheatTimer > overheatCooldownDuration + overheatDuration)
                {
                    overheatTimer = 0;
                    OverheatDuration((int)overheatDuration * 1000);
                }
            }
            
            async void OverheatDuration(int overHeatDuration)
            {
                for (int i = 0; i < statsAbilities.Count; i++) { if (statsAbilities[i].isOverheatable) statsAbilities[i].isOverheat = true; }
                for (int i = 0; i < passiveAbilities.Count; i++){ if (passiveAbilities[i].isOverheatable) passiveAbilities[i].isOverheat = true; }
            
                await Task.Delay(overHeatDuration);
            
                for (int i = 0; i < statsAbilities.Count; i++) { if (statsAbilities[i].isOverheatable) statsAbilities[i].isOverheat = false; }
                for (int i = 0; i < passiveAbilities.Count; i++){ if (passiveAbilities[i].isOverheatable) passiveAbilities[i].isOverheat = false; }
            }
        }
        
    }
}

public enum AbilitySocket
{
    AbilityNitro,
    AbilityDrift,
}
