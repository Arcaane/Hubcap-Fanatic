using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class DecorTarget : MonoBehaviour, ITargetable
{
    public Transform Transform => gameObject.transform;
    
    private bool isAvailable = true;
    public bool IsAvailable => isAvailable;
    public async Task SuccesDash()
    {
        isAvailable = false;
        await Task.Delay((int)(ITargetable.resetTimer * 1000));
        isAvailable = true;
    }

    public async Task FailureDash()
    {
        isAvailable = false;
        await Task.Delay((int)(ITargetable.resetTimer * 1000));
        isAvailable = true;
    }
}
