using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CapturedZone : MonoBehaviour
{
    [SerializeField] private float initSize = 40;
    [SerializeField] private float currentSize = 40;
    [SerializeField] public ZoneState currentZoneState;
    [SerializeField] private bool isCapturing;
    [SerializeField] private int currencyToGive;
    [SerializeField] private float captureDuration = 20f;
    [SerializeField] private float percentReduceCaptureSizeIfPlayerGoOut = 10;
    [SerializeField] private Image debugImage;
    [SerializeField] private RectTransform rect;
    [SerializeField] private Transform plane;

    private SphereCollider collider;
    [SerializeField] private float timer;
    
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
        if (currentZoneState == ZoneState.CapturedOrNotAccesible) return;
        if (isCapturing)
        {
            timer -= Time.deltaTime;
            debugImage.fillAmount = 1 - (timer / captureDuration);
            if (timer < 0)
            {
                CaptureZone();
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
       GameObject spawnedObject = DeliveryRessourcesManager.Instance.SpawnObject(CarPickableManager.Instance._pickableSocket.position);
       CarPickableManager.Instance.AddPickableObject(spawnedObject);
       spawnedObject.transform.parent = CarPickableManager.Instance._pickableSocket;
       this.gameObject.SetActive(false); // TODO : Disable and re-enable the zone after delay
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
            ReduceZoneSize();
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

    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.up, currentSize);
    }
}

public enum ZoneState
{
    NotCaptured,
    CapturedOrNotAccesible
}
