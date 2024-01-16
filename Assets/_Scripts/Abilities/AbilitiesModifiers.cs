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

    public class AbilitiesModifiers : MonoBehaviour
    {
        [CreateAssetMenu(menuName = "Active Item Modifier", fileName = "New ActiveItemModifier")]
        public class ActiveItemModifier : ScriptableObject
        {
            [field:SerializeField] public ActiveModifier[] Modifiers { get; private set; }
        }
    }

    public enum AbilitiesStats
    {
        Damage,
        Size
    }
}