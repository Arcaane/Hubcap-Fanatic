using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Rail : MonoBehaviour
{
    [Header("Curve")]
    [SerializeField] private List<RailPoint> railPoints;
    [SerializeField] private  float nbPoints;
    [SerializeField] private  float distBetweenNodes;
    private List<Vector3> pointsOnCurve =new List<Vector3>(0);
    public List<Vector3> distancedNodes = new List<Vector3>(0);
    [SerializeField] private bool loop;
    [SerializeField] private List<Transform> forms;

    [Header("Tool")] 
    [SerializeField] private bool generateRail;
    [SerializeField] private bool generatePlank;
    public bool addNewPoint;
    public bool removeLastPoint;
    [SerializeField] private GameObject railPoint;
    public bool modeForm;
    private bool form;
    [SerializeField] private GameObject formPoint;
    [SerializeField] private float gizmoSize = 0.1f;


    private void Start()
    {
        DrawRailPoints();
        CreateDistancedNodes();
    }
    

    private void OnDrawGizmos()
    {
        DrawRailPoints();
        CreateDistancedNodes();
        foreach (Vector3 pos in distancedNodes)
        {
            Gizmos.DrawSphere(pos,gizmoSize);
        }

        
        if (addNewPoint)
        {
            AddNewPoint();
            addNewPoint = false;
        }
        if (removeLastPoint)
        {
            RemoveLastPoint();
            removeLastPoint = false;
        }

        if (modeForm)
        {
            if (!form)
            {
                form = true;
                EnterFormMode();
            }

            FormMode();

        }
        else
        {
            if (form)
            {
                form = false;
                ExitFormMode();
            }
        }
    }

    void EnterFormMode()
    {
        for (int i = 0; i < railPoints.Count; i++)
        {
            GameObject obj = Instantiate(formPoint, railPoints[i].nextHandle.position, quaternion.identity, transform);
            obj.name = "PolygonEdge" + i;
            forms.Add(obj.transform);
        }
    }
    
    void ExitFormMode()
    {
        foreach (Transform obj in forms)
        {
            DestroyImmediate(obj.gameObject);
        }
        forms.Clear();
    }

    void FormMode()
    {
        for (int i = 0; i < railPoints.Count; i++)
        {
            railPoints[i].nextHandle.position = forms[i].position;
            if(i > 0)  railPoints[i].previousHandle.position = forms[i-1].position;
            else railPoints[i].previousHandle.position = forms[forms.Count-1].position;

            railPoints[i].point.position = (railPoints[i].nextHandle.position + railPoints[i].previousHandle.position) / 2;
        }
    }

    void AddNewPoint()
    {
        GameObject obj = Instantiate(railPoint, transform.position, quaternion.identity,transform);
        obj.name = "Point" + railPoints.Count;
        RailPoint newPoint = new RailPoint();
        newPoint.point = obj.transform;
        newPoint.previousHandle = obj.transform.GetChild(0);
        newPoint.nextHandle = obj.transform.GetChild(1);
        railPoints.Add(newPoint);
        if (modeForm)
        {
            GameObject objPoly = Instantiate(formPoint, railPoints[railPoints.Count-1].nextHandle.position, quaternion.identity, transform);
            objPoly.name = "PolygonEdge" + (railPoints.Count-1);
            forms.Add(objPoly.transform);
        }
    }
    
    void RemoveLastPoint()
    {
        DestroyImmediate(railPoints[railPoints.Count - 1].point.gameObject);
        railPoints.RemoveAt(railPoints.Count - 1);
    }

    

    int[] GetTrianglesForQuad(int a,int b,int c,int d)
    {
        List<int> triangles = new List<int>(0);
        triangles.Add(a);
        triangles.Add(b);
        triangles.Add(c);
        triangles.Add(b);
        triangles.Add(d);
        triangles.Add(c);
        return triangles.ToArray();
    }
    
    void CreateDistancedNodes()
    {
        if (distBetweenNodes <= 0) return;
        distancedNodes.Clear();
        distancedNodes.Add(railPoints[0].point.position);
        float totalDist = 0;
        for (int i = 1; i < pointsOnCurve.Count; i++)
        {
            totalDist += Vector3.Distance(pointsOnCurve[i], pointsOnCurve[i - 1]);
        }
        int numberOfNodes =  Mathf.RoundToInt(totalDist / distBetweenNodes);
        float distNode = totalDist / numberOfNodes;
        numberOfNodes--;

        int index = 1;
        Vector3 current = pointsOnCurve[0];
        for (int i = 0; i < numberOfNodes; i++)
        {
            if (Vector3.SqrMagnitude(pointsOnCurve[index] - current) < distNode * distNode)
            {
                float dist = distNode - Vector3.Distance(pointsOnCurve[index], current);
                index++;
                for (int j = 0; j < 500; j++)
                {
                    if (Vector3.SqrMagnitude(pointsOnCurve[index] - pointsOnCurve[index - 1]) < dist * dist)
                    {
                        dist -= Vector3.Distance(pointsOnCurve[index], pointsOnCurve[index - 1]);
                        index++;
                    }
                    else
                    {
                        Vector3 pos = pointsOnCurve[index-1] + (pointsOnCurve[index] - pointsOnCurve[index-1]).normalized * dist;
                        distancedNodes.Add(pos);
                        current = pos;
                        break;
                    }
                }
            }
            else
            {
                Vector3 pos = current + (pointsOnCurve[index] - current).normalized * distBetweenNodes;
                distancedNodes.Add(pos);
                current = pos;
            }
        }
        if(!loop)distancedNodes.Add(railPoints[railPoints.Count-1].point.position);
    }
    
    private void DrawRailPoints()
    {
        pointsOnCurve.Clear();
        for (int i = 0; i < railPoints.Count-1; i++)
        {
            DrawPoints(railPoints[i].point.position,railPoints[i].nextHandle.position,railPoints[i+1].previousHandle.position,railPoints[i+1].point.position);
        }
        if (loop)
        {
            DrawPoints(railPoints[railPoints.Count-1].point.position,railPoints[railPoints.Count-1].nextHandle.position,railPoints[0].previousHandle.position,railPoints[0].point.position);
            pointsOnCurve.Add(railPoints[0].point.position);
        }
        else
        {
            pointsOnCurve.Add(railPoints[railPoints.Count-1].point.position);   
        }
    }

    void DrawPoints(Vector3 a,Vector3 b,Vector3 c,Vector3 d)
    {
        for (int i = 0; i < nbPoints; i++)
        {
            Vector3 pos = QuadraticLerp(a, b, c, d, (1 / nbPoints) * i);
            pointsOnCurve.Add(pos);
        }
    }

    Vector3 DoubleLerp(Vector3 a,Vector3 b,Vector3 c,float t)
    {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);
        Vector3 abc = Vector3.Lerp(ab, bc, t);
        return abc;
    }

    Vector3 QuadraticLerp(Vector3 a,Vector3 b,Vector3 c,Vector3 d,float t)
    {
        Vector3 abc = DoubleLerp(a, b, c, t);
        Vector3 bcd = DoubleLerp(b, c, d, t);
        Vector3 quadratic = Vector3.Lerp(abc, bcd, t);
        return quadratic;
    }
}

[Serializable]
public class RailPoint
{
    public Transform point;
    public Transform previousHandle;
    public Transform nextHandle;
}
