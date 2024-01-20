using System;
using System.Threading.Tasks;
using Abilities;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] private Image nitroJauge;
    public RadarDetectorUI radar;
    public GameObject shopScreen;
    
    [Header("Level & Experience")]
    [SerializeField] private Image experienceJauge;
    [SerializeField] private TextMeshProUGUI playerLevelText;
    [SerializeField] private TextMeshProUGUI playerLevelText2;
    
    [SerializeField] private TextMeshProUGUI tokenText;
    [SerializeField] private TextMeshProUGUI tokenText2;
    
    [Header("Wave Informations")]
    [SerializeField] private Image waveDurationJauge;
    [SerializeField] private TextMeshProUGUI waveCountText;
    [SerializeField] private TextMeshProUGUI waveCountText2;

    [Header("Player vitals")]
    [SerializeField] private Image lifeImage;
    [SerializeField] private TextMeshProUGUI lifeText;
    [SerializeField] private TextMeshProUGUI lifeText2;
    
    [SerializeField] public ButtonsItems[] buttonsHandlers;

    [SerializeField] public Transform shootIcon,shopIcon;

    [Header("Shotgun part")]
    [SerializeField] private Image[] shotJauges;
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
    


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetExperienceFillAmount(0);

        SetAbilityCdInUI();
        
        for (int i = 0; i < abilitiesSlots.Length; i++)
        {
            abilitiesSlots[i].abilityCooldownSlider.gameObject.SetActive(false);
        }
    }
    
    public void ActivateShotgunFlamme()
    {
        foreach (RectTransform SF in shotGunIconsFlammes)
        {
            SF.gameObject.SetActive(true);
        }
    }
    
    public void DeactivateShotgunFlamme()
    {
        foreach (RectTransform SF in shotGunIconsFlammes)
        {
            SF.gameObject.SetActive(false);
        }
    }

    public void SetNitroJauge(float amount)
    {
        nitroJauge.fillAmount = amount;
        if (amount > 0.25f)
        {
            nitroJauge.color = usable;
        }
        else
        {
            nitroJauge.color = unusable;
        }
    }
    
    public void SetShotJauge(float amount,int shot)
    {
        shotJauges[shot].fillAmount = amount;
        if (amount > 1)
        {
            shotJauges[shot].color = usable;
        }
        else
        {
            shotJauges[shot].color = unusable;
        }
    }

    public void SetExperienceFillAmount(float amount)
    {
        experienceJauge.fillAmount = amount;
    }

    public void SetLevelPlayerText(int i)
    {
        playerLevelText.text = $"LEVEL {i.ToString()}";
        playerLevelText2.text = $"LEVEL {i.ToString()}";
    }

    public void UpdateWaveDuration(float amount)
    {
        waveDurationJauge.fillAmount = amount;
    }

    public void UpdateWaveCount(int i)
    {
        waveCountText.text = $"WAVE {i.ToString()}";
        waveCountText2.text = $"WAVE {i.ToString()}";
    }
    
    public void SetPlayerLifeJauge(float f)
    {
        lifeImage.fillAmount = f;
    }

    public void SetLifePlayerText(int i)
    {
        lifeText.text = $"LIFE : {i.ToString()}";
        lifeText2.text = $"LIFE : {i.ToString()}";
    }

    public void SetGoldText(int i)
    {
        tokenText2.text = tokenText.text = i.ToString();
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


    private void Update()
    {
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
}
