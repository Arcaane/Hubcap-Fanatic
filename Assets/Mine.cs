using ManagerNameSpace;
using UnityEngine;

public class Mine : MonoBehaviour
{
    public int damages;
    public float explosionRadius;
    public LayerMask enemyMask;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;
        
        GameObject b = Pooler.instance.SpawnTemporaryInstance(Key.FX_Explosion, transform.position, Quaternion.identity, 5).gameObject;
        b.transform.localScale = new Vector3(explosionRadius, explosionRadius, explosionRadius);
        
        var cols = Physics.OverlapSphere(transform.position, explosionRadius, enemyMask);
        for (int i = 0; i < cols.Length; i++) cols[i].GetComponent<IDamageable>()?.TakeDamage(damages);
        Pooler.instance.DestroyInstance(Key.OBJ_Mine, this);
    }
}
