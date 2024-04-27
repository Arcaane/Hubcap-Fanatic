using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class MerchantBehavior : MonoBehaviour
{
    public NavMeshAgent agent;
    public PathStop[] stops;

    public static MerchantBehavior instance;

    public int waitTimeAtPoints = 5000;
    private bool waiting;

    public TestShop shop;
    
    
    [Header("FollowPath")]
    public int previous,next;
    public float speed,targetSpeed, travelSpeed,index;
    public List<Vector3> distancedNodes;
    public float completion;
    public float upVector = 1.2f;
    public float lastFrameCompletion;

    private void Awake()
    {
        instance = this;
    }


    void FixedUpdate()
    {
        completion += Time.fixedDeltaTime * speed;
        if (completion >= 1)
        {
            completion--;
        }

        speed = Mathf.Lerp(speed, targetSpeed, Time.fixedDeltaTime * 2);
        
        index = Mathf.Lerp(0, distancedNodes.Count, completion);
        previous = Mathf.FloorToInt(index);
        next = Mathf.CeilToInt(index)%distancedNodes.Count;
        index -= previous;
        
        transform.position = Vector3.Lerp(distancedNodes[previous], distancedNodes[next], index)+Vector3.up*upVector;
        transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(distancedNodes[next] - distancedNodes[previous]),Time.deltaTime*5);
        
        
        for (int i = 0; i < stops.Length; i++)
        {
            if (lastFrameCompletion < stops[i].stop && completion >= stops[i].stop)
            {
                WaitAtPoint(stops[i]);
            }
        }
        
        lastFrameCompletion = completion;
    }
    

    private async void WaitAtPoint(PathStop stop)
    {
        
        targetSpeed = 0;
        UIManager.Instance.UpdateMerchantNotif("MERCHANT PASSED BY " + stop.stopName);
        await Task.Delay(Mathf.RoundToInt(1000 * stop.stopDuration));
        targetSpeed = travelSpeed;
        
    }
    
    [Serializable]
    public struct PathStop
    {
        public float stop;
        public string stopName;
        public float stopDuration;
    }
}
