using UnityEngine;

namespace HubcapInterface {
    [CreateAssetMenu(menuName = "UI/Slider Data")]
    public class SliderDataSO : ScriptableObject {
        [SerializeField] private Color textColorFront = new();
        [SerializeField] private Color textColorFrontDisable = new();
        [SerializeField] private Color textColorBack = new();
        [SerializeField] private Color shadowColor = new();

        public Color TextColorFront => textColorFront;
        public Color TextColorFrontDisable => textColorFrontDisable;
        public Color TextColorBack => textColorBack;
        public Color ShadowColor => shadowColor;
    }
}