using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SpawnZoneDelivery : MonoBehaviour
{
    [Header("Setup Zone Size Values")] [SerializeField]
    private float initSize = 40;

    [SerializeField] private float currentSize = 40;

    [Header("Zone State")] [SerializeField]
    public SpawnDeliveryState currenSpawnState = SpawnDeliveryState.IsOrNotDelivered;

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


    [Header("Reward Type")] [SerializeField]
    private GameObject packageToDeliver;


    [Header("------------------ Effects ------------------")] [SerializeField]
    private Transform capturePart;

    [SerializeField] private ParticleSystem effectOnCapture;
    [SerializeField] private List<ParticleSystem> smokeFX;

    [Header("Renderer")]
    [SerializeField] private RectTransform rect;
    [SerializeField] private Transform plane;

    [SerializeField] private ParticleSystem psDrop;

    private SphereCollider collider;

    [Header("---------- Debug Editor Values ----------")] [SerializeField]
    private float timer;

    //[SerializeField] private float timeDeliveryIncrement = 0f;
    public int index;

    private Vector2 randomPosition2D;
    private Vector3 randomPosition;

    private void Start()
    {
        timer = 0;
        SetupDeliveryZone();
        SetupDurationForeachSmoke();
        
        randomPosition2D = Random.insideUnitCircle * currentSize;
        randomPosition = new Vector3(randomPosition2D.x, 0f, randomPosition2D.y) + transform.position;
    }

    private void Update()
    {
        if (currenSpawnState == SpawnDeliveryState.Delivered) return;
        
        timer -= Time.deltaTime;
        
        plane.GetComponent<MeshRenderer>().material.SetFloat("_time", timer / deliveryDuration);

        if (timer < 0)
        {
            DeliveryZone(randomPosition);
        }
    }

    public void SetupDurationForeachSmoke()
    {
        foreach (ParticleSystem ps in smokeFX)
        {
            ps.Stop();

            var main = ps.main;
            main.duration = deliveryDuration - 1.0f;

            ps.Play();
            ps.gameObject.SetActive(true);
        }
    }


    public void SetupDeliveryZone()
    {
        currentSize = initSize;
        timer = deliveryDuration;
    }

    private void DeliveryZone(Vector3 randomPosition)
    {
        currenSpawnState = SpawnDeliveryState.Delivered;
        GivePlayerRessources(randomPosition);
    }
    
    private void GivePlayerRessources(Vector3 randomPosition)
    {
        psDrop.transform.position = randomPosition + new Vector3(0, 1.5f, 0);
        psDrop.Play();
        DisableZone();
        StartCoroutine(DelayedAction(() =>
        {
            GameObject spawnedObject = Instantiate(packageToDeliver, randomPosition + new Vector3(0, 1.5f, 0), Quaternion.identity);
            spawnedObject.transform.parent = PickableManager.Instance.worldSocket;
            hasDelivered = true;
        }, 0.5f));
    }

    private IEnumerator DelayedAction(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
    
    private void EnableZone()
    {
        foreach (Transform child in capturePart)
        {
            child.gameObject.SetActive(true);
        }
    }

    private void DisableZone()
    {
        foreach (Transform child in capturePart)
        {
            child.gameObject.SetActive(false);
        }

        //Reset Vector
        randomPosition2D = Vector2.zero;
        randomPosition = Vector3.zero;
        psDrop.Stop();

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
        Handles.color = Color.blue;
        Handles.DrawWireDisc(transform.position, Vector3.up, currentSize);
#endif
    }
}