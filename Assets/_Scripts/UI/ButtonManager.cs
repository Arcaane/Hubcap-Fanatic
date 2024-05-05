using System;
using System.Collections.Generic;
using DG.Tweening;
using HubcapInterface.DoTween;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace HubcapInterface {
    public class ButtonManager : BaseSliderClass, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
        [SerializeField] private float transitionDuration = 0.25f;
        [SerializeField] private List<ButtonEvent> buttonHoverEvents = new();
        [SerializeField] private UnityEvent onClickEvents = new();
        
        /// <summary>
        /// Override the awake method set in BaseSliderClass
        /// </summary>
        protected override void Awake() {
            base.Awake();
            InitData();
        }
        
        /// <summary>
        /// Initialize Buttons data
        /// </summary>
        private void InitData() {
            foreach (ButtonEvent baseHoverEvent in buttonHoverEvents) {
                switch (baseHoverEvent.eventType) {
                    case ButtonEventType.Scale:
                        baseHoverEvent.baseSize = baseHoverEvent.parentTransform.localScale;
                        break;

                    case ButtonEventType.ColorImage:
                        baseHoverEvent.baseColor = baseHoverEvent.spriteImage.color;
                        break;

                    case ButtonEventType.ColorText:
                        baseHoverEvent.baseColor = baseHoverEvent.text.color;
                        break;

                    case ButtonEventType.SpriteSwipe:
                        baseHoverEvent.baseSprite = baseHoverEvent.spriteImage.sprite;
                        break;

                    case ButtonEventType.Alpha:
                        baseHoverEvent.baseAlpha = baseHoverEvent.canvasGroup.alpha;
                        break;

                    case ButtonEventType.FillAmount:
                        baseHoverEvent.baseAmount = baseHoverEvent.spriteImage.fillAmount;
                        break;

                    case ButtonEventType.DoTweenEffect: break;
                    case ButtonEventType.None: break;
                }
            }
        }
        
        #region HANDLE MOUSE
        
        public void OnPointerEnter(PointerEventData eventData) => CallAllButtonEvent(buttonHoverEvents);
        public void OnPointerExit(PointerEventData eventData) => CallAllButtonEvent(buttonHoverEvents, false);
        public void OnPointerClick(PointerEventData eventData) => onClickEvents.Invoke();
        
        #endregion HANDLE MOUSE
        
        
        #region HANDLE GAMEPAD
        
        public override void DisableSelection() {
            base.DisableSelection();
            CallAllButtonEvent(buttonHoverEvents, false);
        }

        public override void EnableSelection() {
            base.EnableSelection();
            CallAllButtonEvent(buttonHoverEvents);
        }

        public override void ActionInputPressed() => onClickEvents.Invoke();

        #endregion HANDLE GAMEPAD
        
        
        #region BUTTON METHODS
        
        /// <summary>
        /// Call all the buttonEvents inside a list of buttonEvents
        /// </summary>
        /// <param name="buttonEvents"></param>
        /// <param name="hover"></param>
        private void CallAllButtonEvent(List<ButtonEvent> buttonEvents, bool hover = true) {
            foreach (ButtonEvent hoverEv in buttonEvents) {
                CallHoverEvents(hoverEv, hover);
            }
        }

        /// <summary>
        /// Call HoverEvents when mouse enter GUI element
        /// </summary>
        /// <param name="hoverEv"></param>
        /// <param name="hover"></param>
        private void CallHoverEvents(ButtonEvent hoverEv, bool hover = true) {
            switch (hoverEv.eventType) {
                case ButtonEventType.Scale:
                    hoverEv.parentTransform.DOKill();
                    hoverEv.parentTransform.DOScale(hover ? hoverEv.targetSize : hoverEv.baseSize, transitionDuration);
                    break;

                case ButtonEventType.ColorImage:
                    hoverEv.spriteImage.DOKill();
                    hoverEv.spriteImage.DOColor(hover ? hoverEv.targetColor : hoverEv.baseColor, transitionDuration);
                    break;

                case ButtonEventType.ColorText:
                    hoverEv.text.DOKill();
                    hoverEv.text.DOColor(hover ? hoverEv.targetColor : hoverEv.baseColor, transitionDuration);
                    break;

                case ButtonEventType.SpriteSwipe:
                    hoverEv.spriteImage.sprite = hover ? hoverEv.targetSprite : hoverEv.baseSprite;
                    break;

                case ButtonEventType.Alpha:
                    hoverEv.canvasGroup.DOKill();
                    hoverEv.canvasGroup.DOFade(hover ? hoverEv.targetAlpha : hoverEv.baseAlpha, transitionDuration);
                    break;

                case ButtonEventType.FillAmount:
                    hoverEv.spriteImage.DOKill();
                    hoverEv.spriteImage.DOFillAmount(hover ? hoverEv.targetAmount : hoverEv.baseAmount, transitionDuration);
                    break;

                case ButtonEventType.DoTweenEffect: 
                    if(hover) hoverEv.doTweenEffect.StartEffect(true);
                    else hoverEv.doTweenEffect.StopEffect();
                    break;
                
                case ButtonEventType.None: break;

                default: break;
            }
        }
        
        #endregion BUTTON METHODS
    }
    
    [Serializable]
    public class ButtonEvent {
        public ButtonEventType eventType = ButtonEventType.None;
        public Transform parentTransform = null;
        public Vector3 targetSize = new(1, 1, 1);
        public Vector3 baseSize = new(1, 1, 1);
        public Image spriteImage = null;
        public TextMeshProUGUI text = null;
        public Color targetColor = new();
        public Color baseColor = new();
        [Range(0, 1)] public float targetAmount = new();
        public float baseAmount = new();
        public Sprite targetSprite = null;
        public Sprite baseSprite = null;
        public CanvasGroup canvasGroup = null;
        [Range(0, 1)] public float targetAlpha = 0;
        public float baseAlpha = 0;
        public DoTweenEffect doTweenEffect = null;
    }

    public enum ButtonEventType {
        Scale,
        ColorImage,
        ColorText,
        SpriteSwipe,
        Alpha,
        FillAmount,
        DoTweenEffect,
        None
    }
}