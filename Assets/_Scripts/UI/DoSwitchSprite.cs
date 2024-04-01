using System.Collections;
using UnityEngine;

public class DoSwitchSprite : MonoBehaviour {
    [Header("Data Information")] 
    [SerializeField] private SliderManager slider = null;
    [Space]
    [SerializeField] private Sprite updatedSprite = null;
    [SerializeField] private float timeToSwitchbackSprite = 0.25f;
    
    public void DOSwitchSpriteEvent() {
        StartCoroutine(UpdateSpriteAfterTime());
    }

    /// <summary>
    /// Bring back the old sprite after waiting x seconds
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdateSpriteAfterTime() {
        slider.ChangeForceSprite(true, updatedSprite);
        yield return new WaitForSeconds(timeToSwitchbackSprite);
        slider.ChangeForceSprite(false);
    }
}
