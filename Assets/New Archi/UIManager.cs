using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] private Image nitroJauge;
    [SerializeField] private Transform straffJauge;
    public RadarDetectorUI radar;

    private void Awake()
    {
        instance = this;
    }

    public void SetNitroJauge(float amount)
    {
        nitroJauge.fillAmount = amount;
    }
    
    public void SetStraffJauge(float amount)
    {
        straffJauge.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, amount);
    }
}
