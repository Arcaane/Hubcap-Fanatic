using System.Collections.Generic;
using ManagerNameSpace;
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
    [SerializeField] private int healToGive;
    [SerializeField] private GameObject zoneDebug;
    
    private void Start()
    {
        SetupAntennaValue();
    }

    private void Update()
    {
        switch (currentAntennaState)
        {
             case AntennaState.IsInactive: DeactivatePowerAntenna(); break;
             case AntennaState.IsBeingCaptured: BeingCaptured(); break; 
             case AntennaState.IsBeingLeaved: BeingLeaved(); break;
             case AntennaState.AntennaIsActivated: AntennaCaptured(); break;
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
        fillAmountMeshRenderer.materials[0].SetColor("_fillColor", new Color(0, 147, 255));

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
        fillAmountMeshRenderer.materials[0].SetFloat("_time", fillAmountValue);
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
            
            fillAmountMeshRenderer.materials[0].SetFloat("_time", fillAmountValue);
            timerLeaving = timeBeforeDecreasing;
            if (fillAmountValue < 0) fillAmountValue = 0;
        }
    }

    private void AntennaCaptured()
    {
        currentAntennaState =  AntennaState.AntennaIsActivated;
        CarHealthManager.instance.TakeHeal(healToGive);
        sCol.enabled = false;
        zoneDebug.SetActive(false);
        currentAntennaState = AntennaState.IsInactive;
        
        // FX
        GameObject go = PoolManager.instance.SpawnTemporaryInstance(Key.FX_PlayerGiveLife, transform.position, Quaternion.identity, 1.5f).gameObject;
        gameObject.SetActive(true);
    }
    
    private void TurnOnAntenna()
    {
        EnableAntennaTowerChild();
        SetupAntennaValue();
    }
    
    private void DeactivatePowerAntenna()
    {
        timeSinceCaptured += Time.deltaTime;
        fillAmountMeshRenderer.materials[0].SetColor("_fillColor", Color.Lerp(Color.red, Color.green, timeSinceCaptured / reactivateTimeDelay));
        fillAmountMeshRenderer.materials[0].SetFloat("_time", 1 - (timeSinceCaptured / reactivateTimeDelay));
        if (timeSinceCaptured >= reactivateTimeDelay)
        {
            TurnOnAntenna();
        }
    }
    
    private void EnableAntennaTowerChild()
    {
        sCol.enabled = true;
        zoneDebug.gameObject.SetActive(true);
        // Antenne réactivée
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
