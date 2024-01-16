using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AntennaArea : MonoBehaviour
{
    [Header("Setup Zone Size Values")]  
    [SerializeField] private float initSize = 40;
    [SerializeField] private float currentSize = 40;
    
    [Header("Zone State")]
    [SerializeField] public AntennaState currentAntennaState;
    [SerializeField] private bool isCapturing;
    
    [Header("Setup Parameters Values")]  
    [Tooltip("Step 1.1 => Time to capture the antenna")]
    [SerializeField] private float captureDuration = 20f;
    [Tooltip("Step 1.2 => Time to before decreasing the capture value")]
    [SerializeField] private float timeBeforeDecreasing = 5f;
    [Tooltip("Step 2 => Duration of the time  to show indicator on the map")]
    [SerializeField] private float activationTowerDuration = 5f;
    [Tooltip("Step 3 => Time to reactivate the antenna after being captured and show indicator on the map")]
    [SerializeField] private float reactivateTimeDelay = 5f; 
    
    [SerializeField] private Image debugImage;
    [SerializeField] private RectTransform rect;
    [SerializeField] private Transform plane;

    private SphereCollider collider;
    [Header("---------- Debug Editor Values ----------")]
    [Header("Timer Value")]
    [SerializeField] private float timerCapturing;
    [SerializeField] private float timerLeaving;
    [SerializeField] private float timeSinceCaptured = 0f;
    [Header("Fill Amount")]
    [SerializeField] private float fillAmountValue = 0f;
    
    private void Start()
    {
        //Get Collider
        collider = GetComponent<SphereCollider>();
        GetComponent<Collider>().isTrigger = true;
        SetupAntennaValue();    
    }

    private void Update()
    {
        switch (currentAntennaState)
        {
             case AntennaState.IsInactive:
                 timeSinceCaptured += Time.deltaTime;
                 if (timeSinceCaptured >= reactivateTimeDelay)
                 {
                     EnableAntennaTowerChild();
                     SetupAntennaValue();
                 }
                 break;
                case AntennaState.IsBeingCaptured:
                    BeingCaptured();
                 break;
                case AntennaState.IsBeingLeaved:
                    BeingLeaved();
                 break;
                case AntennaState.AntennaIsActivated:
                    Debug.Log("Hello");
                    AntennaCaptured();
                 break;
        }
    }
    private void SetupAntennaValue()
    {
        //Reset All Values
        currentSize = initSize;
        timerCapturing = captureDuration;
        currentAntennaState = AntennaState.IsBeingLeaved;
        fillAmountValue = 0;
        timeSinceCaptured = 0f;
        isCapturing = false;

        //Set Size For the Rendering Part
        if (collider != null)
        {
            collider.radius = currentSize;   
        }
        if (debugImage != null)
        {
            debugImage.fillAmount = fillAmountValue;
        }

        if (rect != null)
        {
            rect.sizeDelta = new Vector2(currentSize * 2, currentSize * 2);
        }
        if (plane != null)
        {
            plane.localScale = new Vector3(currentSize / 4.25f, currentSize / 4.25f, currentSize / 4.25f);
        }
    }
    
    private void BeingCaptured()
    {
        timerLeaving = 0;
        timerCapturing -= Time.deltaTime;
        fillAmountValue = 1 - (timerCapturing / captureDuration);
        debugImage.fillAmount = fillAmountValue;
        if (timerCapturing < 0)
        {
            currentAntennaState = AntennaState.AntennaIsActivated;
            timerCapturing = 0;
        }
    }
    
    private void BeingLeaved()
    {
        timerLeaving += Time.deltaTime;
        if (timerLeaving > timeBeforeDecreasing && Mathf.Abs(fillAmountValue) > 0.001f)
        {
            timerCapturing += Time.deltaTime;
            fillAmountValue = 1 - (timerCapturing / captureDuration);
            if (timerCapturing >= captureDuration)
            {
                timerCapturing = captureDuration;
                fillAmountValue = 0;
            }

            debugImage.fillAmount = fillAmountValue;
            timerLeaving = timeBeforeDecreasing;
            if (fillAmountValue < 0) fillAmountValue = 0;
        }
    }

    private void AntennaCaptured()
    {
        currentAntennaState =  AntennaState.AntennaIsActivated;
        AntennaDiscoverMap();
    }

    private void AntennaDiscoverMap()
    {
        DisableAntennaTowerChild();
    }
    
    private void EnableAntennaTowerChild()
    {
        gameObject.GetComponent<SphereCollider>().enabled = true;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    private void DisableAntennaTowerChild()
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
        
        if (currentAntennaState == AntennaState.IsBeingLeaved)
        {
            currentAntennaState = AntennaState.IsBeingCaptured;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (currentAntennaState == AntennaState.IsBeingCaptured)
        {
            currentAntennaState = AntennaState.IsBeingLeaved;
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
