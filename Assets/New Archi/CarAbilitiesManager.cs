using System;
using UnityEngine;

public class CarAbilitiesManager : MonoBehaviour
{
    private static CarAbilitiesManager instance;
    public static CarAbilitiesManager Instance => instance;

    private void Awake()
    {
        instance = this;
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
}

public enum AbilitySocket
{
    AbilityNitro,
    AbilityDrift,
}
