using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CmdPrefabData : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI methodText = null;
    public string textToWrite = "";
    
    public TextMeshProUGUI MethodText => methodText;
    

    /// <summary>
    /// Increment the text
    /// </summary>
    public void IncrementText() => CommandConsoleRuntime.Instance.MakeTabulation(textToWrite);
}