using UnityEngine;

public interface ITargetable
{
    Transform Transform { get; }
    
    public static bool IsAvailable = true;
    
    
}
