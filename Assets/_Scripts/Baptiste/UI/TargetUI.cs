using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetUI : MonoBehaviour
{
    [Header("Target Type")]
    [SerializeField] private TargetType targetType;
    [SerializeField] private List<Sprite> iconImages;
    
    [Header("UI Elements")]
    public Image targetImage;
    public Image backgroundImage;
    public TextMeshProUGUI distanceText;
    
    [Header("Debug")]
    [Range(0, 2000)]
    [SerializeField] private float distance = 0f;
    
    void Start()
    {
        SetText(distanceText, distance);
    }

    void Update()
    {
        SetText(distanceText, distance);
        SwitchIcon();
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
        }

        tmpGUI.text = adjustedNumber.ToString("0.#") + " " + unit;
    }

    void SwitchIcon()
    {
        switch (targetType)
        {
            case TargetType.DropZone:
                SetImage(targetImage, iconImages[0]);
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
}

[System.Serializable]
enum TargetType
{
    DropZone,
    DeliveryZone,
    ShopZone
}
