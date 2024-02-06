using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager instance;
    
    private bool isOptionsCanvasDisplay;
    [SerializeField] private GameObject optionsCanvas;
    [SerializeField] private EventSystem evt;
    [SerializeField] private GameObject[] focusOnOpenGO;

    [SerializeField] private GameObject optionSection;
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        optionsCanvas.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            if (isOptionsCanvasDisplay)
            {
                ResumeGame();
            }
            else
            {
                optionsCanvas.SetActive(true);
                SetControllerFocus(0);
                isOptionsCanvasDisplay = !isOptionsCanvasDisplay;
            }
        }
    }

    private void SetControllerFocus(int i)
    {
        evt.SetSelectedGameObject(focusOnOpenGO[i]);
    }

    public void ResumeGame()
    {
        optionsCanvas.SetActive(false);
        isOptionsCanvasDisplay = !isOptionsCanvasDisplay;
    }

    public void OpenOptionsSection()
    {
        optionSection.SetActive(true);
    }
}
