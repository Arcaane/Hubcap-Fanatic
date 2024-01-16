using UnityEngine;

public class SpearAbility : Ability
{
    public GameObject spearPrefab;
    public Collider[] cols;
    public LayerMask enemyLayerMask;

    public override void StartAbility()
    {
        base.StartAbility();
        cooldownTimer = 0;
    }

    public override void UpdateAbility()
    {
        if (activable) return;
        
        cooldownTimer += Time.deltaTime;
        if (cooldownTimer > cooldown)
        {
            LaunchSpear();
        } 
    }

    private void LaunchSpear()
    {
        var carPos = CarController.instance.transform.position;
        cooldownTimer = 0;
        
        float tempShorterDst = 10000;
        int shorterIndex = -1;
        
        cols = Physics.OverlapSphere(carPos, 150, enemyLayerMask);
        if (cols.Length > 0)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                var tempDst = Vector3.Distance(carPos, cols[i].transform.position);
                
                if (tempDst < tempShorterDst)
                {
                    tempShorterDst = tempDst;
                    shorterIndex = i;
                }
            }
            
            Vector3 relativePos = carPos - cols[shorterIndex].transform.position;
            Instantiate(spearPrefab, CarController.instance.transform.position, Quaternion.LookRotation(-relativePos));
        }
    }
    
    // EFFECTS
    
    public int damageValue;
    public float effectDurationValue;
    
    private void EffectSpear(GameObject target)
    {
        var carPos = CarController.instance.transform.position;
        Vector3 relativePos = carPos - target.transform.position;
        Instantiate(spearPrefab, carPos, Quaternion.LookRotation(-relativePos));
    }
    
    private void EffectDamage(GameObject target)
    {
        IDamageable damageable = target.GetComponent<IDamageable>();
        damageable.TakeDamage(damageValue);
    }
    
    private void EffectForceBreak(GameObject target)
    {
        CarBehaviour carBehaviour = target.GetComponent<CarBehaviour>();
        carBehaviour.forceBreak = true;
        carBehaviour.forceBreakTimer = effectDurationValue;
    }
    
}
