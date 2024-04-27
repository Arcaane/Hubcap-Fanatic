using TMPro;
using UnityEngine;

public class VersionTxtUpdater : MonoBehaviour {
    /// <summary>
    /// Update the version value of the game
    /// </summary>
    private void Awake() => GetComponent<TextMeshProUGUI>().text = $"VERSION {Application.version}";
}
