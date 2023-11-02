using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Rigidbody playerRb;
    public Transform player;
    public CarControllerNoRb car;
    public Vector3 Offset;
    public float speed;
    
    // Update is called once per frame
    void LateUpdate()
    {
        //Vector3 playerForward = (playerRb.velocity + player.transform.forward).normalized;
        transform.position = Vector3.Lerp(transform.position, player.position + Offset, speed * Time.deltaTime);
        //transform.LookAt(player.position + playerForward);
    }
}