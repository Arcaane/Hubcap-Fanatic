using DG.Tweening;
using UnityEngine;

public class DoPunchScale : MonoBehaviour {
    [Header("Data Information")]
    [SerializeField] private Transform tr = null;
    [Space]
    [SerializeField] private float duration = 0f;
    [SerializeField] private Vector3 strength = new();
    [SerializeField] private int vibrato = 10;
    [SerializeField, Range(0,1)] private float elasticity = 90F;
    
    /// <summary>
    /// Method to call in UnityEvents to happen
    /// </summary>
    public void DOPunchScaleEvent() {
        tr.DOPunchScale(strength, duration, vibrato, elasticity);
    }
}
