using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Abilities
{
    [CreateAssetMenu(menuName = "Ability", fileName = "New Ability")]
    public class AbilitiesSO : ScriptableObject
    {
        [Header("Informations")] public string abilityName;
        [TextArea(3, 3)] public string description;
        public Sprite abilitySprite;
        public int level;

        [Space(5)] [Header("Abilities Creation")]
        public AbilityTrigger trigger;
        public TargetAbility target;
        public When when;
        public State state;

        [Space(5)] [Header("Abilities Level Modifiers")]
        public AbilitiesModifiers[] levelsAbilitiesModifiers;
        
        // Stats
        public int effectDamage;
        public int effectSizeRadius;

        public void Activate()
        {
            switch (trigger)
            {
                case AbilityTrigger.OnEnemyCollision: /* CarAbilitiesManager.instance.OnEnemyCollision += ApplyWhenModifiers */; break;
                case AbilityTrigger.OnWallCollision: break;
                case AbilityTrigger.OnEnterState: break;
                case AbilityTrigger.OnExitState: break;
                case AbilityTrigger.OnUpdateState: break;
                case AbilityTrigger.OnEnemyDamageDealt: break;
                case AbilityTrigger.OnPlayerDamageDealt: break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public void ApplyWhenModifiers()
        {
            switch (state)
            {
                case State.All: break;
                case State.Default: if(!CarController.instance.isDefault); return;
                case State.Drift: if (!CarController.instance.driftBrake) return; break;
                case State.Straff: if(!CarController.instance.isStraffing) return; break;
                case State.Pill: if(!CarController.instance.brakeMethodApplied) return; break;
                case State.Nitro: if(!CarController.instance.nitroMode) return; break;
                default: throw new ArgumentOutOfRangeException(); break;
            }

            Debug.Log($"State: {state} Passed!");
            
            switch (when)
            {
                case When.Immediate: break;
                case When.Delayed: break;
                case When.OnTargetDie: break;
                case When.EveryXSeconds: break;
                default: throw new ArgumentOutOfRangeException(nameof(when), when, null);
            }
        }

        public void ApplyEffectOnTarget()
        {
            switch (target)
            {
                case TargetAbility.Enemy: break;
                case TargetAbility.Player: break;
                case TargetAbility.ZoneAroundEnemy: break;
                case TargetAbility.ZoneAroundPlayer: break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private void ApplyEffect(Effect eft)
        {
            
        }
        
        private async void DelayEffect(Effect eft, int delayInMS)
        {
            await Task.Delay(delayInMS);
            ApplyEffect(eft);
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
    Enemy,  
    Player,
    ZoneAroundEnemy,
    ZoneAroundPlayer,
    ClosestEnemy
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
    Undefined_1,
    Undefined_2,
    Undefined_3,
    Undefined_4,
    Undefined_5
}