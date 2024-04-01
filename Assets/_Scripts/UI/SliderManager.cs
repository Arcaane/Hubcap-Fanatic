using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour {
    [Header("Data")]
    [SerializeField] private SliderSO sliderData = null;
    [SerializeField] private Image.OriginHorizontal sliderSide = Image.OriginHorizontal.Right;
    [SerializeField] private bool useCustomSpriteLoad = false;
    [SerializeField] private bool useCustomTxtColorLoad = false;
    [SerializeField] private float targetLoadingValue = 1;
    [SerializeField] private string sliderName = "";
    [SerializeField, Range(0,1)] private float fillAmount = 1;
    private Sprite customSprite = null;
    private bool forceCustomSprite = false;
    [Space]
    [SerializeField] private Color customTextColor = new Color();
    [SerializeField] private Color customLoadingTextColor = new Color();
    [Space]
    [SerializeField] private Sprite customColorSlider = null;
    [SerializeField] private Sprite customLoadingColorSlider = null;
    [Space]
    [SerializeField] private float sliderSpeed = 2;

    [Header("Components")] 
    [SerializeField] private Image sliderColor = null;
    [SerializeField] private Image sliderBackground = null;
    [SerializeField] private Image sliderShadow = null;
    [SerializeField] private TextMeshProUGUI sliderTxt = null;
    [SerializeField] private TextMeshProUGUI sliderColoredTxt = null;

    [Header("Events")]
    [SerializeField] private UnityEvent OnValueComplete = new();
    [SerializeField] private UnityEvent OnTargetValueReach = new();
    private bool completeEventDone = true;
    private bool completeTargetDone = true;

    private void Start() => UpdateAllComponents();

    #if UNITY_EDITOR
    private void OnValidate() => UpdateAllComponents(true);
    #endif

    public void UpdateTargetValue(float target) => targetLoadingValue = target;
    
    public void UpdateData(string sliderName, float fillAmount) {
        this.sliderName = sliderName;
        this.fillAmount = fillAmount;
        
        CheckEvents();
        
        UpdateAllComponents();
    }

    /// <summary>
    /// Check if some events need to be called
    /// </summary>
    private void CheckEvents() {
        // REACH MAX VALUE
        switch (CheckValue(1, fillAmount)) {
            case true when !completeEventDone:
                OnValueComplete.Invoke();
                completeEventDone = true;
                break;
            case false when completeEventDone:
                completeEventDone = false;
                break;
        }
        
        // REACH TARGET VALUE
        switch (CheckValue(targetLoadingValue, fillAmount)) {
            case true when !completeTargetDone:
                OnTargetValueReach.Invoke();
                completeTargetDone = true;
                break;
            case false when completeTargetDone:
                completeTargetDone = false;
                break;
        }
    }

    /// <summary>
    /// Check if the current value is above or equal the target value
    /// </summary>
    /// <param name="target"></param>
    /// <param name="current"></param>
    /// <returns></returns>
    private bool CheckValue(float target, float current) => current >= target;


    /// <summary>
    /// Update all the components based on the new data
    /// </summary>
    private void UpdateAllComponents(bool forceChanges = false) {
        ApplyColor();
        sliderTxt.text = sliderColoredTxt.text = sliderName;
        sliderTxt.alignment = sliderSide == Image.OriginHorizontal.Left ? TextAlignmentOptions.MidlineLeft : TextAlignmentOptions.MidlineRight;
        sliderColoredTxt.alignment = sliderSide == Image.OriginHorizontal.Left ? TextAlignmentOptions.MidlineLeft : TextAlignmentOptions.MidlineRight;
        
        sliderColor.fillOrigin = (int)sliderSide;
        if (!forceChanges) {
            sliderColor.DOKill();
            sliderColor.DOFillAmount(fillAmount, Mathf.Abs(sliderColor.fillAmount - fillAmount) * sliderSpeed);
        }
        else sliderColor.fillAmount = fillAmount;

        if (forceCustomSprite) sliderColor.sprite = customSprite;
        else if (useCustomSpriteLoad && fillAmount < targetLoadingValue) sliderColor.sprite = customLoadingColorSlider;
        else sliderColor.sprite = customColorSlider;
    }

    /// <summary>
    /// Change the state of force sprite
    /// </summary>
    /// <param name="forceCustomSprite"></param>
    /// <param name="customSprite"></param>
    public void ChangeForceSprite(bool forceCustomSprite, Sprite customSprite = null) {
        this.forceCustomSprite = forceCustomSprite;
        this.customSprite = customSprite;
        UpdateAllComponents();
    }
    
    /// <summary>
    /// Update colors of all components
    /// </summary>
    private void ApplyColor() {
        sliderBackground.color = sliderData.backgroundColor;
        sliderShadow.color = sliderData.shadowColor;
        
        sliderTxt.color = sliderData.backgroundColor;
        
        if (useCustomTxtColorLoad && fillAmount < targetLoadingValue) sliderColoredTxt.color = customLoadingTextColor;
        else sliderColoredTxt.color = customTextColor;
    }
}