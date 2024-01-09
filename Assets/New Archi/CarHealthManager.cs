using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarHealthManager : MonoBehaviour , IDamageable
{
    [SerializeField] private int maxLifePoints = 100;
    private int lifePoints = 100;

    public void TakeDamage(int damages)
    {
        lifePoints -= damages;
        GameManager.instance.uiManager.SetHealthJauge((float) lifePoints / maxLifePoints);
        if(lifePoints <= 0) Death();
    }

    public void Death()
    {
        // TODO : DEFINIR LA FONCTION DE MORT DU JOUEUR
    }
}
