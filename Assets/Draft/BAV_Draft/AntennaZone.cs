using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class AntennaArea : MonoBehaviour
{
    [Header("Setup Zone Size Values")]  
    [SerializeField] private float initSize = 40;
    [SerializeField] private float antennaEffectSize = 40;
    
    [Header("Zone State")]
    [SerializeField] public AntennaState currentAntennaState;
    [SerializeField] private bool isCapturing;
    
    [Header("----------Setup Parameter Values ----------")]
    [Tooltip("Step 1.1 => Time to capture the antenna")]
    [SerializeField] private float captureDuration = 20f;
    [Tooltip("Step 1.2 => Time to before decreasing the capture value")]
    [SerializeField] private float timeBeforeDecreasing = 5f;
    [Tooltip("Step 2 => Duration of the time  to show indicator on the map")]
    [SerializeField] private float activationTowerDuration = 5f;
    [Tooltip("Step 3 => Time to reactivate the antenna after being captured and show indicator on the map")]
    [SerializeField] private float reactivateTimeDelay = 5f; 
    
    [Header("---------- Current Renderer Part ----------")]
    [SerializeField] private Image debugImage;
    //[SerializeField] private RectTransform rect;
    [SerializeField] private Transform plane;
    [SerializeField] private Transform parentFodder;

    private SphereCollider collider;
    [Header("---------- Debug Editor Values ----------")]
    [Header("Gizmos")]
    [SerializeField] private bool enabledGizmos;
    [SerializeField] private bool convoyIsInRange;
    [SerializeField] private bool merchantIsInRange;
    
    [Header("Timer Value")]
    [SerializeField] private float timerCapturing;
    [SerializeField] private float timerLeaving;
    [SerializeField] private float timerActivation;
    [SerializeField] private float timeSinceCaptured = 0f;
    
    [Header("Fill Amount")]
    [SerializeField] private float fillAmountValue = 0f;
    private float currentSize = 0f;
    
    [SerializeField] private List<int> indexList = new List<int>();
    
    [SerializeField] private SphereCollider sCol;
    [SerializeField] private MeshRenderer fillAmountMeshRenderer;
    
    private void Start()
    {
        SetupAntennaValue();
    }

    private void Update()
    {
        switch (currentAntennaState)
        {
             case AntennaState.IsInactive:
                    DeactivatePowerAntenna();
                 break;
                case AntennaState.IsBeingCaptured:
                    BeingCaptured();
                 break;
                case AntennaState.IsBeingLeaved:
                    BeingLeaved();
                 break;
                case AntennaState.AntennaIsActivated:
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
    }
    
    private void BeingCaptured()
    {
        timerLeaving = 0;
        timerCapturing -= Time.deltaTime;
        fillAmountValue = 1 - (timerCapturing / captureDuration);
        //fillAmountMeshRenderer.material.SetFloat("");
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
        AntennaDiscoverEffect();
        
        timerActivation += Time.deltaTime;
        if (timerActivation >= activationTowerDuration)
        {
            currentAntennaState = AntennaState.IsInactive;
            timerActivation = 0;
        }
    }

    private void AntennaDiscoverEffect()
    {
        CheckDistanceToConvoy(); //Convoy
        CheckDistanceToMerchand(); //Merchant  
    }
    

    private void CheckDistanceToTarget(Transform targetTransform, TargetType targetType, ref bool targetIsInRange, List<int> indexList)
    {
        if (targetTransform != null && !targetIsInRange)
        {
            Vector3 antennaPosition = transform.position;
            Vector3 targetPosition = targetTransform.position;
            float distance = Vector3.Distance(antennaPosition, targetPosition);

            if (distance < antennaEffectSize)
            {
                targetIsInRange = true;
            }
        }
    }

    private void CheckDistanceToConvoy()
    {
        if (ConvoyManager.instance?.currentConvoy != null)
        {
            CheckDistanceToTarget(ConvoyManager.instance.currentConvoy.transform, TargetType.Convoy,
                ref convoyIsInRange, indexList);
        }
        if(convoyIsInRange) UIIndic.instance.EnableOrDisableSpecificUI(3, true);
    }

    private void CheckDistanceToMerchand()
    {
        CheckDistanceToTarget(MerchantBehavior.instance?.gameObject?.transform, TargetType.Merchant, ref merchantIsInRange, indexList);
        if(merchantIsInRange) UIIndic.instance.EnableOrDisableSpecificUI(4, true);
    }

    private void TurnOnAntenna()
    {
        EnableAntennaTowerChild();
        SetupAntennaValue();
    }

    
    private void DeactivatePowerAntenna()
    {
        if (convoyIsInRange)
        {
            UIIndic.instance.EnableOrDisableSpecificUI(3);
            convoyIsInRange = false;
        }
        if (merchantIsInRange)
        {
            UIIndic.instance.EnableOrDisableSpecificUI(4);
            merchantIsInRange = false;
        }
        
        timeSinceCaptured += Time.deltaTime;
        if (timeSinceCaptured >= reactivateTimeDelay)
        {
            TurnOnAntenna();
        }
    }
    
    
    private void EnableAntennaTowerChild()
    {
        gameObject.GetComponent<SphereCollider>().enabled = true;
        foreach (Transform child in parentFodder)
        {
            child.gameObject.SetActive(true);
        }
    }

    private void DisableAntennaTowerChild()
    {
        gameObject.GetComponent<SphereCollider>().enabled = false;
        foreach (Transform child in parentFodder)
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
        if (!enabledGizmos) return;
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.up,  initSize);

        Handles.color = Color.green;
        Handles.DrawWireDisc(transform.position, Vector3.up,  antennaEffectSize);
    }
#endif
}
