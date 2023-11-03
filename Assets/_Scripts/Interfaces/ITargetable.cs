using System.Threading.Tasks;
using UnityEngine;

public interface ITargetable
{
    Transform Transform { get; }
    
    bool IsAvailable { get; }

    static float resetTimer = 1.5f;
    Task SuccesDash();
    Task FailureDash();
}
