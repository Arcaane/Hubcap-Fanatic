using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetUI : MonoBehaviour
{
    [Header("Target Type")]
    [SerializeField] public TargetType targetType;
    [SerializeField] private List<Sprite> iconImages;
    
    [Header("UI Elements")]
    public Image targetImage;
    public Image durationBeforeSpawnImage;
    public Image backgroundImage;
    public TextMeshProUGUI distanceText;
    
    [Header("Debug")]
    private float distance = 0f;
    [SerializeField] private float timer;
    public int indexDeliveryPoints = 0; 
    
    void Start()
    {
        SetText(distanceText, distance);
        durationBeforeSpawnImage.fillAmount = 0;
    }

    void Update()
    {
        SetText(distanceText, distance);
        SwitchIcon();
        CalculateDistance(targetType, indexDeliveryPoints);
    }

    void IncreaseFillAmount()
    {
        timer += Time.deltaTime;
        durationBeforeSpawnImage.fillAmount = 1 - (timer / UIIndic.instance.Obj[indexDeliveryPoints].GetComponent<SpawnZoneDelivery>().DeliveryDuration);    }

    void DecreaseFillAmount()
    {
        timer += Time.deltaTime;
        durationBeforeSpawnImage.fillAmount = 1 - (timer / UIIndic.instance.Obj[indexDeliveryPoints].GetComponent<SpawnZoneDelivery>().DeliveryDuration);
    }
    
    void SetText(TextMeshProUGUI tmpGUI, string text)
    {
        tmpGUI.text = text + "m";
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
                SetImage(targetImage, iconImages[0]);
                IncreaseFillAmount();
                break;
            case TargetType.DeliveryZone:
                SetImage(targetImage, iconImages[1]);
                break;
            case TargetType.ShopZone:
                SetImage(targetImage, iconImages[2]);
                break;
        }
    }
    
    void SetImage(Image image, Sprite sprite)
    {
        image.sprite = sprite;
    }

    void CalculateDistance(TargetType targetType, int index = 0)
    {
        switch (targetType)
        {
            case TargetType.DropZone:
                distance = Vector3.Distance(CarController.instance.transform.position, UIIndic.instance.Obj[indexDeliveryPoints].transform.position);
                break;
            case TargetType.DeliveryZone:
                distance = Vector3.Distance(CarController.instance.transform.position, UIIndic.instance.Obj[indexDeliveryPoints].transform.position);
                break;
            case TargetType.ShopZone:
                break;
        }
    }
}

[System.Serializable]
public enum TargetType
{
    DropZone,
    DeliveryZone,
    ShopZone
}
