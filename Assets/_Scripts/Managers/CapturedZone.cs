using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CapturedZone : MonoBehaviour
{
    [SerializeField] private float initSize = 40;
    [SerializeField] private float currentSize = 40;
    [SerializeField] public ZoneState currentZoneState;
    [SerializeField] private bool isCapturing;
    public bool IsCaptured { get { return isCapturing; } }
    [SerializeField] private int currencyToGive;
    [SerializeField] private float captureDuration = 20f;
    [SerializeField] private float reactivateDelay = 5f; 
    [SerializeField] private float percentReduceCaptureSizeIfPlayerGoOut = 10;
    [SerializeField] private Image debugImage;
    [SerializeField] private RectTransform rect;
    [SerializeField] private Transform plane;
    
    [Header("Reward Type")]
    [SerializeField] private RewardType rewardType;
    

    
    private SphereCollider collider;
    [Header("------------------ Debug Values ------------------")]
    [SerializeField] private float timer;
    [SerializeField] private float timeSinceCaptured = 0f;
    
    private void Start()
    {
        currentSize = initSize;
        timer = captureDuration;
        
        collider = GetComponent<SphereCollider>();
        collider.radius = currentSize;
        rect.sizeDelta = new Vector2(currentSize * 2, currentSize * 2);
        plane.localScale = new Vector3(currentSize / 4.25f, currentSize / 4.25f, currentSize / 4.25f);
        GetComponent<Collider>().isTrigger = true;
    }

    private void Update()
    {
        if (currentZoneState == ZoneState.CapturedOrNotAccesible)
        {
            timeSinceCaptured += Time.deltaTime;
            Debug.Log("Hello");
            if (timeSinceCaptured >= reactivateDelay)
            {
                EnableTower();
                currentZoneState = ZoneState.NotCaptured;
                currentSize = initSize;
                timer = captureDuration;
                isCapturing = false;
                debugImage.fillAmount = 0;


                collider.radius = currentSize;
                rect.sizeDelta = new Vector2(currentSize * 2, currentSize * 2);
                plane.localScale = new Vector3(currentSize / 4.25f, currentSize / 4.25f, currentSize / 4.25f);
                timeSinceCaptured = 0f;
            }
        }
        else
        {
            if (isCapturing)
            {
                timer -= Time.deltaTime;
                debugImage.fillAmount = 1 - (timer / captureDuration);
                if (timer < 0)
                {
                    CaptureZone();
                    DisableTower();
                }
            }    
        }
    }

    private void CaptureZone()
    {
        currentZoneState = ZoneState.CapturedOrNotAccesible;
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
                  //GivePlayerRessources();
                    break;
       }
    }

    private void GiveIncrementValue(ref int value)
    {
        value++;
    }

    /*
     * 

    private void GivePlayerRessources()
    {
        GameObject spawnedObject = DeliveryRessourcesManager.Instance.SpawnObject(PickableManager.Instance.carPickableSocket.position);
        PickableManager.Instance.AddPickableObject(spawnedObject);
        spawnedObject.transform.parent = PickableManager.Instance.carPickableSocket;
        DisableTower();
    }
    */
    
    private void EnableTower()
    {
        gameObject.GetComponent<SphereCollider>().enabled = true;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    private void DisableTower()
    {
        gameObject.GetComponent<SphereCollider>().enabled = false;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        if (currentZoneState == ZoneState.NotCaptured)
        {
            isCapturing = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        if (currentZoneState == ZoneState.NotCaptured)
        {
            //ReduceZoneSize();
            isCapturing = false;
            timer = captureDuration;
            debugImage.fillAmount = 0;
        }
    }

    private void ReduceZoneSize()
    {
        currentSize *= 1 - (percentReduceCaptureSizeIfPlayerGoOut / 100);
        
        collider.radius = currentSize;
        rect.sizeDelta = new Vector2(currentSize * 2, currentSize * 2);
        plane.localScale = new Vector3(currentSize / 4.25f, currentSize / 4.25f, currentSize / 4.25f);
        
        if (currentSize < 5)
        {
            currentZoneState = ZoneState.CapturedOrNotAccesible;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.up, currentSize);
        #endif
    }
#endif
    
}

public enum ZoneState
{
    NotCaptured,
    CapturedOrNotAccesible
}

