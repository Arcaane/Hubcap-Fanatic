using UnityEngine;

public class SpearObject : MonoBehaviour
{
    public float speed;

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.forward * (speed * Time.deltaTime);
        transform.position += pos;
    }

    private void OnCollisionEnter(Collision other)
    {
        other.transform.GetComponent<IDamageable>()?.TakeDamage(100);
        Destroy(gameObject);
    }
}
