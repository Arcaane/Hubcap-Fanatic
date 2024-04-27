using System;
using UnityEngine;

/// <summary>
/// This is the extension of the interface IUpdate. If you add this class to a script you'll also need to add the IUpdate interface and implement the UpdateTick method
/// </summary>
public class UpdatesHandler : MonoBehaviour {
    private bool hasMadeRegistrationAtStart = false;
    
    private void Start() {
        AddToRegistration(OnStartContinue);
        hasMadeRegistrationAtStart = true;
    }
    private void OnEnable() {
        if(hasMadeRegistrationAtStart) AddToRegistration(OnEnableContinue);
    }

    /// <summary>
    /// Add the object to the list of IUpdate
    /// </summary>
    private void AddToRegistration(Action actionToCall) {
        if (this is IUpdate) UpdateManager.Instance.RegisterIUpdate(this as IUpdate);
        if (this is IFixedUpdate) UpdateManager.Instance.RegisterIFixedUpdate(this as IFixedUpdate);
        actionToCall?.Invoke();
    }
    
    
    private void OnDisable() => RemoveFromRegistration(OnDisableContinue);
    private void OnDestroy() => RemoveFromRegistration(OnDestroyContinue);
    
    /// <summary>
    /// Remove the object from the list of IUpdate
    /// </summary>
    private void RemoveFromRegistration(Action actionToCall) {
        if (this is IUpdate) UpdateManager.Instance.UnRegisterIUpdate(this as IUpdate);
        if (this is IFixedUpdate) UpdateManager.Instance.UnRegisterIFixedUpdate(this as IFixedUpdate);
        actionToCall?.Invoke();
    }
    
    
    //Methods extension
    protected virtual void OnStartContinue() { }
    protected virtual void OnEnableContinue() { }
    protected virtual void OnDestroyContinue() { }
    protected virtual void OnDisableContinue() { }
}