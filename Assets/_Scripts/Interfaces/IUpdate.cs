/// <summary>
/// Interface for the IUpdate which allow to call Update inside a single method Update
/// </summary>
public interface IUpdate {
    public void UpdateTick();
}

/// <summary>
/// Interface for the IFixedUpdate which allow to call FixedUpdate inside a single method FixedUpdate
/// </summary>
public interface IFixedUpdate {
    public void FixedUpdateTick();
}