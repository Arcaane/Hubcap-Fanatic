using UnityEngine;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif


[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class ProceduralRoad : MonoBehaviour
{
    [System.Serializable]
    public struct BezierCurve
    {
        public Transform parent;
        public List<Transform> controlPositions;
    }

    public List<BezierCurve> controlCurves = new List<BezierCurve>();
    public int resolution = 10;
    public float roadWidth = 5f;
    public float offsetBetweenPoints = 5f;
    
    [Header("Gizmos")]
    public bool showGizmos = true;

    private Mesh roadMesh;
    private MeshFilter meshFilter;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        roadMesh = new Mesh();
        meshFilter.mesh = roadMesh;

        CreateRoadMesh();
    }

    void Update()
    {
        CreateRoadMesh();
    }

    bool CheckControlCurvesChanged()
    {
        foreach (BezierCurve curve in controlCurves)
        {
            // Vérifiez si les positions de chaque courbe sont définies
            if (curve.controlPositions == null || curve.controlPositions.Count != 4)
            {
                Debug.LogError("Four control points must be assigned for each curve!");
                return false;
            }
        }

        return true;
    }

void CreateRoadMesh()
{
    List<Vector3> allVertices = new List<Vector3>();
    List<int> allTriangles = new List<int>();

    for (int c = 0; c < controlCurves.Count; c++)
    {
        if (controlCurves[c].controlPositions.Count != 4)
        {
            Debug.LogError("Four control points must be assigned for each curve!");
            return;
        }

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);
            Vector3 point = CalculateBezierPoint(t,
                controlCurves[c].controlPositions[0].localPosition,
                controlCurves[c].controlPositions[1].localPosition,
                controlCurves[c].controlPositions[2].localPosition,
                controlCurves[c].controlPositions[3].localPosition);

            // Calculate the scale of the road at the current point
            float scale = Mathf.Lerp(controlCurves[c].controlPositions[0].localScale.x,
                controlCurves[c].controlPositions[3].localScale.x, t);

            // Calculate the rotation of the road at the current point
            Quaternion rotation = Quaternion.Lerp(controlCurves[c].controlPositions[0].localRotation,
                controlCurves[c].controlPositions[3].localRotation, t);

            // Calculate vertices for the road
            float halfWidth = roadWidth * 0.5f * scale;
            vertices.Add(point + rotation * (Vector3.Cross(Vector3.up, point.normalized).normalized * halfWidth));
            vertices.Add(point + rotation * (-Vector3.Cross(Vector3.up, point.normalized).normalized * halfWidth));

            // Set up triangles
            if (i < resolution - 1)
            {
                int vertIndex = i * 2;
                triangles.Add(vertIndex);
                triangles.Add(vertIndex + 1);
                triangles.Add(vertIndex + 2);

                triangles.Add(vertIndex + 2);
                triangles.Add(vertIndex + 1);
                triangles.Add(vertIndex + 3);
            }
        }

        // Add the vertices and triangles of the current curve to the overall lists
        allVertices.AddRange(vertices);
        allTriangles.AddRange(triangles.Select(index => index + allVertices.Count - vertices.Count));
    }

    roadMesh.Clear();
    roadMesh.vertices = allVertices.ToArray();
    roadMesh.triangles = allTriangles.ToArray();
    roadMesh.RecalculateNormals();
}


    void UpdateControlPointsRelativeToParent(BezierCurve curve)
    {
        for (int i = 0; i < 4; i++)
        {
            curve.controlPositions[i].localPosition =
                curve.parent.InverseTransformPoint(curve.controlPositions[i].position);
        }
    }

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }
    
    [ContextMenu("Rebuild Mesh")]
    void RebuildMesh()
    {
        CreateRoadMesh();
        meshFilter.mesh = roadMesh;
    }

    [ContextMenu("Assign mesh to collider")]
    void AssignMeshToCollider() {
        GetComponent<MeshCollider>().sharedMesh = GetComponent<MeshFilter>().sharedMesh;
    }
    
    
    [ContextMenu("Clear Curve")]
    void ClearCurve()
    {
        meshFilter.mesh = null;
        List<BezierCurve> curvesCopy = new List<BezierCurve>(controlCurves);

        foreach (BezierCurve curve in curvesCopy)
        {
            foreach (Transform child in curve.parent)
            {
                DestroyImmediate(child.gameObject);
            }
            DestroyImmediate(curve.parent.gameObject);
        }
        controlCurves.Clear();
    }

    [ContextMenu("Clear Mesh")]
    void ClearMesh()
    {
        meshFilter.mesh = null;
    }


    [ContextMenu("Add A Curve")]
    void AddACurve()
    {
        GameObject curveParent = CreateCurveParent();

        BezierCurve newCurve = new BezierCurve
        {
            controlPositions = new List<Transform>(),
            parent = curveParent.transform
        };

        if (controlCurves.Count > 0)
        {
            BezierCurve lastCurve = controlCurves[controlCurves.Count - 1];
            Vector3 lastPosition = lastCurve.controlPositions[3].position;
            Vector3 lastForward = lastCurve.controlPositions[3].forward;

            for (int i = 0; i < 4; i++)
            {
                Vector3 controlPointPosition = GetDefaultControlPointPosition(i, offsetBetweenPoints, lastForward) + lastPosition;
                GameObject newControlPoint = CreateControlPoint(i, curveParent.transform, controlPointPosition, Quaternion.LookRotation(lastForward), lastCurve.controlPositions[3].localScale);
                newCurve.controlPositions.Add(newControlPoint.transform);
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject newControlPoint = CreateControlPoint(i, curveParent.transform, GetDefaultControlPointPosition(i, offsetBetweenPoints, Vector3.forward), Quaternion.identity, Vector3.one);
                newCurve.controlPositions.Add(newControlPoint.transform);
            }
        }

        UpdateControlPointsRelativeToParent(newCurve);
        controlCurves.Add(newCurve);
        CreateRoadMesh();
    }

    Vector3 GetDefaultControlPointPosition(int index, float offset = 1.0f, Vector3 orientation = default)
    {
        if (index == 0)
        {
            return Vector3.zero + new Vector3(0, 0, 0.001f);
        }
        else
        {
            return orientation.normalized * (index * offset);
        }
    }


   
    GameObject CreateCurveParent()
    {
        GameObject curveParent = new GameObject("BezierCurveParent");
        curveParent.transform.position = transform.position;
        curveParent.transform.parent = transform;
        return curveParent;
    }

    GameObject CreateControlPoint(int index, Transform parent, Vector3 localPosition, Quaternion rotation, Vector3 scale)
    {
        GameObject newControlPoint = new GameObject($"Control Point {index}");
        newControlPoint.transform.parent = parent;
        newControlPoint.transform.localPosition = localPosition;
        newControlPoint.transform.localRotation = rotation;
        newControlPoint.transform.localScale = scale;
        return newControlPoint;
    }

    Vector3 GetControlPointOffset(int index)
    {
        return new Vector3(0, 0, index * 5f);
    }
    
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!showGizmos) return;
        foreach (BezierCurve curve in controlCurves)
        {
            Gizmos.color = Color.green;

            for (float t = 0; t <= 1; t += 0.05f)
            {
                Vector3 point = CalculateBezierPoint(t, curve.controlPositions[0].position, curve.controlPositions[1].position, curve.controlPositions[2].position, curve.controlPositions[3].position);
                Gizmos.DrawSphere(point, 0.1f);
            }

            Gizmos.color = Color.red;
        }
    }
#endif
}

