using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CarDash : MonoBehaviour
{
    [Header("DASH")]
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] public float dashSpeed;
    [SerializeField] private float dashCooldown = 1;
    
    private float dashTimer = 0;
    public float resetSpeed = 10;
    public bool IsDashing => isDashing;
    private bool isDashing;
    
    [HideInInspector] public Vector3 dashForward;
    
    [Space]
    [Header("Targets groups")] 
    private List<ITargetable> targetsInLevel = new(); // Contain all targets that can be reached on load level
    private List<ITargetable> targetsReachable = new(); // Contain all targets that can be reached at this moment
    private ITargetable currentTarget;
    private ITargetable storedTarget;
    private Transform cachedTarget;
    
    [Space]
    [Header("Detection Settings")]
    public float detectionDst;
    [FormerlySerializedAs("isLeftTrigger")] public bool isInAimingMode = false;
    
    [Space]
    [Header("User Interface")]
    public RectTransform rectImage;
    public float rectSizeMultiplier = 2;
    [SerializeField] ClonedTargetSystem targetClonePrefab;
    
    [Space]
    [Header("Charge Settings")]
    public bool isCharging;
    [SerializeField] private float chargeDuration = .8f;
    [SerializeField] private float chargeAmount;
    [SerializeField] private float middleChargeTolerance = 0.10f;
    [SerializeField] private float endChargeTolerance = 0.25f;
    [SerializeField] private Slider chargeSlider;
    
    //Events
    [HideInInspector] public UnityEvent OnInputRelease;
    [HideInInspector] public UnityEvent OnTargetChange;
    
    void Start()
    {
        var tempTargs = FindObjectsOfType<MonoBehaviour>().ToArray();
        foreach (var t in tempTargs.Where(args => args is ITargetable).Cast<ITargetable>())
        {
            targetsInLevel.Add(t);
        }
        
        OnInputRelease.AddListener(CheckRelease);
        OnTargetChange.AddListener(ResetChargeAmount);

        chargeSlider.maxValue = chargeDuration;
    }
    
    void Update()
    {
        if (dashCooldown > 0) dashCooldown -= Time.deltaTime;
        else
        {
            dashCooldown = 0;
        }

        if (isCharging)
        {
            SetChargeAmount(chargeAmount += Time.deltaTime);
        }

        if (isInAimingMode)
        {
            rectImage.gameObject.SetActive(true);
            rectImage.transform.position = ClampedScreenPosition(currentTarget.Transform.position);
            float distanceFromTarget = Vector3.Distance(currentTarget.Transform.position, transform.position);
            rectImage.sizeDelta = new Vector2(Mathf.Clamp(115 - (distanceFromTarget - rectSizeMultiplier),50,200), Mathf.Clamp(115 - (distanceFromTarget - rectSizeMultiplier),50,200));
        }
        
        foreach (var t in targetsInLevel)
        {
            if (targetsReachable.Contains(t))
            {
                if (!IsTargetReachable(t) || !t.IsAvailable)
                {
                    targetsReachable.Remove(t);
                }
            }
            else // isReachable
            {
                if (IsTargetReachable(t) && t.IsAvailable) 
                    targetsReachable.Add(t);
            }
        }

        if (targetsReachable.Count < 1)
        {
            rectImage.gameObject.SetActive(false);
            isCharging = false;
            return;
        }

        if (isInAimingMode && !isDashing && currentTarget != null) isCharging = true;
        
        CheckTargetChange();

        if (!isCharging)
        {
            currentTarget = ClosestTarget();
        }
    }

    #region DashGestion
    private async void Dash(ITargetable target = null)
    {
        isDashing = true;

        dashForward = transform.forward;
        // POS
        Vector3 initPos = transform.position;
        Vector3 finalPos = default;
        // ROT
        Quaternion initRotation = transform.rotation;
        Quaternion finalRotation = default;
        
        Vector3 dir = default;
        Vector3 inverseDir = default;
        
        if (target != null) // AS UNE TARGET
        {
            inverseDir = target.Transform.position - transform.position;
            inverseDir.Normalize();
            finalPos = target.Transform.position + (inverseDir * 2);
            finalRotation = Quaternion.LookRotation(inverseDir);
            
        }
        else // DASH NORMAL
        {
            finalPos = initPos + transform.forward * dashSpeed;
            finalPos.y = initPos.y;
            initRotation = transform.rotation;
            finalRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        }
        
        float timer = 0;
        while (timer < dashDuration)
        {
            transform.position = Vector3.Lerp(initPos, finalPos, timer / dashDuration);
            transform.rotation = Quaternion.Lerp(initRotation, finalRotation, timer / dashDuration);
            await Task.Yield();
            timer += Time.deltaTime;
        }
        
        transform.position = finalPos;
        transform.rotation = finalRotation;
        GetComponent<Rigidbody>().velocity = (target == null ? transform.forward : inverseDir) * resetSpeed;
        isDashing = false;
    }
    
    void SetChargeAmount(float charge)
    {
        chargeAmount = charge;
        chargeSlider.value = chargeAmount;
    }

    private bool HalfCharge()
    {
        return chargeAmount > chargeDuration / 2 - middleChargeTolerance && chargeAmount < chargeDuration / 2 + middleChargeTolerance;
    }
    
    private bool FullCharge()
    {
        return chargeAmount >= chargeDuration - endChargeTolerance && chargeAmount < chargeDuration + endChargeTolerance;
    }
    
    private void CheckRelease()
    {
        isCharging = false;
        
        if (HalfCharge()) chargeAmount = .5f;

        if (FullCharge()) chargeAmount = 1;
        
        CloneInterface(chargeAmount);
        ApplyRelease(chargeAmount);
        
        isInAimingMode = false;
        rectImage.gameObject.SetActive(false);
        SetChargeAmount(0);
    }
    private void ApplyRelease(float amount)
    {
        if (amount == 0.5f || amount == 1) // Réussi
        {
            // Faire Dash
            Dash(currentTarget);
            // Enlever la target des targets dispo
            currentTarget?.SuccesDash();
            StopTargetFocus();
        }
        else // Raté
        {
            currentTarget?.FailureDash();
            StopTargetFocus();
        }
    }
    #endregion
    
    #region Input
    public void AButton(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (isInAimingMode)
            {
                OnInputRelease.Invoke();
            }
            if (dashCooldown <= 0 && !isDashing) Dash();
            isCharging = false;
        }
    }
    public void LBButton(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (ClosestTarget() != null) isInAimingMode = true;
        }
    }
    #endregion
    
    #region Target Detection
    private bool IsTargetReachable(ITargetable t) =>  Vector3.Distance(t.Transform.position, transform.position) < detectionDst;

    private ITargetable ClosestTarget()
    {
        var tempDst = 1000;
        ITargetable returnTargetable = null;
        
        foreach (var t in targetsReachable.Where(t => returnTargetable is null || Vector3.Distance(t.Transform.position, transform.position) < tempDst))
        {
            returnTargetable = t;
        }

        return returnTargetable;
    }
    
    private void CheckTargetChange()
    {
        if (storedTarget != currentTarget)
        {
            storedTarget = currentTarget;
            rectImage.DOComplete();
            rectImage.DOScale(4, .2f).From();
            OnTargetChange.Invoke();
        }
    }

    private void StopTargetFocus()
    {
        currentTarget = null;
    }
    
    private void CloneInterface(float chargeValue)
    {
        if (targetClonePrefab == null)
            return;

        ClonedTargetSystem clonedTarget = Instantiate(targetClonePrefab, rectImage.position, rectImage.rotation, rectImage.parent);
        float sliderValue = chargeValue;
        clonedTarget.SetupClone(cachedTarget,transform, rectImage.sizeDelta, sliderValue);
    }

    private void ResetChargeAmount()
    {
        SetChargeAmount(0);
    }
    #endregion
    
    Vector3 ClampedScreenPosition(Vector3 targetPos)
    {
        Vector3 WorldToScreenPos = Camera.main.WorldToScreenPoint(targetPos);
        Vector3 clampedPosition = new Vector3(Mathf.Clamp(WorldToScreenPos.x, 0, Screen.width), Mathf.Clamp(WorldToScreenPos.y, 0, Screen.height), WorldToScreenPos.z);
        return clampedPosition;
    }
}
