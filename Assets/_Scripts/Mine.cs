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
        
        GameObject b = PoolManager.instance.SpawnTemporaryInstance(Key.FX_Explosion, transform.position, Quaternion.identity, 5).gameObject;
        b.transform.localScale = new Vector3(explosionRadius, explosionRadius, explosionRadius);
        float dist = Vector3.Distance(transform.position, PlayerCarController.Instance.transform.position);
        if (dist < 30)
        {
            CameraShake.instance.SetShake(0.4f *( 1- dist/30));
        }
        
        var cols = Physics.OverlapSphere(transform.position, explosionRadius, enemyMask);
        for (int i = 0; i < cols.Length; i++) cols[i].GetComponent<IDamageable>()?.TakeDamage(damages);
        PoolManager.instance.DestroyInstance(Key.OBJ_Mine, this);
    }
}
