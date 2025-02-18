using System;
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

    private DateTime buildDateTime;

    const string gameTitle = "Proto Week-End";

    private void Start()
    {
        SetupData();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void SetupData()
    {
        SetText(gameTitleGUI, gameTitle);
        SetText(versionTextGUI, "Version : " + Application.version);
        
        DisplayDateAndTime();
    }


    private void DisplayDateAndTime()
    {
        SetText(dateTextGUI, buildDateTime.ToString("HH:mm dd/MM/yyyy"));
    }

    private DateTime GetBuildDateTime()
    {
        string buildDateString = Application.buildGUID.Substring(0, 12);
        Debug.Log("Build Date String: " + buildDateString);

        DateTime buildDateTime;
        if (DateTime.TryParseExact(buildDateString, "yyyyMMddHHmm", null, System.Globalization.DateTimeStyles.None, out buildDateTime))
        {
            return buildDateTime;
        }
        else
        {
            //Debug.LogError("Failed to parse build date and time.");
            return DateTime.Now; // Fallback to current date and time in case of failure
        }
    }


    private TextMeshProUGUI SetText(TextMeshProUGUI tmpGUI, string text)
    {
        tmpGUI.text = text;
        return tmpGUI;
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