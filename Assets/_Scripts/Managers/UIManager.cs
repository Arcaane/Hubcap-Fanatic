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
    
    [Header("Wave Informations")]
    [SerializeField] private Image waveDurationJauge;
    [SerializeField] private TextMeshProUGUI waveCountText;

    [SerializeField] private Transform straffJauge;
    

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
    }

    public void UpdateWaveDuration(float amount)
    {
        waveDurationJauge.fillAmount = amount;
    }

    public void UpdateWaveCount(int i)
    {
        waveCountText.text = i.ToString();
    }

    public void SetStraffJauge(float amount)
    {
        straffJauge.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, amount);
    }
}
