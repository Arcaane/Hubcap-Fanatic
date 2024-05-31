using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HubcapInterface {
    public class SliderManager : BaseSliderClass {
        [Header("SLIDER EFFECT DATA")] 
        [SerializeField] private float effectDuration = 0.5f;
        [SerializeField] private float effectDelayedDuration = 0.5f;
        [Space] 
        [SerializeField] private bool useDelayedEffect = false;
        [SerializeField] private float backgroundEffectDelay = 1;
        [SerializeField] private Image sliderColorDelayed = null;

        [Header("EFFECT EVENTS")] 
        [SerializeField] private UnityEvent startEffect = new();   
        [SerializeField] private UnityEvent stopEffect = new();   
        
        
        protected override void UpdateFillAmount() {
            if (Application.isPlaying && effectDuration != 0f) {
                sliderColorImage.DOKill();
                sliderColorImage.DOFillAmount(fillAmount, effectDuration);
                
                if (useDelayedEffect) {
                    sliderColorDelayed.DOKill();
                    sliderColorDelayed.DOFillAmount(fillAmount, effectDelayedDuration).SetDelay(backgroundEffectDelay);
                }
                
                return;
            }

            sliderColorImage.fillAmount = fillAmount;
            if(useDelayedEffect) sliderColorDelayed.fillAmount = fillAmount;
        }

        #region BASE VISUALS
        
        protected override void UpdateAlignement() {
            base.UpdateAlignement();
            if(useDelayedEffect) sliderColorDelayed.fillOrigin = side == SliderSide.right ? 1 : 0;
        }

        protected override void UpdateSpriteImage() {
            base.UpdateSpriteImage();
            sliderColorDelayed.gameObject.SetActive(useDelayedEffect);
        }
        
        #endregion BASE VISUALS

        public override void UpdateSliderAmount(float amount) {
            fillAmount = amount;
            UpdateFillAmount();
            UpdateSpriteImage();
            UpdateTextData();
        }

        public void UpdateCurrentvalue(int newAmount) {
            currentValue = newAmount;
            UpdateTextData();
        }

        #region EFFECTS

        public void StartEffects() => startEffect.Invoke();
        public void StopEffects() => stopEffect.Invoke();

        #endregion EFFECTS

    }
}