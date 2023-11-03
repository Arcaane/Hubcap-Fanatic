using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickWallDemo : MonoBehaviour
{
    public List<GameObject> brickStorage = new List<GameObject>();
    private List<Vector3> brickInitialPos = new List<Vector3>();
    private List<Quaternion> brickInitialRot = new List<Quaternion>();

    public void Start()
    {
        AddBricksInStorage();
    }

    void AddBricksInStorage()
    {
        brickStorage.Clear();
        foreach (Transform brickChild in transform)
        {
            brickStorage.Add(brickChild.gameObject);
            brickInitialPos.Add(brickChild.transform.position);
            brickInitialRot.Add(brickChild.transform.rotation);
        }
    }

    public void ResetBrick()
    {
        ResetBrickState();
        ResetBrickPosAndRot();
    }

    void ResetBrickState()
    {
        foreach (GameObject brick in brickStorage)
        {
            brick.GetComponent<Brick>().ResetRB();
        }
    }
    
    void ResetBrickPosAndRot()
    {
        for (int i = 0; i < brickStorage.Count; i++)
        {
            brickStorage[i].transform.position = brickInitialPos[i];
            brickStorage[i].transform.rotation = brickInitialRot[i];
        }
    }
}
