using DG.Tweening;
using UnityEngine;

public class RotatePlatform : MonoBehaviour
{
    public float speed;
    public bool isRotating;

    private void Start()
    {
        isRotating = true;
    }

    void Update()
    {
        if (!isRotating) return;
        transform.Rotate(0,speed * Time.deltaTime,0);
    }

    public void RotateForward()
    {
        isRotating = false;
        transform.DORotate(new Vector3(0, 68, 0), 1f);
    }

    public void ToRotate(Vector3 vec)
    {
        transform.DORotate(vec, 1f);
    }
}
