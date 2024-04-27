using System;
using System.Collections.Generic;
using UnityEngine;

public class DisplayState : MonoBehaviour
{ 
    [Header("------ Display State -----")]
    public ParticleSystem stateDisplay;
    private Material stateDisplayMaterial;
    public List<Texture2D> stateDisplayTexture;
    
    [Header("------ Field that need to be Private -----")]
    public GameObject obj;

    private void Start()
    {
        stateDisplayMaterial = stateDisplay.GetComponent<Renderer>().sharedMaterial;
        SetStateDisplay(0);
    }

    void OnEnable()
    {
        stateDisplay.Play();
    }
    
    void OnDisable()
    {
        stateDisplay.Stop();
    }
    
    void SetStateDisplay(int state)
    {
        stateDisplayMaterial.SetTexture("_MainTex", stateDisplayTexture[state]);
    }
}
