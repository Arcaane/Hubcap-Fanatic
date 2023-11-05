using UnityEngine;

public class SmoothUpDownMovement : MonoBehaviour
{
    public float amplitude = 1.0f; // Adjust the amplitude of the movement
    public float frequency = 1.0f; // Adjust the frequency of the movement

    private Vector3 initialPosition;

    void Start()
    {
        // Store the initial position of the object
        initialPosition = transform.position;
    }

    void Update()
    {
        // Calculate the vertical offset based on time
        float yOffset = amplitude * Mathf.Sin(frequency * Time.time);

        // Update the object's position with the smooth up-and-down movement
        transform.position = initialPosition + new Vector3(0, yOffset, 0);
    }
}

