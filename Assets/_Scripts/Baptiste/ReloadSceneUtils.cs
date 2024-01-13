using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadSceneUtils : MonoBehaviour
{
    public DeliveryRessourcesManager deliveryManager;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ReloadScene();
        }
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(50, 50, 2003, 1003), "Press F1 to reload scene");
    }
}

