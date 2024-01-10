using UnityEngine;

public class WheelVisualController : MonoBehaviour
{
    [Header("Wheel Visuals")] 
    public GameObject wheelMesh;
    public Vector2 minMaxSteerAngle = new Vector2(-30, 30);
    public float steerAngle;
    [Range(-1, 1)]
    public float wheelRadius = 0.5f;
    public float rotationSpeed = 10.0f;
    
    [Header("Spark  Visuals")]
    public GameObject sparkPrefab;

    // Start is called before the first frame update
    void Start()
    {
        RotateWheel();
    }

#if UNITY_EDITOR
    // Update is called once per frame in the editor
    void Update()
    {
        RotateWheel();
    }
#endif

    void RotateWheel()
    {
        float targetSteerAngle = Mathf.Lerp(minMaxSteerAngle.x, minMaxSteerAngle.y, (wheelRadius + 1) / 2);
        wheelMesh.transform.localRotation = Quaternion.Euler(wheelMesh.transform.localRotation.x, targetSteerAngle, 90);
        sparkPrefab.transform.localRotation = Quaternion.Euler(0, targetSteerAngle, 0);
        wheelMesh.transform.Rotate(Vector3.up * Time.time * rotationSpeed);
    }
}