using UnityEngine;

/// <summary>
/// This is the extension of the interface IUpdate. If you add this class to a script you'll also need to add the IUpdate interface and implement the UpdateTick method
/// </summary>
public class UpdatesHandler : MonoBehaviour {
    private bool hasMadeRegistration = false;
    private bool hasMadeStartRegistration = false;

    protected virtual void Start() {
        if (!hasMadeRegistration) {
            AddToRegistration();
            hasMadeStartRegistration = true;
        }
    }
    protected virtual void OnEnable() {
        if (!hasMadeRegistration && hasMadeStartRegistration) AddToRegistration();
    }

    /// <summary>
    /// Add the object to the list of IUpdate
    /// </summary>
    private void AddToRegistration() {
        if (this is IUpdate) UpdateManager.Instance.RegisterIUpdate(this as IUpdate);
        if (this is IFixedUpdate) UpdateManager.Instance.RegisterIFixedUpdate(this as IFixedUpdate);
        hasMadeRegistration = true;
    }
    
    
    protected virtual void OnDisable() {
        if(hasMadeRegistration) RemoveFromRegistration();
    }
    protected virtual void OnDestroy() {
        if(hasMadeRegistration) RemoveFromRegistration();
    }

    /// <summary>
    /// Remove the object from the list of IUpdate
    /// </summary>
    private void RemoveFromRegistration() {
        if (this is IUpdate) UpdateManager.Instance.UnRegisterIUpdate(this as IUpdate);
        if (this is IFixedUpdate) UpdateManager.Instance.UnRegisterIFixedUpdate(this as IFixedUpdate);
        hasMadeRegistration = false;
    }
}