using System.Collections;
using System.Threading.Tasks;
using Abilities;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class CarHealthManager : MonoBehaviour, IDamageable
{
    public static CarHealthManager instance;

    [SerializeField] public int maxLifePoints = 100;
    [SerializeField] private int lifePoints = 100;
    [SerializeField] public float armorInPercent;

    [SerializeField] private Material[] mat;
    [SerializeField] private int feedbackDurationInMS = 300;

    [SerializeField] private WaveManager spawnManager;
    [SerializeField] private Volume volume;
    private Vignette vt;

    [SerializeField] private Image[] moveOnDeath;
    [SerializeField] private MeshRenderer renderer;
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        moveOnDeath[0].fillAmount = 1;
        moveOnDeath[1].fillAmount = 1;
        
        moveOnDeath[0].DOFillAmount(0, 0.35f);
        moveOnDeath[1].DOFillAmount(0, 0.35f);
        
        mat = new Material[renderer.materials.Length];
        for (int i = 0; i < mat.Length; i++)
        {
            mat[i] = new Material(renderer.materials[i]);
        }
        renderer.materials = mat;
        
        moveOnDeath[0].fillAmount = moveOnDeath[1].fillAmount = 0;
        lifePoints = maxLifePoints;
        UIManager.instance.SetPlayerLifeJauge((float)lifePoints / maxLifePoints);
        UIManager.instance.SetLifePlayerText(lifePoints);
        volume.profile.TryGet(out vt);
    }
    
    public void TakeDamage(int damages)
    {
        if (!IsDamageable()) return;

        var a = Mathf.FloorToInt(damages - (damages *  armorInPercent/100)); 
        lifePoints -= a;
        ActiveDamageFB();
        
        SetVignette();

        if (lifePoints < 1)
        {
            Death();
            lifePoints = 0;
        }
        
        UIManager.instance.UITakeDamage();
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
        //CarController.instance.forceBreak = true;
        //CarController.instance.forceBreakTimer = 15;
        CarController.instance.targetSpeed = 0;
        CarController.instance.maxSpeed = 0;
        CarController.instance.rb.mass = 100;
        
        spawnManager.dontSpawn = true;
        PoliceCarManager.Instance.CallOnPlayerDeath();
        diePS.gameObject.SetActive(true);
        diePS.Play();
        StartCoroutine(LoadScene());
        await Task.Delay(2500);
        explosionPS.Play();
        SaveGold();
        await Task.Delay(2300);
        // Ecran noir
        moveOnDeath[0].DOFillAmount(1, 0.35f);
        moveOnDeath[1].DOFillAmount(1, 0.35f).OnComplete(() => asyncOperation.allowSceneActivation = true);
        // Switch scene
    }

    public void TakeHeal(int i)
    {
        lifePoints += i;
        if (lifePoints > maxLifePoints) lifePoints = maxLifePoints;
        SetVignette();
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

    private void SaveGold()
    {
        if (!PlayerPrefs.HasKey("Gold")) PlayerPrefs.SetInt("Gold", 0);
        var currentGold = PlayerPrefs.GetInt("Gold");
        PlayerPrefs.SetInt("Gold", currentGold + CarAbilitiesManager.instance.goldAmountWonOnRun);
        PlayerPrefs.Save();
    }

    private void SetVignette()
    {
        ColorParameter colorParameter = new ColorParameter(Color.Lerp(Color.red, Color.black, (float)lifePoints / maxLifePoints), false);
        ClampedFloatParameter intensity =
            new ClampedFloatParameter(Mathf.Lerp(0.45f, 0.35f, (float) lifePoints / maxLifePoints),0,1);
        vt.color.SetValue(colorParameter);
        vt.intensity.SetValue(intensity);
    }
}