using System;
using UnityEngine;

public class CarHealthManager : MonoBehaviour , IDamageable
{
    public static CarHealthManager instance;
    
    [SerializeField] private int maxLifePoints = 100;
    [SerializeField] private int lifePoints = 100;

    private void Start()
    {
        instance = this;
        lifePoints = maxLifePoints;
    }

    public void TakeDamage(int damages)
    {
        lifePoints -= damages;
        //GameManager.instance.uiManager.SetHealthJauge((float) lifePoints / maxLifePoints);
        if(lifePoints < 1) Death();
    }

    public void Death()
    {
        // TODO : DEFINIR LA FONCTION DE MORT DU JOUEUR
        // TODO - Explosion 
        // TODO - Reduce timeScale
    }

    public GUIStyle style;
    
    private void OnGUI()
    {
        GUI.Label(new Rect(50, 150, 500, 150), "Press F1 to reload scene");
        GUI.Label(new Rect(50, 50, 350, 150), $"Life: {lifePoints}/{maxLifePoints}", style);
    }
}
