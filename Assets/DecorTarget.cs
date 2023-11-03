using System;
using System.Threading;
using UnityEngine;

public class DecorTarget : MonoBehaviour, ITargetable
{
    public Vector3 Position => gameObject.transform.position;
    
    private Canvas targetCanvas;
    public Canvas TargetCanvas => targetCanvas;

    private float fillValue;
    public float FillValue => fillValue;
    
    private bool isFilling;
    public bool IsFilling => isFilling;

    private float timer;
    private bool isInCooldown;
    public bool IsInCooldown => isInCooldown;

    // Update is called once per frame
    void Update()
    {
        UpdateFill();
    }
    
    public void StartFill()
    {
        fillValue = 0;
        isFilling = true;
    }

    public void UpdateFill()
    {
        if (isInCooldown)
        {
            timer += Time.deltaTime;
            if (timer > ITargetable.TargetCooldown)
            {
                isInCooldown = false;
                timer = 0;
            }
        }
        
        if (isInCooldown) return;
        if (isFilling)
        {
            fillValue += Time.deltaTime;
            if (fillValue > 0.5 - ITargetable.PerfectTimingTolerance && fillValue < 0.5 + ITargetable.PerfectTimingTolerance)
            {
                // Perfect timing
            }
            else if (fillValue > 1 - ITargetable.EndTimingTolerance && fillValue < 1 + ITargetable.EndTimingTolerance)
            {
                // Normal Timing
            }
            else if(fillValue > 1 + ITargetable.EndTimingTolerance)
            {
                // Désactiver pour un moment
            }
        }
    }

    public void CancelTarget()
    {
        isFilling = false;
        fillValue = 0;
    }

    public void OnTargetVisible()
    {
        throw new NotImplementedException();
    }

    public void OnTargetInvisible()
    {
        throw new NotImplementedException();
    }
}
