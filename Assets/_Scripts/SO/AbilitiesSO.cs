using System;
using System.Threading.Tasks;
using DG.Tweening;
using ManagerNameSpace;
using UnityEngine;
using UnityEngine.Serialization;

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
        public OverheatEffect overheatEffect;

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
        
        private float _effectDamage;
        private float _effectSizeRadius;
        private float _effectDuration;
        private int _effectDelayMilliseconds;
        private float _effectRepeatDelay;

        public float mightPowerUpLevel;
        private float abilityDamage => _effectDamage *= mightPowerUpLevel;

        [Space(4)]
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
        private bool isInCooldown = false;
        
        public bool isOverheat;
        public bool isOverheatable;
        
        [Space(5)]
        [Header("Layer Masks")]
        public LayerMask enemyLayerMask;
        
        public void Initialize()
        {
            isOverheat = false;
            player = CarController.instance;
            carAbilities = CarAbilitiesManager.instance;
            isInCooldown = false;

            mightPowerUpLevel = carAbilities.powerUpMight;
            
            _effectDamage = effectDamage;
            _effectSizeRadius = effectSizeRadius;
            _effectDuration = effectDuration;
            _effectDelayMilliseconds = effectDelayMilliseconds;
            _effectRepeatDelay = effectRepeatDelay;

            if (type == AbilityType.ClassicAbilites)
            {
                switch (trigger)
                {
                    case AbilityTrigger.OnEnemyCollision:  carAbilities.OnEnemyCollision += Activate ; break;
                    case AbilityTrigger.OnWallCollision: carAbilities.OnWallCollision += Activate ; break;
                    case AbilityTrigger.OnUpdateState: carAbilities.OnUpdate += Activate; break;
                    case AbilityTrigger.OnEnemyDamageDealt: carAbilities.OnEnemyDamageTaken += Activate ; break;
                    case AbilityTrigger.OnPlayerDamageDealt: carAbilities.OnPlayerDamageTaken += Activate ; break;
                    case AbilityTrigger.OnPill: carAbilities.OnPill += Activate ; break;
                    case AbilityTrigger.OnBeginDrift: carAbilities.OnBeginDrift += Activate ; break;
                    case AbilityTrigger.OnBeginNitro: carAbilities.OnBeginNitro += Activate ; break;
                    case AbilityTrigger.OnEnemyHitWithShotgun: carAbilities.OnEnemyHitWithShotgun += Activate ; break;
                    case AbilityTrigger.OnShotGunUsed: carAbilities.OnShotgunUsed += Activate ; break;
                    case AbilityTrigger.OnEnemyKilled: carAbilities.OnEnemyKilled += Activate ; break;
                    case AbilityTrigger.OnShotGunUsedWithoutTarget: carAbilities.OnShotgunUsedWithoutTarget += Activate ; break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
            else if (type == AbilityType.UpgrateStatsAbilites)
            {
                LevelUpStatsAbility();
            }
            else if (type == AbilityType.GoldGiver)
            {
                carAbilities.GoldAmountWonOnRun += (int)amount[0];
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
            returnedCollision = collision;
            ApplyWhenModifiers();
        }
        
        public void ApplyWhenModifiers()
        {
            if (isInCooldown) { return; }
            
            switch (state)
            {
                case State.All: break;
                case State.Default: if(player.driftBrake || player.nitroMode) return; break;
                case State.Drift: if (!player.driftBrake) return; break;
                case State.Nitro: if(!player.nitroMode) return; break;
                default: throw new ArgumentOutOfRangeException(); break;
            }

            if (isOverheat)
            {
                switch (overheatEffect)
                {
                    case OverheatEffect.MultipleMines:
                        for (int i = 0; i < 5; i++)
                        {
                            float angle = Mathf.PI * 2 / 5 * i;
                            float x = Mathf.Cos(angle) * 10;
                            float z = Mathf.Sin(angle) * 10;
                            EffectSpawnMine(player.transform.position + new Vector3(x,0,z));
                        }
                        break;
                    case OverheatEffect.Effect_2: break;
                    case OverheatEffect.Effect_3: break;
                    default: throw new ArgumentOutOfRangeException();
                }
                
                return;
            }
            
            switch (target)
            {
                case TargetAbility.HitEnemy: ApplyEffectOnTarget(returnedTargetObject); break;
                case TargetAbility.Player: ApplyEffectOnTarget(player.gameObject); break;
                case TargetAbility.ZoneAroundHitEnemy: ApplyEffectOnTargetsInZone(returnedTargetObject.transform.position,_effectSizeRadius); break;
                case TargetAbility.ZoneAroundPlayer: ApplyEffectOnTargetsInZone(player.transform.position, _effectSizeRadius); break;
                case TargetAbility.ClosestEnemyToPlayer: ApplyEffectOnClosestTarget(player.transform.position, _effectSizeRadius); break;
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
                case When.Delayed: DelayEffect(targetObj,_effectDelayMilliseconds);
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
                case Effect.SpawnMine: EffectSpawnMine(targetObj.transform.position); break;
                case Effect.LifeSteal: EffectLifeSteal(targetObj); break;
                case Effect.Scorch: EffectScorch(targetObj); break;
                case Effect.Berserk: EffectBerserk(targetObj); break;
                case Effect.Shield: EffectShield(targetObj); break;
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
                effectRepeatTimer = _effectRepeatDelay;
                ApplyEffect(targetObj);
            }
        }

        #endregion
        
        #region EffectFunctions

        private void EffectSpear(GameObject targetObj)
        {
            if (targetObj == null) return;
            var carPos = player.transform.position;
            Vector3 relativePos = carPos - targetObj.transform.position;
            relativePos = new Vector3(relativePos.x, 0, relativePos.z).normalized;
            Transform obj = Pooler.instance.SpawnInstance(Key.OBJ_Spear, carPos, Quaternion.LookRotation(-relativePos)) as Transform;
            if (obj.gameObject != null) obj.GetComponent<SpearObject>().damages = Mathf.FloorToInt(abilityDamage);
        }
    
        private void EffectDamage(GameObject targetObj)
        {
            IDamageable damageable = targetObj.GetComponent<IDamageable>();
            damageable.TakeDamage(Mathf.FloorToInt(abilityDamage));
        }
    
        private void EffectForceBreak(GameObject targetObj)
        {
            CarBehaviour carBehaviour = targetObj.GetComponent<CarBehaviour>();
            if (carBehaviour == null) return;
            carBehaviour.forceBreak = true;
            carBehaviour.forceBreakTimer = _effectDuration;
            GameObject go = Pooler.instance.SpawnTemporaryInstance(Key.FX_MotorBreak, targetObj.transform.position + new Vector3(0,0.5f,0), Quaternion.identity, effectDuration).gameObject;
            go.transform.SetParent(targetObj.transform);
            go.SetActive(true);
        }

        private void EffectExplosion(GameObject targetObj)
        {
            var position = targetObj.transform.position;
            
            float dist = Vector3.Distance(position, CarController.instance.transform.position);
            if (dist < 30)
            {
                CameraShake.instance.SetShake(0.4f *( 1- dist/30));
            }
            
            GameObject gameObject = Pooler.instance.SpawnTemporaryInstance(Key.FX_Explosion, position, Quaternion.identity, 5).gameObject;
            gameObject.transform.localScale = new Vector3(_effectSizeRadius, _effectSizeRadius,_effectSizeRadius);
            
            var cols = Physics.OverlapSphere(position, _effectSizeRadius, enemyLayerMask);
            foreach (var t in cols)
            {
                t.GetComponent<IDamageable>()?.TakeDamage(Mathf.FloorToInt(abilityDamage));
            }
        }

        private void EffectSpawnMine(Vector3 targetObj)
        {
            Mine mine = Pooler.instance.SpawnInstance(Key.OBJ_Mine, targetObj, Quaternion.identity).GetComponent<Mine>();
            mine.damages = Mathf.FloorToInt(abilityDamage);
            mine.explosionRadius = _effectSizeRadius;
        }

        private void EffectLifeSteal(GameObject targetObj)
        {
            CarHealthManager.instance.TakeHeal(Mathf.FloorToInt(abilityDamage));
            GameObject go = Pooler.instance.SpawnTemporaryInstance(Key.FX_PlayerGiveLife, targetObj.transform.position + new Vector3(0,0.5f,0), Quaternion.identity, 1.5f).gameObject;
            go.transform.SetParent(targetObj.transform);
            go.SetActive(true);
        }
        
        private async void EffectScorch(GameObject targetObj)
        {
            CarBehaviour carBehaviour = targetObj.GetComponent<CarBehaviour>();
            if (carBehaviour == null || carBehaviour.isScorch) return;
            carBehaviour.isScorch = true;
            
            var a = Mathf.FloorToInt(_effectDuration / ((float)_effectDelayMilliseconds / 1000));
            for (int i = 0; i < a; i++)
            {
                if (!targetObj.activeSelf) return;
                targetObj.GetComponent<IDamageable>()?.TakeDamage(Mathf.FloorToInt(abilityDamage));
                await Task.Delay(_effectDelayMilliseconds);
            }

            if (carBehaviour == null) return;
            carBehaviour.isScorch = false;
        }

        private async void EffectBerserk(GameObject targetObj)
        {
            if (!player) return;
            player.shootDuration = _effectDamage;
            player.isBerserk = true;
            
            await Task.Delay(Mathf.FloorToInt(_effectDuration * 1000));
            if (!player) return;
            
            player.shootDuration = carAbilities.baseShotgunDuration;
            player.isBerserk = false;
        }
        
        private async void EffectShield(GameObject targetObj)
        {
            CarController car = targetObj.GetComponent<CarController>();
            if (!car) return;
            car.shield.SetActive(true);
            car.isShield = true;
            await Task.Delay(Mathf.FloorToInt(_effectDuration * 1000));
            if (!car) return;
            car.shield.SetActive(false);
            car.isShield = false;
        }

        private async void EffectTarget(GameObject obj)
        {
            var a = obj.GetComponent<CarBehaviour>();
            if (!a) return;
            a.isAimEffect = true;
            await Task.Delay(Mathf.FloorToInt(_effectDuration * 1000));
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
                    case AbilitiesStats.EffectDamage: _effectDamage = (int)modifiers[i].newValue; break;
                    case AbilitiesStats.EffectSizeRadius: _effectSizeRadius = modifiers[i].newValue;  break;
                    case AbilitiesStats.EffectDuration: _effectDuration = modifiers[i].newValue; break;
                    case AbilitiesStats.EffectDelayMilliseconds: _effectDelayMilliseconds = (int)modifiers[i].newValue; break;
                    case AbilitiesStats.EffectRepeatDelay: _effectRepeatDelay = modifiers[i].newValue; break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void LevelUpStatsAbility()
        {
            switch (statsModifier)
            {
                case StatsModifier.SpeedOnRoad:
                    player.maxSpeed = howStatsModify switch
                    {
                        HowStatsModify.Subtract => carAbilities.baseSpeedOnRoad - amount[level],
                        HowStatsModify.Add => 40 + amount[level],
                        HowStatsModify.Multiply => carAbilities.baseSpeedOnRoad * amount[level],
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;
                case StatsModifier.SpeedOnSand:
                    player.offRoadSpeed = howStatsModify switch
                    {
                        HowStatsModify.Subtract => carAbilities.baseSpeedOnSand - amount[level],
                        HowStatsModify.Add => 22 + amount[level],
                        HowStatsModify.Multiply => carAbilities.baseSpeedOnSand * amount[level],
                        _ => player.offRoadSpeed
                    };
                    break;
                case StatsModifier.NitroSpeed: player.nitroSpeed = howStatsModify switch
                    {
                        HowStatsModify.Subtract => carAbilities.baseNitroSpeed - amount[level],
                        HowStatsModify.Add => 55 + amount[level],
                        HowStatsModify.Multiply => carAbilities.baseNitroSpeed * amount[level],
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;
                case StatsModifier.NitroCooldown:
                    player.nitroRegen = howStatsModify switch
                    {
                        HowStatsModify.Subtract => carAbilities.baseNitroCooldown - amount[level],
                        HowStatsModify.Add => carAbilities.baseNitroCooldown - amount[level],
                        HowStatsModify.Multiply => carAbilities.baseNitroCooldown * amount[level],
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;
                case StatsModifier.ShotgunDamage:
                    player.shotgunDamages = howStatsModify switch
                    {
                        HowStatsModify.Subtract => Mathf.FloorToInt(carAbilities.baseShotgunDamage - amount[level]),
                        HowStatsModify.Add => (int)(carAbilities.baseShotgunDamage + amount[level]),
                        HowStatsModify.Multiply => Mathf.FloorToInt(carAbilities.baseShotgunDamage * amount[level]),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;
                case StatsModifier.CollisionDamage:
                    carAbilities.damageOnCollisionWithEnemy = howStatsModify switch
                    {
                        HowStatsModify.Subtract => Mathf.FloorToInt(carAbilities.damageOnCollisionWithEnemy - amount[level]),
                        HowStatsModify.Add => (int)(carAbilities.damageOnCollisionWithEnemy + amount[level]),
                        HowStatsModify.Multiply => Mathf.FloorToInt(carAbilities.damageOnCollisionWithEnemy * amount[level]),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;
                case StatsModifier.CriticalHit :
                {
                    player.vaynePassiveMultiplier = howStatsModify switch
                    {
                        HowStatsModify.Subtract => Mathf.FloorToInt(carAbilities.baseCritDamage - amount[level]),
                        HowStatsModify.Add => 1 + amount[level],
                        HowStatsModify.Multiply => Mathf.FloorToInt(carAbilities.baseCritDamage * amount[level]),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    player.gotVayneUpgrade = true;
                    player.shotBeforeCritAmount = 2;
                    break;
                }
                case StatsModifier.Armor:
                {
                    CarHealthManager.instance.armorInPercent = howStatsModify switch
                    {
                        HowStatsModify.Subtract => Mathf.FloorToInt(carAbilities.baseArmorPercent - amount[level]),
                        HowStatsModify.Add => carAbilities.baseArmorPercent + amount[level],
                        HowStatsModify.Multiply => Mathf.FloorToInt(carAbilities.baseArmorPercent * amount[level]),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                } break;
                case StatsModifier.MaxHealth:
                {
                    CarHealthManager.instance.maxLifePoints = howStatsModify switch
                    {
                        HowStatsModify.Subtract => Mathf.FloorToInt(carAbilities.baseMaxHealth - amount[level]),
                        HowStatsModify.Add => (int)(carAbilities.baseMaxHealth + amount[level]),
                        HowStatsModify.Multiply => Mathf.FloorToInt(carAbilities.baseMaxHealth * amount[level]),
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    CarHealthManager.instance.TakeHeal(level == 0 ? Mathf.FloorToInt(amount[level]) : Mathf.FloorToInt(amount[level] - amount[level - 1]));
                } break;
                case StatsModifier.HitBeforeDeliverDrop:
                {
                    player.CollsionBeforeDropDeliver = howStatsModify switch
                    {
                        HowStatsModify.Subtract => Mathf.FloorToInt(carAbilities.baseHitBeforeDeliverDrop - amount[level]),
                        HowStatsModify.Add => (int)(carAbilities.baseHitBeforeDeliverDrop + amount[level]),
                        HowStatsModify.Multiply => Mathf.FloorToInt(carAbilities.baseHitBeforeDeliverDrop * amount[level]),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                } break;
                case StatsModifier.OverallAbilitiesCooldown:
                {
                    carAbilities.overallAbilitiesCooldown = howStatsModify switch
                    {
                        HowStatsModify.Subtract => Mathf.FloorToInt(carAbilities.baseOverallAbilitiesCooldown - amount[level]),
                        HowStatsModify.Add => carAbilities.baseOverallAbilitiesCooldown + amount[level],
                        HowStatsModify.Multiply => Mathf.FloorToInt(carAbilities.baseOverallAbilitiesCooldown * amount[level]),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                } break;
                case StatsModifier.AttackSpeCooldown:
                {
                    player.shootDuration = howStatsModify switch
                    {
                        HowStatsModify.Subtract => Mathf.FloorToInt(carAbilities.baseAttackCooldown - amount[level]),
                        HowStatsModify.Add => carAbilities.baseAttackCooldown + amount[level],
                        HowStatsModify.Multiply => Mathf.FloorToInt(carAbilities.baseAttackCooldown * amount[level]),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                } break;
                case StatsModifier.BouncePowerOnDrop:
                {
                    CarController.instance.speedRetained = howStatsModify switch
                    {
                        HowStatsModify.Subtract => Mathf.FloorToInt(carAbilities.baseSpeedRetainedOnBounce - amount[level]),
                        HowStatsModify.Add => 0.3f + amount[level],
                        HowStatsModify.Multiply => Mathf.FloorToInt(carAbilities.baseSpeedRetainedOnBounce * amount[level]),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                } break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
        #endregion

        public int index = -1;
        private void SetEffectInCooldown()
        {
            if (!isCapacityCooldown) return;
            isInCooldown = true;
            carAbilities.LaunchCo(cooldownDuration * (1 - carAbilities.overallAbilitiesCooldown/100), index);
            UIManager.instance.abilitiesSlots[index].abilityCooldownSlider.gameObject.SetActive(true);
            UIManager.instance.abilitiesSlots[index].abilityCooldownSlider.fillAmount = 1;
            UIManager.instance.abilitiesSlots[index].abilityCooldownSlider.DOFillAmount(0, cooldownDuration * (1 - carAbilities.overallAbilitiesCooldown/100)).OnComplete(
                delegate
                {
                    isInCooldown = false;
                    UIManager.instance.abilitiesSlots[index].abilityCooldownSlider.gameObject.SetActive(false);
                });
        }
    }
}

public enum AbilityTrigger
{
    OnEnemyCollision,
    OnEnemyHitWithShotgun,
    OnShotGunUsed,
    OnWallCollision,
    OnBeginNitro,
    OnBeginDrift,
    OnPill,
    OnUpdateState,
    OnEnemyDamageDealt,
    OnPlayerDamageDealt,
    OnEnemyKilled,
    OnShotGunUsedWithoutTarget
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
    CollisionDamage,
    CriticalHit,
    MaxHealth,
    AttackSpeCooldown,
    OverallAbilitiesCooldown,
    Armor,
    HitBeforeDeliverDrop,
    BouncePowerOnDrop
}

public enum HowStatsModify
{
    Subtract,
    Add,
    Multiply
}

public enum OverheatEffect
{
    MultipleMines,
    Effect_2,
    Effect_3
}


[Serializable]
public struct ActiveModifier
{
    public AbilitiesStats stat;
    public float newValue;
}