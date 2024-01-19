using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIIndic : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform camCenter;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private List<GameObject> obj;
    public List<GameObject> Obj => obj;
    [SerializeField] private Rect rectAdjusted;
    [SerializeField] private Vector2 center;
    [SerializeField] public List<TargetUI> targetUIPrefab;
    [SerializeField] private GameObject indic;
    [SerializeField] private float delayBeforeRemove = 5f;
    public float DelayBeforeRemove => delayBeforeRemove;
    public Transform uiParent;
    public static UIIndic instance;

    public Vector2 offset;

    public int currentIndex = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //Setup Rect    
        SetupRect();

        for (int i = 0; i < DeliveryRessourcesManager.Instance.deliveryPoints.Count; i++)
        {
            obj.Add(DeliveryRessourcesManager.Instance.deliveryPoints[i]);
        }
        
        //Setup 
        CreateIndicsAtStart(DeliveryRessourcesManager.Instance.deliveryPoints.Count);
        CreateIndicForConvoy();
        CreateIndicForMerchant();
    }
    
    void SetupRect()
    {
        rectAdjusted = rectTransform.rect;
        rectAdjusted.center = cam.pixelRect.center;
        center = cam.WorldToScreenPoint(camCenter.position);
    }

    public void Update()
    {
        int count = obj.Count;
        for (int i = count - 1; i >= 0; i--)
        {
            if (i < obj.Count && (obj[i] == null || obj[i].activeSelf == false))
            {
                RemoveIndic(i);
            }
            else if (i < obj.Count) 
            {
                UpdateIndic(i);
            }
        }
    }

    void CreateImages()
    {
        targetUIPrefab.Add(Instantiate(indic, Vector3.zero, quaternion.identity,uiParent).GetComponent<TargetUI>());
    }
    
    void CreateIndicsAtStart(int count, TargetType targetType = TargetType.DeliveryZone)
    {
        for (int i = 0; i < count; i++)
        {
            TargetUI deliveryZoneUI = Instantiate(indic, Vector3.zero, quaternion.identity, uiParent).GetComponent<TargetUI>();
            deliveryZoneUI.targetType = targetType;
            deliveryZoneUI.objBinded = obj[i];
            deliveryZoneUI.indexDeliveryPoints = i;
            targetUIPrefab.Add(deliveryZoneUI);
        }
        EnableOrDisableDeliveryZone();
    }
    
    void CreateIndicForConvoy()
    {
        TargetUI convoyUI = Instantiate(indic, Vector3.zero, quaternion.identity, uiParent).GetComponent<TargetUI>();
        convoyUI.targetType = TargetType.Convoy;
        convoyUI.objBinded = ConvoyManager.instance.gameObject;
        convoyUI.indexDeliveryPoints = 0;
        targetUIPrefab.Add(convoyUI);
        obj.Add(convoyUI.objBinded);
        EnableOrDisableSpecificUI(3);
    }
    
    void CreateIndicForMerchant()
    {
        TargetUI merchantUI = Instantiate(indic, Vector3.zero, quaternion.identity, uiParent).GetComponent<TargetUI>();
        merchantUI.targetType = TargetType.Merchant;
        merchantUI.objBinded = MerchantBehavior.instance.gameObject;
        merchantUI.indexDeliveryPoints = 0;
        targetUIPrefab.Add(merchantUI);
        obj.Add(merchantUI.objBinded);
        EnableOrDisableSpecificUI(4);
    }


    public void UpdateIndic(int indexObj)
    {
        if (targetUIPrefab[indexObj] != null)
        {
            if (rectAdjusted.Contains(cam.WorldToScreenPoint(obj[indexObj].transform.position)))
            {
                targetUIPrefab[indexObj].transform.localScale = Vector3.Lerp(targetUIPrefab[indexObj].transform.localScale, Vector3.zero, Time.deltaTime * 17);
            }
            else
            {
                targetUIPrefab[indexObj].transform.localScale = Vector3.Lerp(targetUIPrefab[indexObj].transform.localScale, Vector3.one * 1f * (Mathf.Sin(Time.time * 5 + indexObj) * 0.3f + 1), Time.deltaTime * 5);
                Vector3 objPosNear = camCenter.position + (obj[indexObj].transform.position - camCenter.position).normalized * 3;
                Vector2 pos = cam.WorldToScreenPoint(objPosNear);
                pos = FindPointOnRectBorder(pos - center, center, rectAdjusted);
                targetUIPrefab[indexObj].transform.position = pos;
            }
        }
    }

    public void EnableOrDisableSpecificUI(int index, bool enable = false)
    {
        targetUIPrefab[index].gameObject.SetActive(enable);
    }
    

    public void EnableOrDisableDeliveryZone(bool enable = false)
    {
        for (int i = 0; i < DeliveryRessourcesManager.Instance.deliveryPoints.Count; i++)
        {
            targetUIPrefab[i].gameObject.SetActive(enable);
        }
    }

    public void RemoveIndic(int indexObj)
    {
        obj.RemoveAt(indexObj);
        Destroy(targetUIPrefab[indexObj].gameObject);
        targetUIPrefab.RemoveAt(indexObj);
        //Debug.Log("INDIC REMOVED " + indexObj);
    }
    
    
    public void AddIndic(GameObject newObj, TargetType type, out int indicNb)
    {
        obj.Add(newObj);
        CreateImages();
        indicNb = obj.Count - 1;
        targetUIPrefab[indicNb].targetType = type;
        targetUIPrefab[indicNb].indexDeliveryPoints = indicNb;
        targetUIPrefab[indicNb].objBinded = newObj;
        //Debug.Log("INDIC CREATED "+ indicNb);
    }

    public Vector2 FindPointOnRectBorder(Vector2 dir,Vector2 center,Rect rect)
    {
        float angleSup = Vector2.SignedAngle(Vector2.up, rect.max - center);
        float angleInf = Vector2.SignedAngle(Vector2.up, new Vector2(rect.xMax,rect.yMin) - center);
        float angle = Vector2.SignedAngle(Vector2.up, dir);
        
        if (angle > angleSup && angle < -angleSup)
        {
            Vector2 intersection = Intersection(center, center + dir.normalized * 1000, new Vector2(rect.xMin, rect.yMax), new Vector2(rect.xMax, rect.yMax), out bool found);
            return intersection;
        }
        if (angle < angleInf || angle > -angleInf)
        {
            Vector2 intersection = Intersection(center, center + dir.normalized * 1000, new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMax, rect.yMin), out bool found);
            return intersection;
        }
        if (angle > 0)
        {
            Vector2 intersection = Intersection(center, center + dir.normalized * 1000, new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMin, rect.yMax), out bool found);
            return intersection;
        } 
        if (angle < 0)
        {
            Vector2 intersection = Intersection(center, center + dir.normalized * 1000, new Vector2(rect.xMax, rect.yMin), new Vector2(rect.xMax, rect.yMax), out bool found);
            return intersection;
        }
        return Vector2.zero;
    }
    
    
    public Vector2 Intersection(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2, out bool found)
    {
       
        float tmp = (B2.x - B1.x) * (A2.y - A1.y) - (B2.y - B1.y) * (A2.x - A1.x);
 
        if (tmp == 0)
        {
            // No solution!
            found = false;
            return Vector2.zero;
        }
 
        float mu = ((A1.x - B1.x) * (A2.y - A1.y) - (A1.y - B1.y) * (A2.x - A1.x)) / tmp;
 
        found = true;
 
        return new Vector2(
            B1.x + (B2.x - B1.x) * mu,
            B1.y + (B2.y - B1.y) * mu
        );
    }
}
