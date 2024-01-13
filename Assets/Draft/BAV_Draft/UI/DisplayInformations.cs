using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayInformations : MonoBehaviour
{
    [Header("Display Informations")] 
    public List<TextMeshProUGUI> informationToDisplayTexts;
    
    private void Start()
    {
        UpdateDisplayInformations();
    }

    public void UpdateDisplayInformations()
    {
        SetText(informationToDisplayTexts[0], "Wave ");
        SetText(informationToDisplayTexts[1], "Convoy Kills Count : ");
        SetText(informationToDisplayTexts[2], "Enemys Kills Count :");
        SetText(informationToDisplayTexts[3], "Deliverys Complete Count :");
    }
    
    private TextMeshProUGUI SetText(TextMeshProUGUI tmpGUI, string text)
    {
        tmpGUI.text = text;
        return tmpGUI;
    }
}
