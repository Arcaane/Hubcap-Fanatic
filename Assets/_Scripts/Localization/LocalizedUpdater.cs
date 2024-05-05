using HubcapLocalisation;
using UnityEngine;
using UnityEngine.Events;

public class LocalizedUpdater : MonoBehaviour {
    [SerializeField] private UnityEvent localizationUpdate = null;
    private void Start() => LocalizationManager.OnLocalizationChanged += () => localizationUpdate.Invoke();
}
