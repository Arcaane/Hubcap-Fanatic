using System.Threading.Tasks;
using UnityEngine;

public class PointsGiver : MonoBehaviour
{
    public int powerToGive;
    

    public HowToGive howToGive = HowToGive.byPercent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBonusCollider"))
        {
            Debug.Log("Collision");
            
            CarControllerNoRb player = other.transform.root.GetComponent<CarControllerNoRb>();
            
            if (howToGive == HowToGive.byPoints)
            {
                player.GivePowerByNumber(powerToGive);
            }
            else
            {
                player.GivePowerByPercent(powerToGive);
            }

            ResetActive();
        }
    }

    private async void ResetActive()
    {
        gameObject.SetActive(false);
        await Task.Delay(10000);
        gameObject.SetActive(true);
    }
}

public enum HowToGive 
{
    byPoints,
    byPercent
}
