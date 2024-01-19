using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using DG.Tweening;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class CarHealthManager : MonoBehaviour, IDamageable
{
    public static CarHealthManager instance;

    [SerializeField] private int maxLifePoints = 100;
    [SerializeField] private int lifePoints = 100;

    [SerializeField] private Material[] mat;
    [SerializeField] private int feedbackDurationInMS = 300;

    [SerializeField] private WaveManager spawnManager;
    [SerializeField] private Volume volume;
    private Vignette vt;

    [SerializeField] private Image[] moveOnDeath;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        moveOnDeath[0].fillAmount = moveOnDeath[1].fillAmount = 0;
        lifePoints = maxLifePoints;
        UIManager.instance.SetPlayerLifeJauge((float)lifePoints / maxLifePoints);
        UIManager.instance.SetLifePlayerText(lifePoints);
        volume.profile.TryGet(out vt);
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
        CarController.instance.forceBreakTimer = 15;
        
        spawnManager.dontSpawn = true;
        PoliceCarManager.Instance.CallOnPlayerDeath();
        diePS.gameObject.SetActive(true);
        diePS.Play();
        StartCoroutine(LoadScene());
        await Task.Delay(2500);
        explosionPS.Play();
        await Task.Delay(2300);
        // Ecran noir
        moveOnDeath[0].DOFillAmount(1, 0.35f);
        moveOnDeath[1].DOFillAmount(1, 0.35f).OnComplete(() => asyncOperation.allowSceneActivation = true);
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


    private AsyncOperation asyncOperation;
    IEnumerator LoadScene()
    {
        yield return null;
        asyncOperation = SceneManager.LoadSceneAsync("PROTO_WE_Start");
        asyncOperation.allowSceneActivation = false;
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }
}