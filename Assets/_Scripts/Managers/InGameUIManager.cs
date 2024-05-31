using System.Collections.Generic;
using DG.Tweening;
using Helper;
using HubcapInterface;
using UnityEngine;

namespace HubcapManager {
    [RequireComponent(typeof(Canvas))]
    public class InGameUIManager : Singleton<InGameUIManager> {
        private RectTransform canvasTransform = null;
        public RectTransform CanvasTransform => canvasTransform;
        
        [Header("SLIDERS DATA")] 
        [SerializeField] private SliderManager lifeSlider = null;
        [SerializeField] private SliderManager nitroSlider = null;
        [SerializeField] private SliderManager levelSlider = null;
        [SerializeField] private SliderManager waveSlider = null;
        [SerializeField] private List<SliderManager> shotgunSliders = new();

        [Header("SHOOT INFORMATION")] 
        [SerializeField] public Transform shootIcon = null;

        protected override void Awake() {
            base.Awake();
            canvasTransform = GetComponent<RectTransform>();
        }

        #region SLIDERS

        public void UpdateLifeSlider(float amount, int value) {
            lifeSlider.StartEffects();
            lifeSlider.UpdateSliderAmount(amount);
            lifeSlider.UpdateCurrentvalue(value);
        }

        public void UpdateLevelSlider(float amount, int value) {
            levelSlider.UpdateSliderAmount(amount);
            levelSlider.UpdateCurrentvalue(value);
        }
        
        public void UpdateWaveSlider(float amount, int value) {
            waveSlider.UpdateSliderAmount(amount);
            waveSlider.UpdateCurrentvalue(value);
        }

        public void UpdateShotgunSlider(float amount, int id) => shotgunSliders[id].UpdateSliderAmount(amount);

        //Init MinAmount Nitro Slider
        public void UpdateNitroSlider(float amount) => nitroSlider.UpdateSliderAmount(amount);
        public void InitNitroSlider(float minAmount) => nitroSlider.UpdateDisableAmount(minAmount);
        public void StartNitroEffect() => nitroSlider.StartEffects();
        public void StopNitroEffect() => nitroSlider.StopEffects();

        #endregion SLIDERS

        #region SHOTGUN DATA
        
        /// <summary>
        /// Update shootIcon position and scale
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="targetScale"></param>
        public void UpdateShootIcon(Vector3 targetPosition, Vector3 targetScale) {
            shootIcon.localPosition = targetPosition;
            shootIcon.DOScale(targetScale, .25f);
        }

        /// <summary>
        /// Disable the shootIcon from the screen
        /// </summary>
        public void DisableShootIcon() => shootIcon.DOScale(Vector3.zero, .25f);

        #endregion SHOTGUN DATA
    }
}