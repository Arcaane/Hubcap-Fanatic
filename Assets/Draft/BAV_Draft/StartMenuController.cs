using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [Header("Text in the Menu")]
    [SerializeField] private TextMeshProUGUI gameTitleGUI;
    [SerializeField] private TextMeshProUGUI dateTextGUI;
    [SerializeField] private TextMeshProUGUI versionTextGUI;

    [Header("Button in the Menu")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;

    const string gameTitle = "Proto Week-End";

    private void Start()
    {
        SetupData();
    }

    private void SetupData()
    {
        SetText(gameTitleGUI, gameTitle);
        SetText(versionTextGUI, "Version : " + Application.version);
        
        DisplayDateAndTime();
    }


    private void DisplayDateAndTime()
    {
        #if UNITY_EDITOR
            SetText(dateTextGUI, DateTime.Now.ToString("HH:mm dd/MM/yyyy "));
        #else
            SetText(dateTextGUI, GetBuildDateTime().ToString("HH:mm dd/MM/yyyy"));
        #endif
    }

    private TextMeshProUGUI SetText(TextMeshProUGUI tmpGUI, string text)
    {
        tmpGUI.text = text;
        return tmpGUI;
    }

    private DateTime GetBuildDateTime()
    {
        string buildDateString = Application.buildGUID.Substring(0, 12);
        return DateTime.ParseExact(buildDateString, "yyyyMMddHHmm", null);
    }



    private void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void StartGame()
    {
        LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}