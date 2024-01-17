using ManagerNameSpace;
using UnityEngine;

public class CarHealthManager : MonoBehaviour, IDamageable
{
    public static CarHealthManager instance;

    [SerializeField] private int maxLifePoints = 100;
    [SerializeField] private int lifePoints = 100;
    [SerializeField] private GameObject damageText;

    private void Start()
    {
        instance = this;
        lifePoints = maxLifePoints;
        UIManager.instance.SetPlayerLifeJauge((float)lifePoints / maxLifePoints);
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
        
        if (lifePoints < 1) Death();
    }

    public bool IsDamageable() => gameObject.activeSelf;

    public void Death()
    {
        // TODO : DEFINIR LA FONCTION DE MORT DU JOUEUR
        // TODO - Explosion 
        // TODO - Reduce timeScale
    }
}