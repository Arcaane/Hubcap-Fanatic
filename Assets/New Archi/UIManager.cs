using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] private Image nitroJauge;
    public RadarDetectorUI radar;

    private void Awake()
    {
        instance = this;
    }

    public void SetNitroJauge(float amount)
    {
        nitroJauge.fillAmount = amount;
    }
}
