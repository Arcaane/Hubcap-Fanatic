using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SpawnZoneDelivery : MonoBehaviour
{
    [SerializeField] private float initSize = 40;
    [SerializeField] private float currentSize = 40;
    [SerializeField] public SpawnDeliveryState currenSpawnState = SpawnDeliveryState.IsOrNotDelivered;
    [SerializeField] private bool hasDelivered = false;
    public bool HasDelivered { get { return hasDelivered; } }
    [SerializeField] private int currencyToGive;
    [SerializeField] private float deliveryDuration = 20f;
    [SerializeField] private float timeBeforeLaunchingDelivery = 5f; 
    
    
    [Header("Reward Type")]
    [SerializeField] private RewardType rewardType;

    [Header("Renderer")]
    [SerializeField] private Image debugImage;
    [SerializeField] private RectTransform rect;
    [SerializeField] private Transform plane;

    
    private SphereCollider collider;
    [Header("Debug Values")]
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
            if (timeDeliveryIncrement >= timeBeforeLaunchingDelivery && currenSpawnState == SpawnDeliveryState.IsOrNotDelivered)
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
        currenSpawnState =  SpawnDeliveryState.Delivered;
        GivePlayerReward();
    }

    private void GivePlayerReward()
    {
       Debug.Log("Player drop " + currencyToGive + " scraps!");
       switch (rewardType)
       {
           case RewardType.IncrementValue:
               GiveIncrementValue(ref currencyToGive);
               break;
              case RewardType.ObjectDelivery:
                  GivePlayerRessources();
                    break;
       }
    }

    private void GiveIncrementValue(ref int value)
    {
        value++;
    }
    
    private void GivePlayerRessources()
    {
        Vector2 randomPosition2D = Random.insideUnitCircle * currentSize;
        Vector3 randomPosition = new Vector3(randomPosition2D.x, 0f, randomPosition2D.y) + transform.position;
        GameObject spawnedObject = DeliveryRessourcesManager.Instance.SpawnObject(randomPosition);
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
        UIIndic.instance.RemoveIndic(index);
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
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
