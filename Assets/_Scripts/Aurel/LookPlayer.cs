using UnityEngine;

public class LookPlayer : MonoBehaviour
{
    Transform playerTransform;
    private DecorTarget arrowTargetSystem;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = FindObjectOfType<CarDash>().transform;
        arrowTargetSystem = GetComponentInChildren<DecorTarget>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!arrowTargetSystem.IsInCooldown)
            transform.LookAt(playerTransform);
    }
}
