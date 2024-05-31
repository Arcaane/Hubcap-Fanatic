using System.Collections;
using System.Collections.Generic;
using HubcapCarBehaviour;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetUI : MonoBehaviour
{
    [Header("Target Type")]
    [SerializeField] public TargetType targetType;
    [SerializeField] private List<Sprite> iconImages;
    [SerializeField] private Color[] iconColors;
    
    [Header("UI Elements")]
    public Image[] targetImage;
    public Image durationBeforeSpawnImage;
    public TextMeshProUGUI[] distanceText;
    
    [Header("Debug")]
    private float distance = 0f;
    [SerializeField] private float timer;
    public int indexDeliveryPoints = 0; 
    public GameObject objBinded;
    private float _deliveryDuration;
    
    void Start()
    {
        if (TargetType.DropZone == targetType)
        {
            _deliveryDuration = UIIndic.instance.Obj[indexDeliveryPoints].GetComponent<SpawnZoneDelivery>().DeliveryDuration;
        }
        SetText(Mathf.RoundToInt(distance).ToString());
        durationBeforeSpawnImage.fillAmount = 0;
    }

    void Update()
    {
        if(objBinded == null) return;
        SetText(Mathf.RoundToInt(distance).ToString());
        SwitchIcon();
        if (ConvoyManager.instance.currentConvoy != null && TargetType.Convoy == targetType)
        {
            objBinded = ConvoyManager.instance.currentConvoy.gameObject;
        }
        CalculateDistance();
    }

    void IncreaseFillAmount()
    {
        if (durationBeforeSpawnImage.fillAmount >= 1) return;
        timer += Time.deltaTime;
        durationBeforeSpawnImage.fillAmount = timer / _deliveryDuration;
    }

    void DecreaseFillAmount()
    {
        timer += Time.deltaTime;
        durationBeforeSpawnImage.fillAmount = 1 - (timer / _deliveryDuration);
    }
    
    void SetText(string text)
    {
        distanceText[0].text = distanceText[1].text = text + "M";
    }
    
    void SetText(TextMeshProUGUI tmpGUI, float number)
    {
        string unit = "m";
        float adjustedNumber = number;

        if (number > 999)
        {
            adjustedNumber = number / 1000f;
            unit = "km";
            tmpGUI.text = adjustedNumber.ToString("0.#") + " " + unit;
        }
        else
        {
            tmpGUI.text = adjustedNumber.ToString("0") + " " + unit;
        }
    }
    
    void SwitchIcon()
    {
        switch (targetType)
        {
            case TargetType.DropZone:
                SetImage( iconImages[0],iconColors[0]);
                IncreaseFillAmount();
                break;
            case TargetType.DeliveryZone:
                SetImage( iconImages[1],iconColors[1]);
                durationBeforeSpawnImage.fillAmount = 1;
                break;
            case TargetType.Merchant:
                SetImage( iconImages[2],iconColors[2]);
                durationBeforeSpawnImage.fillAmount = 1;
                break;
            case TargetType.CampZone:
                SetImage( iconImages[3],iconColors[3]);
                durationBeforeSpawnImage.fillAmount = 1;
                break;            
            case TargetType.Convoy:
                SetImage( iconImages[4],iconColors[4]);
                durationBeforeSpawnImage.fillAmount = 1;
                break;
        }
    }
    
    void SetImage(Sprite sprite,Color color)
    {
        targetImage[0].sprite = targetImage[1].sprite = durationBeforeSpawnImage.sprite = sprite;
        durationBeforeSpawnImage.color = color;
    }

    void CalculateDistance()
    {
        distance = Vector3.Distance(PlayerCarController.Instance.transform.position, objBinded.transform.position);
    }
}

[System.Serializable]
public enum TargetType
{
    DropZone,
    DeliveryZone,
    Merchant,
    CampZone,
    Convoy
}
