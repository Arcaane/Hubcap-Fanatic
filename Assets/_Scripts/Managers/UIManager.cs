using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abilities;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] private Image nitroJauge;
    public RadarDetectorUI radar;
    public GameObject shopScreen;

    [Header("Slider Information")] 
    [SerializeField] private SliderManager waveSliderManager = null;
    [SerializeField] private SliderManager levelSliderManager = null;
    [SerializeField] private SliderManager lifeSliderManager = null;
    [SerializeField] private SliderManager nitroSliderManager = null;
    [SerializeField] private List<SliderManager> shotgunSliders = new();

    [Header("Player vitals")]
    [SerializeField] private Image lifeImage;
    [SerializeField] private Image easeLifeJauge;
    [SerializeField] private TextMeshProUGUI lifeText;
    [SerializeField] private TextMeshProUGUI lifeText2;
    
    [SerializeField] public ButtonsItems[] buttonsHandlers;

    [SerializeField] public Transform shootIcon,shopIcon;

    [SerializeField] public Transform[] fireShots;
    
    [SerializeField] private Image[] moveOnDeath;
    
    [Header("Shotgun part")]
    [SerializeField] public Image[] shotJauges;
    [SerializeField] private RectTransform[] shotGunIconsFlammes;
    [SerializeField] private Color usable, unusable;

    [SerializeField] public AbilitiesPair[] abilitiesSlots;

    [Serializable]
    public class AbilitiesPair
    {
        public Image passiveAbilityIcon;
        public Image abilityCooldownSlider;
        public TextMeshProUGUI cdText;
        public Image statAbilityIcon;
    }
    
    public void SetAbilityCdInUI()
    {
        for (int i = 0; i < abilitiesSlots.Length; i++)
        {
            abilitiesSlots[i].abilityCooldownSlider = abilitiesSlots[i].passiveAbilityIcon.gameObject.transform.parent.GetChild(1).GetComponent<Image>();
            abilitiesSlots[i].cdText = abilitiesSlots[i].passiveAbilityIcon.gameObject.transform.parent.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        }
    }

    [Header("Merchant")]
    [SerializeField] private TextMeshProUGUI merchantText;
    [SerializeField] private TextMeshProUGUI merchantText2;
    
    [Header("ShopUI")]
    [SerializeField] public ShopOption[] shopOptions;
    [SerializeField] private CanvasGroup hudGroup,shopGroup;
    [SerializeField] private AnimationCurve[] optionsCurves;
    [SerializeField] private bool shopOpen,shopTransition;
    public bool ShopOpen => shopOpen;
    [SerializeField] private int shopOptionSelected = 0;
    [SerializeField] private TextMeshProUGUI shoptokenText;
    [SerializeField] private TextMeshProUGUI shoptokenText2;
    [SerializeField] private Image exitSign;

    private bool stickUsed;
    private Vector2 stickValue;
    public bool pause;
    public GameObject abilityCorner;
    public GameObject pauseMenu;
    public ShopOption[] abilitiesPause,statsPause;
    
    [Header("Player Bounds")]
    [SerializeField] private GameObject fillParent = null;
    [SerializeField] private Image fillImage = null;
    
    private void Awake()
    {
        instance = this;
    }

    private void Start() {
        UpdateExperienceData(0, "Level 01");

        SetAbilityCdInUI();
        
        for (int i = 0; i < abilitiesSlots.Length; i++)
        {
            abilitiesSlots[i].abilityCooldownSlider.gameObject.SetActive(false);
        }
        
        fillParent.SetActive(false);
        fillImage.fillAmount = 1;
        
        moveOnDeath[0].fillAmount = 1;
        moveOnDeath[1].fillAmount = 1;
        
        moveOnDeath[0].DOFillAmount(0, 0.35f);
        moveOnDeath[1].DOFillAmount(0, 0.35f);
        
        moveOnDeath[0].fillAmount = moveOnDeath[1].fillAmount = 0;
    }

    

    /*public void SetShotJauge(float amount,int shot)
    {
        shotJauges[shot].fillAmount = amount;
        if (amount > 1)
        {
            shotJauges[shot].color = usable;
            SetUIShotgunUsable(shotJauges[shot]);
        }
        else
        {
            shotJauges[shot].color = unusable;
        }
    }*/

    /*private void SetUIShotgunUsable(Image img)
    {
        img.transform.DOShakeScale(0.3f, 1.04f, 11, 90f, true, ShakeRandomnessMode.Harmonic);
        img.DOColor(Color.white, 0.15f).OnComplete(() => img.DOColor(usable, 0.1f));
    }*/

    public void ShootMissUI(int i)
    {
        shotJauges[i].transform.parent.DOLocalRotate(new Vector3(0,0,360), 0.5f, RotateMode.LocalAxisAdd).OnComplete(() => shotJauges[i].transform.parent.transform.localRotation = Quaternion.Euler(0,0,0));
    }

    public void GoodShotUI(int i)
    {
        shotJauges[i].transform.parent.DOLocalMoveY(0, 0.35f, true).
            SetEase(Ease.OutElastic).OnComplete(() => shotJauges[i].transform.parent.
                DOLocalMoveY(-30, 0.15f, true)).SetEase(Ease.InBack);
    }
    
    
    
    /// <summary>
    /// Update data related to the life of the player
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="level"></param>
    public void UpdateLifeData(float amount, string level) => lifeSliderManager.UpdateData(level, amount);

    /// <summary>
    /// Update data related to the level and experience of the player
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="level"></param>
    public void UpdateExperienceData(float amount, string level) => levelSliderManager.UpdateData(level, amount);
    
    /// <summary>
    /// Update data related to the wave progression
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="wave"></param>
    public void UpdateWaveData(float amount, string wave) => waveSliderManager.UpdateData(wave, amount);
    
    /// <summary>
    /// Update data related to the shotguns bullet
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="id"></param>
    public void UpdateShotgunData(float amount, int id) => shotgunSliders[id].UpdateData("Shot", amount);
    
    /// <summary>
    /// Update data related to the nitro
    /// </summary>
    /// <param name="amount"></param>
    public void UpdateNitroData(float amount) => nitroSliderManager.UpdateData("Nitro", amount);
    public void UpdateTargetLoadingNitro(float target) => nitroSliderManager.UpdateTargetValue(target);
    
    

    public void SetGoldText(int i)
    {
        //tokenText2.text = tokenText.text = i.ToString();
    }

    public void UnlockAbilitySlot(int i)
    {
        abilitiesSlots[i].passiveAbilityIcon.transform.parent.parent.parent.gameObject.SetActive(true);
    }
    
    public async void UpdateMerchantNotif(string text)
    {
        Debug.Log("MERCHANT " + text);
        merchantText.gameObject.SetActive(true);
        merchantText2.gameObject.SetActive(true);
        merchantText.text = merchantText2.text = text;
        await Task.Delay(5000);
        merchantText.gameObject.SetActive(false);
        merchantText2.gameObject.SetActive(false);
    }

    public async void OpenShopScreen()
    {
        if (shopTransition || shopOpen) return;
        shopTransition = true;
        Time.timeScale = 0;
        float i = 0;
        while (i < 1)
        {
            i += Time.unscaledDeltaTime;
            hudGroup.alpha = 1 - i;
            shopGroup.alpha = i;
            for (int j = 0; j < 3; j++)
            {
                shopOptions[j].transform.localScale = optionsCurves[j].Evaluate(i) * Vector3.one;
            }
            await Task.Yield();
        }
        hudGroup.alpha = 0;
        shopGroup.alpha = 1;
        for (int j = 0; j < 3; j++)
        {
            shopOptions[j].transform.localScale = Vector3.one;
        }
        shopTransition = false;
        shopOpen = true;
    }
    
    public async void CloseShopScreen()
    {
        if (shopTransition || !shopOpen) return;
        shopTransition = true;
        float i = 0;
        while (i < 1)
        {
            i += Time.unscaledDeltaTime;
            hudGroup.alpha = i;
            shopGroup.alpha = 1 - i;
            for (int j = 0; j < 3; j++)
            {
                shopOptions[j].transform.localScale = optionsCurves[j].Evaluate(1 - i) * Vector3.one;
            }
            await Task.Yield();
        }
        hudGroup.alpha = 1;
        shopGroup.alpha = 0;
        for (int j = 0; j < 3; j++)
        {
            shopOptions[j].transform.localScale = Vector3.zero;
        }
        shopTransition = false;
        shopOpen = false;
        Time.timeScale = 1;
    }

    
    public void OnOpenPause()
    {
        if (shopTransition || shopOpen || pause) return;
        pause = true;
        Time.timeScale = 0;
        hudGroup.alpha = 0;
        abilityCorner.SetActive(false);
        pauseMenu.SetActive(true);

        for (int i = 0; i < CarAbilitiesManager.instance.passiveAbilities.Count; i++)
        {
            abilitiesPause[i].SetPauseOption(CarAbilitiesManager.instance.passiveAbilities[i]);
        }
        
        for (int i = 0; i < CarAbilitiesManager.instance.statsAbilities.Count; i++)
        {
            statsPause[i].SetPauseOption(CarAbilitiesManager.instance.statsAbilities[i]);
        }
    }

    public void OnClosePause()
    {
        if (!pause) return;
        pause = false;
        Time.timeScale = 1;
        hudGroup.alpha = 1;
        abilityCorner.SetActive(true);
        pauseMenu.SetActive(false);
    }
    
    #region Inputs

    public void LStick(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            stickValue = context.ReadValue<Vector2>();
            if (!stickUsed && stickValue.magnitude > 0.5f)
            {
                stickUsed = true;
                float angle = Vector2.SignedAngle(Vector2.up, stickValue);
                
                if (angle > 30 && angle < 150)
                {
                    LeftChoice();
                }
                else if (angle < -30 && angle > -150)
                {
                    RightChoice();
                }
                
            } 
            
        }
        else
        {
            stickValue = Vector2.zero;
            stickUsed = false;
        }
    }


    void LeftChoice()
    {
        if (!shopOpen) return;
        if (shopOptionSelected >= 0)
        {
            shopOptionSelected -= 1;
            if (shopOptionSelected < 0) shopOptionSelected = 2;
        }
    }
    
    void RightChoice()
    {
        if (!shopOpen) return;
        if (shopOptionSelected >= 0)
        {
            shopOptionSelected += 1;
            if (shopOptionSelected > 2) shopOptionSelected = 0;
        }
    }
    
   

    #endregion


    public float easeLifeLerpSpeed = 1f;
    private void Update()
    {
    	if (Math.Abs(lifeImage.fillAmount - easeLifeJauge.fillAmount) > 0.0001f)
        {
            easeLifeJauge.fillAmount = Mathf.Lerp(easeLifeJauge.fillAmount, lifeImage.fillAmount, easeLifeLerpSpeed);
        }
    
        if (CarController.instance.isBerserk)
        {
            fireShots[0].localScale = fireShots[1].localScale = Vector3.Lerp(fireShots[0].localScale, Vector3.one, Time.deltaTime * 5);
        }
        else fireShots[0].localScale = fireShots[1].localScale = Vector3.Lerp(fireShots[0].localScale, Vector3.zero, Time.deltaTime * 5);
        
        
        if (!shopOpen) return;

        for (int i = 0; i < 3; i++)
        {
            if(shopOptionSelected == i) shopOptions[i].transform.localScale = Vector3.Lerp(shopOptions[i].transform.localScale,Vector3.one * 1.2f,Time.unscaledDeltaTime * 5);
            else shopOptions[i].transform.localScale = Vector3.Lerp(shopOptions[i].transform.localScale,Vector3.one * 0.8f,Time.unscaledDeltaTime * 5);

        }
    }
    
    public void AButton(InputAction.CallbackContext context)
    {
        if (!shopOpen) return;
        
        if (context.started)
        {
            if (shopOptionSelected >= 0)
            {
                if(CarExperienceManager.Instance.levelUpTokensAvailable > 0) shopOptions[shopOptionSelected].Buy();
            }
            else CloseShopScreen();
        }
    }
    
    
    // Tweening
    private float effectDuration = 0.275f;
    private float shakeStrength = 15;
    private int shakeVibrato = 20;
    private float shakeRandomness = 90;
    public void UITakeDamage()
    {
        lifeImage.transform.DOShakePosition(effectDuration, new Vector3(shakeStrength,0,0), shakeVibrato, shakeRandomness).SetLoops(1, LoopType.Restart).OnComplete(() => lifeImage.transform.localPosition = new Vector3(0,0,0));
        easeLifeJauge.transform.DOShakePosition(effectDuration, new Vector3(shakeStrength,0,0), shakeVibrato, shakeRandomness).SetLoops(1, LoopType.Restart).OnComplete(() => easeLifeJauge.transform.localPosition = new Vector3(0,0,0));
        lifeText.color = lifeText2.color = Color.white;
        StartCoroutine(SetLifeGoodTextColor(0.14f));
    }
    
    IEnumerator SetLifeGoodTextColor(float time)
    {
        var a = time;
        while (a > 0)
        {
            a -= Time.deltaTime;
            yield return null;
        }
        
        lifeText.color = new Color(1.0f, 0.4f, 0.31f, 1.0f);
        lifeText2.color = new Color(0.15f, 0.15f, 0.15f, 1.0f);
    }
    
    /*private void UINextWave()
    {
        waveCountText.color = waveCountText2.color = Color.white;
        StartCoroutine(SetWaveGoodTextColor(0.14f));
        
        waveDurationJauge.transform.parent.DOScale(1.15f, 0.5f).SetEase(Ease.OutBack).OnComplete(() => waveDurationJauge.transform.parent.DOScale(1, 0.25f));
    }
    
    IEnumerator SetWaveGoodTextColor(float time)
    {
        var a = time;
        while (a > 0)
        {
            a -= Time.deltaTime;
            yield return null;
        }
        
        waveCountText.color = new Color(1.0f, 0.4f, 0.31f, 1.0f);
        waveCountText2.color = new Color(0.15f, 0.15f, 0.15f, 1.0f);
    }*/

    public void UpdateOutOfBoundsTxt(float timer, float maxTimer) {
        fillParent.SetActive(true);
        fillImage.fillAmount = timer / maxTimer;
    }

    public void CloseOutOfBounds() {
        fillParent.SetActive(false);
        fillImage.fillAmount = 1;
    }

    public void BlackScreenDeath(AsyncOperation asyncOperation) {
        moveOnDeath[0].DOFillAmount(1, 0.35f);
        moveOnDeath[1].DOFillAmount(1, 0.35f).OnComplete(() => asyncOperation.allowSceneActivation = true);
    }
}
