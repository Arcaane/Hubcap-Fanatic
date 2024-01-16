using UnityEngine;

namespace Abilities
{
    public class AbilitiesSO : ScriptableObject
    {
        public string abilityName;
        [field: SerializeField,TextArea(10,10)] public string description;
        public Sprite abilitySprite;
    }
}