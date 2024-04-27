using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderUpdater : MonoBehaviour {
    [SerializeField, Range(0,1)] private float sliderValue = 0f;
    [SerializeField] private Image sliderImg = null;
    
    [Header("MinValue Data")] 
    [SerializeField] private bool useMinValue = false;
    [SerializeField] private float minActiveAmount = 0f;
    [SerializeField] private Sprite enableSliderSprite = null;
    [SerializeField] private Sprite disableSliderSprite = null;

    [Header("Text Data")]
    [SerializeField] private bool updateTextBasedOnValue = false;
    [SerializeField] private TextMeshProUGUI sliderTxt = null;
    [SerializeField] private TextMeshProUGUI sliderBackgroundTxt = null;
    [SerializeField] private string frontTextBeforeValue = "";
    private int textSliderValue = 0;

#if UNITY_EDITOR
    private void OnValidate() {
        if (sliderImg == null) return;
        sliderImg.fillAmount = sliderValue;
    }
#endif

    /// <summary>
    /// Initialize the minimum value 
    /// </summary>
    /// <param name="amount"></param>
    public void InitMiNValue(float amount) => minActiveAmount = amount;
    
    /// <summary>
    /// Update the value of the slider
    /// </summary>
    /// <param name="amount"></param>
    public void UpdateSliderValue(float amount, int textSliderValue = 0) {
        sliderValue = amount;
        this.textSliderValue = textSliderValue;
        
        sliderImg.fillAmount = sliderValue;
        UpdateSliderVisual();
    }

    private void UpdateSliderVisual() {
        UpdateVisualBaseddOnMinValue();
        UpdateTextVisual();
    }

    /// <summary>
    /// Update the visual of the slider sprite based on the current value and the target min value
    /// </summary>
    private void UpdateVisualBaseddOnMinValue() {
        if (!useMinValue) return;
        if (disableSliderSprite == null) return;
        sliderImg.sprite = sliderValue <= minActiveAmount ? disableSliderSprite : enableSliderSprite;
    }

    private void UpdateTextVisual() {
        if (!updateTextBasedOnValue) return;
        if (sliderTxt == null || sliderBackgroundTxt == null) return;
        sliderTxt.text = sliderBackgroundTxt.text = $"{frontTextBeforeValue} {textSliderValue}";
    }
}
