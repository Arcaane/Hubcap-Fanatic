using UnityEngine;

public class BoundsOutTrigger : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<OutOfBoundsManager>().RegisterCollider(gameObject);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<OutOfBoundsManager>().UnregisterCollider(gameObject);
        }
    }
}
