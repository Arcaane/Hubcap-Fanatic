using System.Collections;
using System.Threading.Tasks;
using Abilities;
using HubcapCarBehaviour;
using HubcapManager;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class CarHealthManagerOld : MonoBehaviour, IDamageable
{
    public static CarHealthManagerOld instance;

    [SerializeField] public int maxLifePoints = 100;
    public int Lifepoints => lifePoints;
    [SerializeField] private int lifePoints = 100;
    [SerializeField] public float armorInPercent;

    [SerializeField] private Material[] mat;
    [SerializeField] private int feedbackDurationInMS = 300;

    [SerializeField] private WaveManager spawnManager;
    [SerializeField] private Volume volume;
    private Vignette vt;
    
    [SerializeField] private MeshRenderer renderer;
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        mat = new Material[renderer.materials.Length];
        for (int i = 0; i < mat.Length; i++)
        {
            mat[i] = new Material(renderer.materials[i]);
        }
        renderer.materials = mat;
        
        lifePoints = maxLifePoints;
        UIManager.Instance.UpdateLifeSlider((float)lifePoints / maxLifePoints, lifePoints);
        volume.profile.TryGet(out vt);
    }

    public bool TakeDamage(int damage) {
        if (!IsDamageable()) return false;

        var a = Mathf.FloorToInt(damage - (damage * armorInPercent / 100));
        lifePoints -= a;
        ActiveDamageFB();

        SetVignette();

        UIManager.Instance.UITakeDamage();
        UIManager.Instance.UpdateLifeSlider((float) lifePoints / maxLifePoints, lifePoints);

        if (lifePoints >= 1) return false;
        
        Death();
        lifePoints = 0;
        return true;
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
        PlayerCarController.Instance.targetSpeed = 0;
        PlayerCarController.Instance.maxRoadSpeed = 0;
        //*PlayerCarController.Instance.rb.mass = 100;*//

        spawnManager.DisableWavesSpawn();
        PoliceCarManager.Instance.CallOnPlayerDeath();
        diePS.gameObject.SetActive(true);
        diePS.Play();
        MemoryForVictoryScreen.instance.waveCount = WaveManager.Instance.currentWaveCount;
        MemoryForVictoryScreen.instance.victory = false;
        DontDestroyOnLoad(MemoryForVictoryScreen.instance.gameObject);
        
        StartCoroutine(LoadScene());
        await Task.Delay(2500);
        explosionPS.Play();
        SaveGold();
        await Task.Delay(2300);

        UIManager.Instance.BlackScreenDeath(asyncOperation);
        // Switch scene
    }

    public void Victory()
    {
        StartCoroutine(LoadScene());
        
        UIManager.Instance.BlackScreenDeath(asyncOperation);
        // Switch scene
    }
    
    public void TakeHeal(int i)
    {
        lifePoints += i;
        if (lifePoints > maxLifePoints) lifePoints = maxLifePoints;
        SetVignette();
        UIManager.Instance.UpdateLifeSlider((float)lifePoints / maxLifePoints, lifePoints);
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
        asyncOperation = SceneManager.LoadSceneAsync(2);
        asyncOperation.allowSceneActivation = false;
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }

    private void SaveGold()
    {
        GameMaster.instance.AddGold(CarAbilitiesManager.instance.GoldAmountWonOnRun);
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