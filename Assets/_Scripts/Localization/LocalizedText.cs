using TMPro;
using UnityEngine;

namespace HubcapLocalisation {
    public class LocalizedText : MonoBehaviour {
        [SerializeField] private string key = "";
        private TextMeshProUGUI text = null;

        private void Start() {
            text = GetComponent<TextMeshProUGUI>();
            LocalizationManager.OnLocalizationChanged += UpdateText;
            UpdateText();
        }

        /// <summary>
        /// Update the current text based on the language
        /// </summary>
        private void UpdateText() => text.text = LocalizationManager.Localize(key);
    }
}