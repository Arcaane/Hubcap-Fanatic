using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SpawnZoneDelivery : MonoBehaviour
{
    [Header("Setup Zone Size Values")]  
    [SerializeField] private float initSize = 40;
    [SerializeField] private float currentSize = 40;
    [Header("Zone State")]
    [SerializeField] public SpawnDeliveryState currenSpawnState = SpawnDeliveryState.IsOrNotDelivered;
    [SerializeField] private bool hasDelivered = false;

    public bool HasDelivered
    {
        get { return hasDelivered; }
    }

    [SerializeField] private float deliveryDuration = 20f;

    public float DeliveryDuration
    {
        get => deliveryDuration;
        set => deliveryDuration = value;
    }

    [SerializeField] private float timeBeforeLaunchingDelivery = 5f;


    [Header("Reward Type")] 
    [SerializeField] private GameObject packageToDeliver;

    [Header("Renderer")] 
    [SerializeField] private Image debugImage;
    [SerializeField] private RectTransform rect;
    [SerializeField] private Transform plane;
    
    private SphereCollider collider;

    [Header("---------- Debug Editor Values ----------")] 
    [SerializeField] private float timer;
    [SerializeField] private float timeDeliveryIncrement = 0f;
    public int index;

    private void Start()
    {
        SetupDeliveryZone();
    }

    private void Update()
    {
        if (currenSpawnState == SpawnDeliveryState.Delivered)
        {
            timeDeliveryIncrement = 0;
        }
        else
        {
            timeDeliveryIncrement += Time.deltaTime;
            if (timeDeliveryIncrement >= timeBeforeLaunchingDelivery &&
                currenSpawnState == SpawnDeliveryState.IsOrNotDelivered)
            {
                timer -= Time.deltaTime;
                debugImage.fillAmount = 1 - (timer / deliveryDuration);
                if (timer < 0)
                {
                    DeliveryZone();
                }
            }
        }
    }

    public void SetupDeliveryZone()
    {
        currentSize = initSize;
        timer = deliveryDuration;

        rect.sizeDelta = new Vector2(currentSize * 2, currentSize * 2);
        plane.localScale = new Vector3(currentSize / 4.25f, currentSize / 4.25f, currentSize / 4.25f);

        //Reset timer 
        timeDeliveryIncrement = 0;
    }

    private void DeliveryZone()
    {
        currenSpawnState = SpawnDeliveryState.Delivered;
        GivePlayerReward();
    }

    private void GivePlayerReward()
    {
         GivePlayerRessources(); 
    }
    

    private void GivePlayerRessources()
    {
        Vector2 randomPosition2D = Random.insideUnitCircle * currentSize;
        Vector3 randomPosition = new Vector3(randomPosition2D.x, 0f, randomPosition2D.y) + transform.position;
        GameObject spawnedObject = Instantiate(packageToDeliver, randomPosition + new Vector3(0, 1.5f, 0), Quaternion.identity);
        spawnedObject.transform.parent = PickableManager.Instance.worldSocket;
        hasDelivered = true;
        DisableZone();
    }
    
    private void EnableZone()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    private void DisableZone()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        StartCoroutine(DisableWithDelay());
    }

    private IEnumerator DisableWithDelay()
    {
        yield return new WaitForSeconds(UIIndic.instance.DelayBeforeRemove);
        Destroy(gameObject);
    }


    private void OnDrawGizmos()
    {
        /*
         *
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, currentSize);
        */
#if UNITY_EDITOR
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.up, currentSize);
#endif
    }
}