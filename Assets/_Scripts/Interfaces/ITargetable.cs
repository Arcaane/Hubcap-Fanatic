using UnityEngine;

public interface ITargetable
{
    Vector3 Position { get; }
    Canvas TargetCanvas { get; }
    float FillValue { get; }
    bool IsFilling { get; }
    bool IsInCooldown { get; }

    public static float PerfectTimingTolerance = 0.15f;
    public static float EndTimingTolerance = 0.25f;
    public static float TargetCooldown = 1f;

    public void StartFill();
    public void UpdateFill();
    public void CancelTarget();

    public void OnTargetVisible();
    public void OnTargetInvisible();
}
