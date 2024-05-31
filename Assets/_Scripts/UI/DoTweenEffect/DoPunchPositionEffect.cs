using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace HubcapInterface.DoTween {
    public class DoPunchPositionEffect : DoTweenEffect {
        [SerializeField] private List<RectTransform> rects = null;
        [Space] 
        [SerializeField] private float duration = 1f;
        [SerializeField] private int vibrato = 15;
        [SerializeField] private Vector3 strength = new();

        public override void StartEffect(bool loop = true) {
            StopEffect();
            foreach (RectTransform rect in rects) {
                tweeners.Add(rect.DOPunchPosition(strength, duration, vibrato, 1, false).SetLoops(loop ? -1 : 1));
            }
        }
    }
}
