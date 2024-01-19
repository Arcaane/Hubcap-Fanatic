using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CarHealthManager : MonoBehaviour, IDamageable
{
    public static CarHealthManager instance;

    [SerializeField] private int maxLifePoints = 100;
    [SerializeField] private int lifePoints = 100;

    [SerializeField] private Material[] mat;
    [SerializeField] private int feedbackDurationInMS = 300;

    [SerializeField] private WaveManager spawnManager;
    [SerializeField] private Volume volume;
    [SerializeField] private Vignette vt;
    
    private void Start()
    {
        instance = this;
        lifePoints = maxLifePoints;
        UIManager.instance.SetPlayerLifeJauge((float)lifePoints / maxLifePoints);
        UIManager.instance.SetLifePlayerText(lifePoints);
        volume.profile.TryGet(out vt);
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
        ActiveDamageFB();
        
        ColorParameter colorParameter = new ColorParameter(Color.Lerp(Color.red, Color.white, (float)lifePoints / maxLifePoints), false);
        vt.color.SetValue(colorParameter);

        if (lifePoints < 1)
        {
            Death();
            lifePoints = 0;
        }
        
        UIManager.instance.SetPlayerLifeJauge((float)lifePoints / maxLifePoints);
        UIManager.instance.SetLifePlayerText(lifePoints);
    }

    private bool isDead;
    public bool IsDamageable() => !isDead;

    [SerializeField] private ParticleSystem diePS;
    [SerializeField] private ParticleSystem explosionPS;
    private async void Death()
    {
        
        isDead = true;
        CarController.instance.forceBreak = true;
        spawnManager.dontSpawn = true;
        PoliceCarManager.Instance.CallOnPlayerDeath();
        diePS.gameObject.SetActive(true);
        diePS.Play();
        await Task.Delay(2500);
        explosionPS.Play();
        await Task.Delay(2300);
        // Ecran noir
        
        // Switch scene
        
    }

    public void TakeHeal(int i)
    {
        lifePoints += i;
        UIManager.instance.SetPlayerLifeJauge((float)lifePoints / maxLifePoints);
        UIManager.instance.SetLifePlayerText(lifePoints);
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