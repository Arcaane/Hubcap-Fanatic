using System;
using UnityEngine;

public delegate void AbilitiesDelegate();

public class CarAbilitiesManager : MonoBehaviour
{
    private static CarAbilitiesManager instance;
    public static CarAbilitiesManager Instance => instance;
    
    public AbilitiesDelegate OnBrake;
    //public AbilitiesDelegate OnNitro;
    //public AbilitiesDelegate OnDrift;
    
    private void Awake()
    {
        instance = this;
        OnBrake += CurseExplodeOnDeath;
    }

    //[Header("KIT")]
    [SerializeField] private Ability[] nitroAbilities;
    [SerializeField] private Ability[] driftAbilities;
    [SerializeField] public int damageOnCollisionWithEnemy;
    public void ActivateDriftAbilities()
    {
        foreach (var t in driftAbilities)
        {
            //if (!t.activable) return;
            t.StartAbility();
        }
    }
    
    public void DesactivateDriftAbilities()
    {
        foreach (var t in driftAbilities)
        {
            //if (!t.activable) return;
            t.StopAbility();
        }
    }
    
    public void ActivateNitroAbilities()
    {
        foreach (var t in nitroAbilities)
        {
            //if (!t.activable) return;
            t.StartAbility();
        }
    }
    
    public void DesactivateNitroAbilities()
    {
        foreach (var t in nitroAbilities)
        {
            //if (!t.activable) return;
            t.StopAbility();
        }
    }
    
    private void Update()
    {
        foreach (var t in nitroAbilities)
        {
            t.UpdateAbility();
        }

        foreach (var t in driftAbilities)
        {
            t.UpdateAbility();
        }

        // if (Input.GetKeyDown(KeyCode.U))
        // {
        //     foreach (var t in nitroAbilities)
        //     {
        //         t.activable = false;
        //     }
        // }
        //
        // if (Input.GetKeyUp(KeyCode.U))
        // {
        //     foreach (var t in nitroAbilities)
        //     {
        //         t.activable = true;
        //     }
        // }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) // EnemyCollision
        {
            if (Vector3.Dot( other.transform.position - transform.position, transform.forward) > 0.75f)
            {
                Debug.Log("Enemy Touché");
                other.GetComponent<IDamageable>()?.TakeDamage(damageOnCollisionWithEnemy);
            }
        }
    }


    public LayerMask enemyLayerMask;

    public CarAbilitiesManager(AbilitiesDelegate onBrake)
    {
        OnBrake = onBrake;
    }

    #region Curse

    public void MakeCurseExplodeOnDeath()
    {
        var cols = Physics.OverlapSphere(GetComponent<Transform>().position, 5f, enemyLayerMask);
        if (cols.Length <= 0) return;
        foreach (var t in cols)
        {
            PoliceCarBehavior police = t.GetComponent<PoliceCarBehavior>();
            
            // if (police.OnPoliceCarDie.)
            // {
            //     
            // }
            // t.GetComponent<PoliceCarBehavior>().OnPoliceCarDie += CurseExplodeOnDeath;
        }
    }
    
    private void CurseExplodeOnDeath()
    {
        var cols = Physics.OverlapSphere(GetComponent<Transform>().position, 5f, enemyLayerMask);
        if (cols.Length <= 0) return;
        for (int i = 0; i < cols.Length; i++)
        {
            cols[i].GetComponent<IDamageable>()?.TakeDamage(50);
            Debug.Log($"{cols[i].name} took damage");
        }
    }
    
    #endregion
    
}

public enum AbilitySocket
{
    AbilityNitro,
    AbilityDrift,
}
