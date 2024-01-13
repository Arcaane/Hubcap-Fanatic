using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayInformations : MonoBehaviour
{
    [Header("Display Informations")] 
    public TextMeshProUGUI currentWaveText;
    public TextMeshProUGUI convoyKillsNumberText;
    public TextMeshProUGUI ennemyKillsNumberText;
    public TextMeshProUGUI deliverysCompleteNumberText;

    private void Start()
    {
        UpdateDisplayInformations();
    }

    public void UpdateDisplayInformations()
    {
        SetText(currentWaveText, "Wave ");
        SetText(convoyKillsNumberText, "Convoy Kills Count : ");
        SetText(ennemyKillsNumberText, "Enemys Kills Count :");
        SetText(deliverysCompleteNumberText, "Deliverys Complete Count :");
    }
    
    private TextMeshProUGUI SetText(TextMeshProUGUI tmpGUI, string text)
    {
        tmpGUI.text = text;
        return tmpGUI;
    }
}
