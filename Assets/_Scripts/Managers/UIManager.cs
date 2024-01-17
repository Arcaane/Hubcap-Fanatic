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

    [Header("Player vitals")] [SerializeField]
    private Image lifeImage;
    [SerializeField] private TextMeshProUGUI lifeText;
    [SerializeField] private TextMeshProUGUI lifeText2;
    
    [SerializeField] public ButtonsItems[] buttonsHandlers;

    public Transform shootIcon;

    public Image[] shotJauges;
    
    [Header("Wave Informations")]
    [SerializeField] private TextMeshProUGUI merchantText;
    [SerializeField] private TextMeshProUGUI merchantText2;
    

    private void Awake()
    {
        instance = this;
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
        playerLevelText.text = i.ToString();
        playerLevelText2.text = i.ToString();
    }

    public void UpdateWaveDuration(float amount)
    {
        waveDurationJauge.fillAmount = amount;
    }

    public void UpdateWaveCount(int i)
    {
        waveCountText.text = i.ToString();
    }

    // public void SetStraffJauge(float amount)
    // {
    //     straffJauge.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, amount);
    // }

    public void SetPlayerLifeJauge(float f)
    {
        lifeImage.fillAmount = f;
    }

    public void SetLifePlayerText(int i)
    {
        lifeText.text = i.ToString();
        lifeText2.text = i.ToString();
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
