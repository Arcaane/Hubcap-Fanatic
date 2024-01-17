using System.Threading.Tasks;
using UnityEngine;

public class CarHealthManager : MonoBehaviour, IDamageable
{
    public static CarHealthManager instance;

    [SerializeField] private int maxLifePoints = 100;
    [SerializeField] private int lifePoints = 100;

    [SerializeField] private Material[] mat;
    [SerializeField] private int feedbackDurationInMS = 300;
    
    private void Start()
    {
        instance = this;
        lifePoints = maxLifePoints;
        UIManager.instance.SetPlayerLifeJauge((float)lifePoints / maxLifePoints);
        UIManager.instance.SetLifePlayerText(lifePoints);
    }

    [ContextMenu("DamagePlayerTestUI")]
    public void DamagePlayer()
    {
        var a = Random.Range(1, 5);
        TakeDamage(a);
    }
    
    public void TakeDamage(int damages)
    {
        if (!IsDamageable()) return;

        lifePoints -= damages;
        
        UIManager.instance.SetPlayerLifeJauge((float)lifePoints / maxLifePoints);
        UIManager.instance.SetLifePlayerText(lifePoints);
        ActiveDamageFB();
        
        if (lifePoints < 1) Death();
    }

    public bool IsDamageable() => gameObject.activeSelf;

    public void Death()
    {
        // TODO : DEFINIR LA FONCTION DE MORT DU JOUEUR
        // TODO - Explosion 
        // TODO - Reduce timeScale
    }

    private async void ActiveDamageFB()
    {
        foreach (var t in mat)
        {
            t.SetFloat("_UseDamage", 1);
        }

        await Task.Delay(feedbackDurationInMS);

        foreach (var t in mat)
        {
            if (instance)
            {
                t.SetFloat("_UseDamage", 0);
            }
        }
    }
}