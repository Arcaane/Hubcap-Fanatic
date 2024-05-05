namespace HubcapInterface {
    public class SliderManager : BaseSliderClass {
        public override void UpdateSliderAmount(float amount) {
            fillAmount = amount;
            UpdateFillAmount();
            UpdateSpriteImage();
            UpdateTextData();
        }
    }
}