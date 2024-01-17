using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
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

    [SerializeField] public Transform shootIcon;

    [SerializeField] private Image[] shotJauges;

    [SerializeField] public AbilitiesPair[] abilitiesSlots;

    [Serializable]
    public struct AbilitiesPair
    {
        public Image passiveAbilityIcon;
        public Image statAbilityIcon;
    }

    [Header("Wave Informations")]
    [SerializeField] private TextMeshProUGUI merchantText;
    [SerializeField] private TextMeshProUGUI merchantText2;
    

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetExperienceFillAmount(0);
        SetTokenText(CarExperienceManager.Instance.levelUpTokensAvailable);
    }

    public void SetNitroJauge(float amount)
    {
        nitroJauge.fillAmount = amount;
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
        Debug.Log(f);
        lifeImage.fillAmount = f;
    }

    public void SetLifePlayerText(int i)
    {
        lifeText.text = $"LIFE : {i.ToString()}";
        lifeText2.text = $"LIFE : {i.ToString()}";
    }

    public void SetTokenText(int i)
    {
        tokenText.text = i.ToString();
        tokenText2.text = i.ToString();
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
        await Task.Delay(3000);
        merchantText.gameObject.SetActive(false);
        merchantText2.gameObject.SetActive(false);
    }
}
