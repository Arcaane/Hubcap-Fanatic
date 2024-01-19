using System;
using System.Threading.Tasks;
using ManagerNameSpace;
using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu(menuName = "Ability", fileName = "New Ability")]
    public class AbilitiesSO : ScriptableObject
    {
        [Header("ABILITY TYPE")]
        public AbilityType type = AbilityType.ClassicAbilites;
        
        [Header("Informations")] 
        public string abilityName;
        [TextArea(3, 3)] public string description;
        public Sprite abilitySprite;
        public int level = 0;

        [Space(5)] [Header("Abilities Creation")]
        public AbilityTrigger trigger;
        public TargetAbility target;
        public When when;
        public State state;
        public Effect effect;

        [Space(5)] [Header("Abilities Level Modifiers")]
        public ActiveModifier[] modifiersLevel1;
        public ActiveModifier[] modifiersLevel2;
        public ActiveModifier[] modifiersLevel3;
        
        // Stats
        [Header("Classic Abilities")]
        public int effectDamage;
        public int effectSizeRadius;
        public float effectDuration;
        public int effectDelayMilliseconds;
        public float effectRepeatDelay;
        public bool isCapacityCooldown;
        public float cooldownDuration;

        [Header("Stats Abilities")] 
        public StatsModifier statsModifier; 
        public HowStatsModify howStatsModify;
        public float[] amount;
        
        // Target Objects
        private GameObject returnedTargetObject;
        private Collision returnedCollision;
        
        // Memory Vars
        private Collider[] cols; 
        private float effectRepeatTimer;
        private CarController player;
        private CarAbilitiesManager carAbilities;
        private bool isInCooldown;
        
        [Space(5)]
        [Header("Layer Masks")]
        public LayerMask enemyLayerMask;
        
        public void Initialize()
        {
            player = CarController.instance;
            carAbilities = CarAbilitiesManager.instance;

            if (type == AbilityType.ClassicAbilites)
            {
                switch (trigger)
                {
                    case AbilityTrigger.OnEnemyCollision:  carAbilities.OnEnemyCollision += Activate ; break;
                    case AbilityTrigger.OnWallCollision: carAbilities.OnWallCollision += Activate ; break;
                    case AbilityTrigger.OnEnterState: carAbilities.OnStateEnter += Activate ; break;
                    case AbilityTrigger.OnExitState: carAbilities.OnStateExit += Activate ; break;
                    case AbilityTrigger.OnUpdateState: carAbilities.OnUpdate += Activate; break;
                    case AbilityTrigger.OnEnemyDamageDealt: carAbilities.OnEnemyDamageTaken += Activate ; break;
                    case AbilityTrigger.OnPlayerDamageDealt: carAbilities.OnPlayerDamageTaken += Activate ; break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
            else if (type == AbilityType.UpgrateStatsAbilites)
            {
                LevelUpStatsAbility();
            }
            else if (type == AbilityType.GoldGiver)
            {
                carAbilities.goldAmountWonOnRun += (int)amount[0];
            }
        }
        
        public void Activate()
        {
            ApplyWhenModifiers();
        }
        
        public void Activate(GameObject targetObj)
        {
            returnedTargetObject = targetObj;
            ApplyWhenModifiers();
        }
        
        public void Activate(Collision collision)
        {
            Debug.Log("Trigger Activated " + abilityName);
            returnedCollision = collision;
            ApplyWhenModifiers();
        }
        
        public void ApplyWhenModifiers()
        {
            switch (state)
            {
                case State.All: break;
                case State.Default: if(!player.isDefault); return;
                case State.Drift: if (!player.driftBrake) return; break;
                case State.Straff: if(!player.isStraffing) return; break;
                case State.Pill: if(!player.brakeMethodApplied) return; break;
                case State.Nitro: if(!player.nitroMode) return; break;
                default: throw new ArgumentOutOfRangeException(); break;
            }
            
            if (isInCooldown)
            {
                Debug.Log("Is in cooldown");
                return;
            }
            
            switch (target)
            {
                case TargetAbility.HitEnemy: ApplyEffectOnTarget(returnedTargetObject); break;
                case TargetAbility.Player: ApplyEffectOnTarget(player.gameObject); break;
                case TargetAbility.ZoneAroundHitEnemy: ApplyEffectOnTargetsInZone(returnedTargetObject.transform.position,effectSizeRadius); break;
                case TargetAbility.ZoneAroundPlayer: ApplyEffectOnTargetsInZone(player.transform.position, effectSizeRadius); break;
                case TargetAbility.ClosestEnemyToPlayer: ApplyEffectOnClosestTarget(player.transform.position, effectSizeRadius); break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        #region Target

        public void ApplyEffectOnTarget(GameObject targetObj)
        {
            switch (when)
            {
                case When.Immediate: ApplyEffect(targetObj);
                    break;
                case When.Delayed: DelayEffect(targetObj,effectDelayMilliseconds);
                    break;
                case When.OnTargetDie: OnEnemyDieApplyEffect(targetObj);
                    break;
                case When.EveryXSeconds: RepeatedEffect(targetObj);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(when), when, null);
            }
            
            SetEffectInCooldown();
        }
        
        private void OnEnemyDieApplyEffect(GameObject targetObj)
        {
            if (targetObj == null) return;

            PoliceCarBehavior enemy = targetObj.GetComponent<PoliceCarBehavior>();
            if (enemy)
            {
                enemy.OnPoliceCarDie -= ApplyEffect;
                enemy.OnPoliceCarDie += ApplyEffect;
            }

            ConvoyBehaviour convoy = targetObj.GetComponent<ConvoyBehaviour>();
            if (convoy)
            {
                Debug.Log("Add effect on convoy die");   
            }
        }

        public void ApplyEffectOnTargetsInZone(Vector3 zonePos,float zoneRadius)
        {
            cols = Physics.OverlapSphere(zonePos, zoneRadius, enemyLayerMask);
            if (cols.Length > 0)
            {
                for (int i = 0; i < cols.Length; i++)
                {
                    ApplyEffectOnTarget(cols[i].gameObject);
                }
            }
        }

        public void ApplyEffectOnClosestTarget(Vector3 zonePos,float zoneRadius)
        {
            float tempShorterDst = 10000;
            int shorterIndex = -1;
            
            cols = Physics.OverlapSphere(zonePos, zoneRadius, enemyLayerMask);
            if (cols.Length > 0)
            {
                for (int i = 0; i < cols.Length; i++)
                {
                    var tempDst = Vector3.Distance(zonePos, cols[i].transform.position);
                
                    if (tempDst < tempShorterDst)
                    {
                        tempShorterDst = tempDst;
                        shorterIndex = i;
                    }
                }
            
                ApplyEffectOnTarget(cols[shorterIndex].gameObject);
            }
        }

        #endregion

        #region When

        private void ApplyEffect(GameObject targetObj)
        {
            switch (effect)
            {
                case Effect.Explosion: EffectExplosion(targetObj); break;
                case Effect.Spear: EffectSpear(targetObj); break;
                case Effect.Damage: EffectDamage(targetObj); break;
                case Effect.ForceBreak: EffectForceBreak(targetObj); break;
                case Effect.SpawnMine: EffectSpawnMine(targetObj); break;
                case Effect.LifeSteal: EffectLifeSteal(targetObj); break;
                case Effect.Scorch: EffectScorch(targetObj); break;
                case Effect.Berserk: EffectBerserk(targetObj); break;
                case Effect.Shield: EffectShield(targetObj); break;
                case Effect.Information: EffectInformation(targetObj); break;
                case Effect.Target: EffectTarget(targetObj); break;
                default: throw new ArgumentOutOfRangeException(nameof(effect), effect, null);
            }
        }

        private async void DelayEffect(GameObject targetObj,int delayInMS)
        {
            await Task.Delay(delayInMS);
            ApplyEffect(targetObj);
        }

        private void RepeatedEffect(GameObject targetObj)
        {
            if (effectRepeatTimer > 0) effectRepeatTimer -= Time.deltaTime;
            else
            {
                effectRepeatTimer = effectRepeatDelay;
                ApplyEffect(targetObj);
            }
        }

        #endregion
        
        #region EffectFunctions

        private void EffectSpear(GameObject targetObj)
        {
            var carPos = player.transform.position;
            Vector3 relativePos = carPos - targetObj.transform.position;
            Pooler.instance.SpawnInstance(Key.OBJ_Spear, carPos, Quaternion.LookRotation(-relativePos));
        }
    
        private void EffectDamage(GameObject targetObj)
        {
            IDamageable damageable = targetObj.GetComponent<IDamageable>();
            damageable.TakeDamage(effectDamage);
        }
    
        private void EffectForceBreak(GameObject targetObj)
        {
            CarBehaviour carBehaviour = targetObj.GetComponent<CarBehaviour>();
            carBehaviour.forceBreak = true;
            carBehaviour.forceBreakTimer = effectDuration;
            GameObject go = Pooler.instance.SpawnTemporaryInstance(Key.FX_MotorBreak, targetObj.transform.position + new Vector3(0,0.5f,0), Quaternion.identity).gameObject;
            go.transform.SetParent(targetObj.transform);
            go.SetActive(true);
        }

        private void EffectExplosion(GameObject targetObj)
        {
            var a = Instantiate(carAbilities.testEffectsPrefab, targetObj.transform.position, Quaternion.identity);
            a.transform.localScale = new Vector3(effectSizeRadius, effectSizeRadius,effectSizeRadius);
            
            var cols = Physics.OverlapSphere(targetObj.transform.position, effectSizeRadius, enemyLayerMask);
            for (int i = 0; i < cols.Length; i++)
            {
                cols[i].GetComponent<IDamageable>()?.TakeDamage(effectDamage);
            }
        }

        private void EffectSpawnMine(GameObject targetObj)
        { 
            Pooler.instance.SpawnInstance(Key.OBJ_Mine, targetObj.transform.position, Quaternion.identity);
        }

        private void EffectLifeSteal(GameObject targetObj)
        {
            targetObj.GetComponent<IDamageable>().TakeDamage(effectDamage);
            CarHealthManager.instance.TakeHeal(effectDamage);
        }
        
        private async void EffectScorch(GameObject targetObj)
        {
            CarBehaviour carBehaviour = targetObj.GetComponent<CarBehaviour>();
            if (carBehaviour.isScorch) return;
            carBehaviour.isScorch = true;
            
            var a = Mathf.FloorToInt(effectDuration / ((float)effectDelayMilliseconds / 1000));
            for (int i = 0; i < a; i++)
            {
                targetObj.GetComponent<IDamageable>()?.TakeDamage(effectDamage);
                await Task.Delay(effectDelayMilliseconds);
            }

            if (carBehaviour == null) return;
            carBehaviour.isScorch = false;
        }

        private async void EffectBerserk(GameObject targetObj)
        {
            CarController car = targetObj.GetComponent<CarController>();
            if (!car) return;
            var baseStraffDuration = car.shootDuration;
            car.shootDuration = effectDamage;
            await Task.Delay(Mathf.FloorToInt(effectDuration * 1000));
            if (car) return;
            car.shootDuration = baseStraffDuration;
        }
        
        private async void EffectShield(GameObject targetObj)
        {
            CarController car = targetObj.GetComponent<CarController>();
            if (!car) return;
            car.shield.SetActive(true);
            await Task.Delay(Mathf.FloorToInt(effectDuration * 1000));
            if (!car) return;
            car.shield.SetActive(false);
        }

        private async void EffectInformation(GameObject targetObj)
        {
            var uiTargets = UIIndic.instance.targetUIPrefab;
            for (int i = 0; i < uiTargets.Count; i++)
            {
                if (uiTargets[i].targetType == TargetType.Convoy)
                    UIIndic.instance.EnableOrDisableSpecificUI(i, true);
                
                if (uiTargets[i].targetType == TargetType.Merchant)
                    UIIndic.instance.EnableOrDisableSpecificUI(i, true);
            }

            await Task.Delay(Mathf.FloorToInt(effectDuration * 1000));
            
            for (int i = 0; i < uiTargets.Count; i++)
            {
                if (uiTargets[i].targetType == TargetType.Convoy && uiTargets[i].gameObject.activeSelf)
                    UIIndic.instance.EnableOrDisableSpecificUI(i);
                
                if (uiTargets[i].targetType == TargetType.Merchant && uiTargets[i].gameObject.activeSelf)
                    UIIndic.instance.EnableOrDisableSpecificUI(i);
            }
        }

        private async void EffectTarget(GameObject obj)
        {
            var a = obj.GetComponent<CarBehaviour>();
            if (!a) return;
            a.isAimEffect = true;
            await Task.Delay(Mathf.FloorToInt(effectDuration * 1000));
            if (!a) return;
            a.isAimEffect = false;
        }
        
        #endregion


        #region Stats

        public void LevelUpPassiveAbility()
        {
            
            if (level > 3) return;

            ActiveModifier[] modifiers = level switch
            {
                1 => modifiersLevel1,
                2 => modifiersLevel2,
                3 => modifiersLevel3,
                _ => throw new ArgumentOutOfRangeException()
            };

            if (modifiers == null) return;
            
            for (int i = 0; i < modifiers.Length; i++)
            {
                switch (modifiers[i].stat)
                {
                    case AbilitiesStats.EffectDamage: effectDamage += (int)modifiers[i].newValue; break;
                    case AbilitiesStats.EffectSizeRadius: effectSizeRadius += (int)modifiers[i].newValue;  break;
                    case AbilitiesStats.EffectDuration: effectDuration += modifiers[i].newValue; break;
                    case AbilitiesStats.EffectDelayMilliseconds: effectDelayMilliseconds += (int)modifiers[i].newValue; break;
                    case AbilitiesStats.EffectRepeatDelay: effectRepeatDelay += modifiers[i].newValue; break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void LevelUpStatsAbility()
        {
            switch (statsModifier)
            {
                case StatsModifier.SpeedOnRoad: switch (howStatsModify)
                    {
                        case HowStatsModify.Subtract: CarController.instance.maxSpeed = carAbilities.baseSpeedOnRoad - amount[level]; break;
                        case HowStatsModify.Add: CarController.instance.maxSpeed = carAbilities.baseSpeedOnRoad + amount[level];; break;
                        case HowStatsModify.Multiply: CarController.instance.maxSpeed = carAbilities.baseSpeedOnRoad * amount[level];; break;
                    } break;
                case StatsModifier.SpeedOnSand: switch (howStatsModify) {
                        case HowStatsModify.Subtract: player.offRoadSpeed = carAbilities.baseSpeedOnSand - amount[level]; break;
                        case HowStatsModify.Add: player.offRoadSpeed = carAbilities.baseSpeedOnSand + amount[level]; break;
                        case HowStatsModify.Multiply: player.offRoadSpeed = carAbilities.baseSpeedOnSand * amount[level]; break;
                    } break;
                case StatsModifier.NitroSpeed: switch (howStatsModify)
                    {
                        case HowStatsModify.Subtract: player.nitroSpeed = carAbilities.baseNitroSpeed + amount[level]; break;
                        case HowStatsModify.Add: player.nitroSpeed = carAbilities.baseNitroSpeed - amount[level]; break;
                        case HowStatsModify.Multiply: player.nitroSpeed = carAbilities.baseNitroSpeed * amount[level]; break;
                        default: throw new ArgumentOutOfRangeException();
                    } break;
                case StatsModifier.NitroCooldown: switch (howStatsModify)
                    {
                        case HowStatsModify.Subtract: player.nitroRegen = carAbilities.baseNitroCooldown + amount[level]; break;
                        case HowStatsModify.Add: player.nitroRegen = carAbilities.baseNitroCooldown - amount[level]; break;
                        case HowStatsModify.Multiply: player.nitroRegen = carAbilities.baseNitroCooldown * amount[level]; break;
                        default: throw new ArgumentOutOfRangeException();
                    } break;
                case StatsModifier.ShotgunDamage: switch (howStatsModify)
                    {
                        case HowStatsModify.Subtract: player.shotgunDamages = Mathf.FloorToInt(carAbilities.baseShotgunDamage + amount[level]); break;
                        case HowStatsModify.Add: player.shotgunDamages = Mathf.FloorToInt(carAbilities.baseShotgunDamage - amount[level]); break;
                        case HowStatsModify.Multiply: player.shotgunDamages = Mathf.FloorToInt(carAbilities.baseShotgunDamage * amount[level]); break;
                        default: throw new ArgumentOutOfRangeException();
                    } break;
                case StatsModifier.CollisionDamage: switch (howStatsModify)
                    {
                        case HowStatsModify.Subtract: carAbilities.damageOnCollisionWithEnemy = Mathf.FloorToInt(carAbilities.damageOnCollisionWithEnemy + amount[level]); break;
                        case HowStatsModify.Add: carAbilities.damageOnCollisionWithEnemy = Mathf.FloorToInt(carAbilities.damageOnCollisionWithEnemy - amount[level]); break;
                        case HowStatsModify.Multiply: carAbilities.damageOnCollisionWithEnemy = Mathf.FloorToInt(carAbilities.damageOnCollisionWithEnemy * amount[level]); break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    } break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
        #endregion

        private async void SetEffectInCooldown()
        {
            if (!isCapacityCooldown) return;
            isInCooldown = true;
            await Task.Delay(Mathf.FloorToInt(cooldownDuration * 1000));
            isInCooldown = false;
        }
    }
}

public enum AbilityTrigger
{
    OnEnemyCollision,
    OnWallCollision,
    OnEnterState,
    OnExitState,
    OnUpdateState,
    OnEnemyDamageDealt,
    OnPlayerDamageDealt
}

public enum TargetAbility
{
    HitEnemy,  
    Player,
    ZoneAroundHitEnemy,
    ZoneAroundPlayer,
    ClosestEnemyToPlayer
}

public enum When
{
    Immediate,
    Delayed,
    OnTargetDie,
    EveryXSeconds
}

public enum State
{
    All,
    Default,
    Drift,
    Straff,
    Pill,
    Nitro
}

public enum Effect
{
    Explosion,
    Spear,
    Damage,
    ForceBreak,
    SpawnMine,
    LifeSteal,
    Scorch,
    Berserk,
    Shield,
    Information,
    Target
}

public enum AbilitiesStats
{
    EffectDamage,
    EffectSizeRadius,
    EffectDuration,
    EffectDelayMilliseconds,
    EffectRepeatDelay,
}

public enum AbilityType
{
    ClassicAbilites,
    UpgrateStatsAbilites,
    GoldGiver
}

public enum StatsModifier
{
    SpeedOnRoad,
    SpeedOnSand,
    NitroSpeed,
    NitroCooldown,
    ShotgunDamage,
    CollisionDamage
}

public enum HowStatsModify
{
    Subtract,
    Add,
    Multiply
}
[Serializable]
public struct ActiveModifier
{
    public AbilitiesStats stat;
    public float newValue;
}