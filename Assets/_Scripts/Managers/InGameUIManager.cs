using System.Collections.Generic;
using Helper;
using HubcapInterface;
using UnityEngine;

public class InGameUIManager : Singleton<InGameUIManager> {
    [Header("Sliders data")] 
    [SerializeField] private SliderManager lifeSlider = null;
    [SerializeField] private SliderManager nitroSlider = null;
    [SerializeField] private SliderManager levelSlider = null;
    [SerializeField] private SliderManager waveSlider = null;
    [SerializeField] private List<SliderManager> shotgunSliders = new();

    //Update Slider value
    public void UpdateLifeSlider(float amount) => lifeSlider.UpdateSliderAmount(amount);
    public void UpdateNitroSlider(float amount) => nitroSlider.UpdateSliderAmount(amount);
    public void UpdateLevelSlider(float amount) => levelSlider.UpdateSliderAmount(amount);
    public void UpdateWaveSlider(float amount) => waveSlider.UpdateSliderAmount(amount);
    public void UpdateShotgunSlider(float amount, int id) => shotgunSliders[id].UpdateSliderAmount(amount);
    //Init MinAmount Nitro Slider
    public void InitNitroSlider(float minAmount) => nitroSlider.UpdateDisableAmount(minAmount);
}
