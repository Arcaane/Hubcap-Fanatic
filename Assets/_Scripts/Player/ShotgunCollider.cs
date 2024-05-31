using HubcapAbility;
using UnityEngine;

public class ShotgunCollider : MonoBehaviour {
    [SerializeField] private ShotgunHandler shotgunHandler = null;
    
    /// <summary>
    /// If enemy enter in the trigger, add it to the list of targeted enemy
    /// </summary>
    /// <param name="enemy"></param>
    private void OnTriggerEnter(Collider enemy) {
        if (!enemy.CompareTag("Enemy")) return;
        
        if(enemy.GetComponent<ConvoyBehaviour>()) shotgunHandler.AddConvoy(enemy.transform);
        else shotgunHandler.AddEnemyCar(enemy.transform);
    }
    
    /// <summary>
    /// Try to remove an enemy from the list of targeted enemy
    /// </summary>
    /// <param name="enemy"></param>
    private void OnTriggerExit(Collider enemy) {
        shotgunHandler.RemoveEnemyCar(enemy.transform);
    }
}
