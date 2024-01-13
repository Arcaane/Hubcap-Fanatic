using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UIIndic : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform camCenter;
    [SerializeField] private List<GameObject> obj;
    [SerializeField] private Rect rectAdjusted;
    [SerializeField] private Vector2 center;
    [SerializeField] private List<Image> image;
    [SerializeField] private GameObject indic;
    public Transform uiParent;
    [SerializeField] private Sprite[] sprites;
    public static UIIndic instance;
    public Color[] colors;


    private void Awake()
    {
        instance = this;
    }

    public void Update()
    {
        for (int i = 0; i < obj.Count; i++)
        {
            if (obj[i] == null || obj[i].activeSelf == false)
            {
                RemoveIndic(i);
            }
            else
            {
                
                UpdateIndic(i);
            }
        }
    }

    void CreateImages(int sprite,int color)
    {
        
        image.Add(Instantiate(indic, Vector3.zero, quaternion.identity,uiParent).transform.GetChild(1).GetComponent<Image>());
        image[image.Count - 1].sprite = sprites[sprite];
        image[image.Count - 1].color = colors[color];
        image[image.Count - 1].gameObject.name = sprite.ToString();
        image[image.Count - 1].transform.parent.localScale = Vector3.one*0.5f;
        
    }

    public void UpdateIndic(int indexObj)
    {
        if (rectAdjusted.Contains(cam.WorldToScreenPoint(obj[indexObj].transform.position)))
        {
            image[indexObj].transform.parent.localScale = Vector3.Lerp(image[indexObj].transform.parent.localScale,Vector3.zero,Time.deltaTime*17);
        }
        else
        {
            image[indexObj].transform.parent.localScale = Vector3.Lerp(image[indexObj].transform.parent.localScale,Vector3.one*0.5f *(Mathf.Sin(Time.time*5+indexObj)*0.3f+1),Time.deltaTime*5);
            Vector3 objPosNear = camCenter.position + (obj[indexObj].transform.position - camCenter.position).normalized*3;
            Vector2 pos = cam.WorldToScreenPoint(objPosNear);
            pos = FindPointOnRectBorder( pos - center,center, rectAdjusted);
            image[indexObj].transform.parent.position = pos;    
        }
    }

    public void RemoveIndic(int indexObj)
    {
        
        Destroy(image[indexObj].transform.parent.gameObject);
        image.RemoveAt(indexObj);
        
        obj.RemoveAt(indexObj);
    }
    
    public void ChangeColor(int indexObj,int color)
    {
        
        image[indexObj].color = colors[color];
        
    }
    
    public void AddIndic(GameObject newObj,int sprite,int color, out int indicNb)
    {
        obj.Add(newObj);
        CreateImages(sprite,color);
        indicNb = obj.Count - 1;
        Debug.Log("INDIC CREATED "+ indicNb);
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
