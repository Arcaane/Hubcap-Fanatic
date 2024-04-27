using System;
using UnityEngine;

namespace Abilities
{
    
    [CreateAssetMenu(menuName = "AbilityModifier", fileName = "New AbilityModifier")]
    public class AbilitiesModifiers : ScriptableObject
    {
        [field:SerializeField] public ActiveModifier[] Modifiers { get; private set; }
    }
}