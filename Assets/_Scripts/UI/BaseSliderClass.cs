using HubcapLocalisation;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

namespace HubcapInterface {
    public class BaseSliderClass : UISelectable {
        [SerializeField, Range(0, 1)] protected float fillAmount = 1f;
        [SerializeField] private bool useDisableAmount = true;
        [SerializeField, Range(0, 1)] private float disableAmount = .25f;
        [SerializeField] private float currentValue = 1;
        [SerializeField] private SliderDataSO sliderdata = null;
        [SerializeField] private SliderSide side = SliderSide.left;
        [SerializeField, TextArea] private string sliderTxt = "";

        [Header("Slider Images")] [SerializeField]
        private Image sliderColorImage = null;

        [SerializeField] private Image sliderBackgroundImage = null;
        [SerializeField] private Image sliderShadowImage = null;
        [SerializeField] private SliderSprites baseSliderData = new();
        [SerializeField] private SliderSprites disableSliderData = new();

        [Header("Shadow Data")] 
        [SerializeField] private float shadowDistance = 1f;

        [Header("Slider side data")] 
        [SerializeField] private HorizontalLayoutGroup horLayoutFront = null;

        [SerializeField] private HorizontalLayoutGroup horLayoutBack = null;

        [Header("Text & Icons Data")]
        [SerializeField] private bool showTextWhenDisable = true;
        [SerializeField] private bool showIconBackground = true;
        [SerializeField] private Sprite iconSprite = null;
        [SerializeField] private TextMeshProUGUI textSliderFront = null;
        [SerializeField] private TextMeshProUGUI textSliderBack = null;
        [SerializeField] private Image iconSliderFront = null;
        [SerializeField] private Image iconSliderBack = null;

#if UNITY_EDITOR
        private void OnValidate() {
            if (sliderColorImage == null || sliderBackgroundImage == null || sliderShadowImage == null) return;
            UpdateFillAmount();
            UpdateSpriteImage();

            if (sliderdata == null || textSliderFront == null || textSliderBack == null || iconSliderFront == null || iconSliderBack == null) return;

            UpdateColors();
            UpdateTextData();

            if (horLayoutFront == null || horLayoutBack == null) return;
            UpdateAlignement();
        }
#endif

        protected virtual void Awake() {
            UpdateSpriteImage();
            UpdateColors();
            UpdateAlignement();
            UpdateTextData();
            UpdateFillAmount();
        }

        #region BASE VISUALS

        /// <summary>
        /// Update the fillAmount visual
        /// </summary>
        protected void UpdateFillAmount() => sliderColorImage.fillAmount = fillAmount;

        /// <summary>
        /// Update the current sprite being used based on the fillAmount
        /// </summary>
        protected void UpdateSpriteImage() {
            if (fillAmount >= disableAmount) {
                sliderColorImage.sprite = baseSliderData.colorSprite;
                sliderBackgroundImage.sprite = baseSliderData.backgroundSprite;
                sliderShadowImage.sprite = baseSliderData.shadowSprite;
            }
            else if (!useDisableAmount) {
                sliderColorImage.sprite = baseSliderData.colorSprite;
                sliderBackgroundImage.sprite = baseSliderData.backgroundSprite;
                sliderShadowImage.sprite = baseSliderData.shadowSprite;
            }
            else {
                sliderColorImage.sprite = disableSliderData.colorSprite;
                sliderBackgroundImage.sprite = disableSliderData.backgroundSprite;
                sliderShadowImage.sprite = disableSliderData.shadowSprite;
            }

            sliderShadowImage.rectTransform.localPosition = new Vector3(shadowDistance, -shadowDistance, 0);
            sliderShadowImage.rectTransform.anchoredPosition = new Vector2(shadowDistance, -shadowDistance);
            sliderShadowImage.rectTransform.sizeDelta = Vector2.zero;
            
            iconSliderBack.enabled = showIconBackground;
            
            if (iconSprite != null) {
                iconSliderFront.gameObject.SetActive(true);
                iconSliderBack.gameObject.SetActive(true);
                iconSliderFront.sprite = iconSprite;
                iconSliderBack.sprite = iconSprite;
            }
            else {
                iconSliderFront.gameObject.SetActive(false);
                iconSliderBack.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Update the colors being used by the slider
        /// </summary>
        private void UpdateColors() {
            sliderShadowImage.color = sliderdata.ShadowColor;
            textSliderFront.color = IsSliderEnable() || !useDisableAmount ? sliderdata.TextColorFront : sliderdata.TextColorFrontDisable;
            textSliderBack.color = sliderdata.TextColorBack;
            iconSliderFront.color = IsSliderEnable() || !useDisableAmount ? sliderdata.TextColorFront : sliderdata.TextColorFrontDisable;;
            iconSliderBack.color = sliderdata.TextColorBack;
        }

        /// <summary>
        /// Update on which side text and icon need to align
        /// </summary>
        private void UpdateAlignement() {
            horLayoutFront.childAlignment = side switch {
                SliderSide.left => TextAnchor.MiddleLeft,
                SliderSide.right => TextAnchor.MiddleRight,
                _ => TextAnchor.MiddleCenter
            };
            horLayoutBack.childAlignment = side switch {
                SliderSide.left => TextAnchor.MiddleLeft,
                SliderSide.right => TextAnchor.MiddleRight,
                _ => TextAnchor.MiddleCenter
            };

            horLayoutFront.reverseArrangement = (side != SliderSide.left);
            horLayoutBack.reverseArrangement = (side != SliderSide.left);
            
            textSliderFront.horizontalAlignment = side switch {
                SliderSide.left => HorizontalAlignmentOptions.Left,
                SliderSide.right => HorizontalAlignmentOptions.Right,
                _ => HorizontalAlignmentOptions.Center
            };
            textSliderFront.verticalAlignment = VerticalAlignmentOptions.Middle;
            textSliderBack.horizontalAlignment = side switch {
                SliderSide.left => HorizontalAlignmentOptions.Left,
                SliderSide.right => HorizontalAlignmentOptions.Right,
                _ => HorizontalAlignmentOptions.Center
            };
            textSliderBack.verticalAlignment = VerticalAlignmentOptions.Middle;
        }

        #endregion BASE VISUALS

        #region SLIDER TEXT

        /// <summary>
        /// Update the text input of both texts
        /// </summary>
        public void UpdateTextData() {
            textSliderFront.text = GetCurrentSliderString();
            textSliderBack.text = GetCurrentSliderString();

            if (showTextWhenDisable) return;
            textSliderBack.enabled = IsSliderEnable();
        }

        private string GetCurrentSliderString() {
            string finalString = "";
            string[] textSplit = sliderTxt.Split('{', '}');
            foreach (string txt in textSplit) {
                if (txt.Contains("CurrentValue")) finalString += currentValue;
                else if (txt.Contains("Translate")) {
                    if (Application.isPlaying) finalString += LocalizationManager.Localize(txt.Split(":")[1].Replace("}", ""));
                    else finalString += txt.Split(':')[1].Split('.')[1];
                }
                else finalString += txt;
            }

            return finalString;
        }

        #endregion SLIDER TEXT

        /// <summary>
        /// Update the current value of fillAmount for this slider
        /// </summary>
        /// <param name="amount"></param>
        public virtual void UpdateSliderAmount(float amount) {}

        /// <summary>
        /// Update the value which status on the availability of this slider
        /// </summary>
        /// <param name="amount"></param>
        public void UpdateDisableAmount(float amount) => disableAmount = amount;

        private bool IsSliderEnable() => fillAmount > disableAmount;
    }
    
    [Serializable]
    public class SliderSprites {
        public Sprite colorSprite = null;
        public Sprite backgroundSprite = null;
        public Sprite shadowSprite = null;
    }

    public enum SliderSide {
        left,
        right,
        Center
    }
}