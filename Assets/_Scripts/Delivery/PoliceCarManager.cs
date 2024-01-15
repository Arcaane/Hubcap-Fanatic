using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceCarManager : MonoBehaviour
{
    private static PoliceCarManager _instance;
    public static PoliceCarManager Instance => _instance;
    
    [Header("Police Cars in the Scene")]
    [SerializeField] private List<PoliceCarBehavior> policeCars;
    public List<Transform> policeTargetPoints;
    
    void Awake()
    {
        _instance = this;
    }

    public void SwapDirection(PoliceCarBehavior policeCarBehavior)
    {
        policeCarBehavior.target = policeTargetPoints[Random.Range(0, policeTargetPoints.Count)];
    }
    
    public void AddPoliceCar(PoliceCarBehavior policeCar)
    {
        policeCars.Add(policeCar);
    }
    
    public void RemovePoliceCar(PoliceCarBehavior policeCar)
    {
        policeCars.Remove(policeCar);
    }
    
    public void RemoveAllPoliceCars()
    {
        policeCars.Clear();
    }
    
    public void RemovePoliceCar(int index)
    {
        policeCars.RemoveAt(index);
    }
}
