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
}
