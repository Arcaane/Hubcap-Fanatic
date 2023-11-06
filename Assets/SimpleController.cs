using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleController : MonoBehaviour
{
    public float speed = 1.5f;
    
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            transform.position += Vector3.forward * speed * Time.deltaTime;
        }
        
        if(Input.GetKeyDown(KeyCode.Q))
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
        }
        
        if(Input.GetKeyDown(KeyCode.S))
        {
            transform.position += Vector3.back * speed * Time.deltaTime;
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
        }
    }
}
