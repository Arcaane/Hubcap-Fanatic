using HubcapCarBehaviour;
using HubcapManager;
using UnityEngine;

public class Mine : MonoBehaviour {
    [SerializeField, Pooler] private string mineKey = "";
    [SerializeField, Pooler] private string explosionKey = "";
    public int damages;
    public float explosionRadius;
    public LayerMask enemyMask;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;
        
        GameObject b = PoolManager.Instance.RetrieveOrCreateObject(explosionKey, transform.position, Quaternion.identity);
        b.transform.localScale = new Vector3(explosionRadius, explosionRadius, explosionRadius);
        float dist = Vector3.Distance(transform.position, PlayerCarController.Instance.transform.position);
        if (dist < 30)
        {
            CameraShake.instance.SetShake(0.4f *( 1- dist/30));
        }
        
        var cols = Physics.OverlapSphere(transform.position, explosionRadius, enemyMask);
        foreach (Collider col in cols) col.GetComponent<IDamageable>()?.TakeDamage(damages);
        PoolManager.Instance.RetrieveOrCreateObject(mineKey, transform.position, Quaternion.identity);
    }
}
