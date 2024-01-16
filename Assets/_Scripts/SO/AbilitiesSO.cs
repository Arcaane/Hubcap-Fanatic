using System;
using System.Threading.Tasks;
using ManagerNameSpace;
using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu(menuName = "Ability", fileName = "New Ability")]
    public class AbilitiesSO : ScriptableObject
    {
        [Header("Informations")] public string abilityName;
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
        public AbilitiesModifiers[] levelsAbilitiesModifiers;
        
        // Stats
        public int effectDamage;
        public int effectSizeRadius;
        public float effectDuration;
        public int effectDelayMilliseconds;
        public float effectRepeatDelay;
        
        // Target Objects
        private GameObject returnedTargetObject;
        private Collision returnedCollision;
        
        // Memory Vars
        private Collider[] cols; 
        private float effectRepeatTimer;
        private CarController player;
        private CarAbilitiesManager carAbilities;
        
        [Space(5)] [Header("Layer Masks")]
        public LayerMask enemyLayerMask;
        
        public void Initialize()
        {
            player = CarController.instance;
            carAbilities = CarAbilitiesManager.instance;
            
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

            Debug.Log($"State: {state} Passed!");
            
            
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
                case When.OnTargetDie: Debug.LogError("ON TARGET DIE PAS FAIT");
                    break;
                case When.EveryXSeconds: RepeatedEffect(targetObj);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(when), when, null);
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
                case Effect.Explosion: Debug.LogError("PAS ENCORE D'EXPLOSION");
                    break;
                case Effect.Spear: EffectSpear(targetObj);
                    break;
                case Effect.Damage: EffectDamage(targetObj);
                    break;
                case Effect.ForceBreak: EffectForceBreak(targetObj);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(effect), effect, null);
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
        }
        #endregion


        #region Stats

        public void LevelUpStats()
        {
            if (levelsAbilitiesModifiers[level] == null) return;
            
            for (int i = 0; i < levelsAbilitiesModifiers[level].Modifiers.Length; i++)
            {
                switch (levelsAbilitiesModifiers[level].Modifiers[i].stat)
                {
                    case AbilitiesStats.EffectDamage: effectDamage += (int)levelsAbilitiesModifiers[level].Modifiers[i].newValue; break;
                    case AbilitiesStats.EffectSizeRadius: effectSizeRadius += (int)levelsAbilitiesModifiers[level].Modifiers[i].newValue;  break;
                    case AbilitiesStats.EffectDuration: effectDuration += levelsAbilitiesModifiers[level].Modifiers[i].newValue; break;
                    case AbilitiesStats.EffectDelayMilliseconds: effectDelayMilliseconds += (int)levelsAbilitiesModifiers[level].Modifiers[i].newValue; break;
                    case AbilitiesStats.EffectRepeatDelay: effectRepeatDelay += levelsAbilitiesModifiers[level].Modifiers[i].newValue; break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        #endregion
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
    Undefined_4,
    Undefined_5
}

public enum AbilitiesStats
{
    EffectDamage,
    EffectSizeRadius,
    EffectDuration,
    EffectDelayMilliseconds,
    EffectRepeatDelay,
}