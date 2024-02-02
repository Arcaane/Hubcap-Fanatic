using ManagerNameSpace;
using UnityEngine;

public class SpearObject : MonoBehaviour
{
    public float speed;
    public int damages;

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 pos = transform.forward * (speed * Time.fixedDeltaTime);
        transform.position += pos;
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.transform.CompareTag("Enemy")) other.transform.GetComponent<IDamageable>()?.TakeDamage(damages);
        
        if(other.transform.CompareTag("Wall")) Pooler.instance.DestroyInstance(Key.OBJ_Spear,transform.parent);
    }
}
