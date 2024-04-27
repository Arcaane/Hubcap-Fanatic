using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HubcapInterface {
    public class CustomButtonHandler : UISelectable, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
        [SerializeField] private float transitionDuration = 0.25f;
        [SerializeField] private List<ButtonEvent> buttonHoverEvents = new();
        [Space] [SerializeField] private UnityEvent onClickEvents = new();

        private void Awake() => InitData();

        /// <summary>
        /// Initialize data from the events
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

                    case ButtonEventType.None: break;
                }
            }
        }


        #region MOUSE

        public void OnPointerEnter(PointerEventData eventData) => CallAllButtonEvent(buttonHoverEvents);
        public void OnPointerExit(PointerEventData eventData) => CallAllButtonEvent(buttonHoverEvents, false);
        public void OnPointerClick(PointerEventData eventData) => onClickEvents.Invoke();

        #endregion MOUSE

        #region Gamepad

        public override void DisableSelection() {
            base.DisableSelection();
            CallAllButtonEvent(buttonHoverEvents, false);
        }

        public override void EnableSelection() {
            base.EnableSelection();
            CallAllButtonEvent(buttonHoverEvents);
        }

        public override void ActionInputPressed() => onClickEvents.Invoke();

        #endregion Gamepad

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
        /// <exception cref="ArgumentOutOfRangeException"></exception>
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

                case ButtonEventType.None: break;

                default: throw new ArgumentOutOfRangeException();
            }
        }
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
    }

    public enum ButtonEventType {
        Scale,
        ColorImage,
        ColorText,
        SpriteSwipe,
        Alpha,
        FillAmount,
        None
    }
}