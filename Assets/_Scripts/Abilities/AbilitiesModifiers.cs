using System;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    public struct ActiveModifier
    {
        public AbilitiesStats stat;
        public float newValue;
    }
    
    [CreateAssetMenu(menuName = "AbilityModifier", fileName = "New AbilityModifier")]
    public class AbilitiesModifiers : ScriptableObject
    {
        [field:SerializeField] public ActiveModifier[] Modifiers { get; private set; }
    }
    
}