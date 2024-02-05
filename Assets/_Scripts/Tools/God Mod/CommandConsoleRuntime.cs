using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CommandConsoleRuntime : MonoBehaviour {
    #region VARIABLES
    
    #region INSTANCE
    private static CommandConsoleRuntime instance = null;
    public static CommandConsoleRuntime Instance => instance;
    
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else Destroy(gameObject);
        
        commandList.Clear();
    }
    #endregion INSTANCE

    private ToolsInput inputs;
    [SerializeField] private TMP_InputField inputField = null;
    [SerializeField] private TextMeshProUGUI text = null;
    [SerializeField] private GameObject objectChild = null;
    public GameObject ObjectChild => objectChild;
    [Space]
    [SerializeField] private Transform textTransformArea = null;
    [SerializeField] private GameObject textPrefab = null;
    [Space]
    [SerializeField] private Transform parameterTransformArea = null;
    [SerializeField] private GameObject parameterPrefab = null;

    [HideInInspector] public List<object> commandList = new List<object>();

    private bool isShowingHelp = false;
    private List<GameObject> methodList = new List<GameObject>();
    private string lastCommand = "";
    
    #endregion VARIABLES
    
    #region BASIC METHODS
    private void Start() {
        CommandConsole HELP = new CommandConsole("help", "help : show all the possible commands", null, (_) => { ShowHelp(true); });
        
        /* EXAMPLE WITH ENUM
        CommandConsole STRAW = new CommandConsole("straw", "straw <strawType> <strawType2>", new List<CommandClass>() {new (typeof(strawType)), new (typeof(strawType2))}, (value) => { Debug.Log(value[0] + " " + value[1]); });
        */
        AddCommand(HELP);
        
        objectChild.SetActive(false);
        inputs = new ToolsInput();
        inputs.Enable();
        inputs.Interface.OpenConsole.started += _ => {
            if (objectChild.activeSelf) return;
            objectChild.SetActive(true);
            SelectInputField();
            CheckForHelp();
        };
    }

    private void Update() {
        if (inputField != null && inputField.IsActive()) {

            if(Input.GetKeyDown(KeyCode.Return)) CheckEndText();
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Backspace)) RemoveText();
            if (Input.GetKeyDown(KeyCode.Escape)) {
                ResetPrefab();
                objectChild.SetActive(false);
                inputField.text = "";
                //if (Playercontroller.Instance != null) Playercontroller.Instance.ChangeInputState(true);
            }
            if(Input.GetKeyDown(KeyCode.Tab) && methodList.Count != 0) MakeTabulation(methodList[0].GetComponent<CmdPrefabData>().textToWrite);
            if (Input.GetKeyDown(KeyCode.UpArrow) && lastCommand != "") {
                inputField.text = lastCommand;
                inputField.caretPosition = inputField.text.Length;
            }
        }
        else {
           /* if (Input.GetKeyDown(KeyCode.Escape)) {
                Time.timeScale = Time.timeScale == 0 ? 1 : 
            }*/
        }
    }
    #endregion BASIC METHODS
    
    #region CHECK INPUTFIELD
    /// <summary>
    /// Add a text to check
    /// </summary>
    /// <param name="inputField"></param>
    public void AddTextToCheck() => CheckEndText();

    /// <summary>
    /// Check the text in an input field
    /// </summary>
    /// <param name="inputField"></param>
    private void CheckEndText() {
        string[] properties = inputField.text.Split(' ');
        
        ResetPrefab();
        
        foreach (object obj in commandList) {
            if (obj is CommandConsoleBase command && properties[0].ToUpper().Contains(command.CommandName.ToUpper())) {
                List<string> paramerterListString = new List<string>(properties);
                paramerterListString.RemoveAt(0);
                (command as CommandConsole)?.InvokeCommand(paramerterListString);
                lastCommand = inputField.text;
            }
        }

        if (inputField.text.ToUpper() != "HELP") {
            objectChild.SetActive(false);
            //if (Playercontroller.Instance != null) Playercontroller.Instance.ChangeInputState(true);
        }
        inputField.text = "";
    }

    /// <summary>
    /// Check for help debug when there is new letter
    /// </summary>
    /// <param name="inputField"></param>
    public void CheckForHelp() {
        string textField = inputField.text;
        string[] textFieldProperties = textField.Split(' ');

        string property = textFieldProperties[0].ToUpper();
        int propertyLength = property.Length;
        
        if (textFieldProperties.Length == 1) {
            if (textField == "" && !isShowingHelp) {
                ShowHelpText();
                return;
            }

            if (isShowingHelp && textField == "") return;

            ResetPrefab();
            isShowingHelp = false;
            foreach (object obj in commandList) {
                if (obj is CommandConsole command) {
                    string propertyNameCut = propertyLength < command.CommandName.Length ? command.CommandName.ToUpper().Remove(propertyLength) : command.CommandName.ToUpper();
                    if (propertyNameCut == property) CreateMethodPrefab(command.DescriptionCommand, propertyLength);

                    if (command.CommandsClassType == null && property == command.CommandName.ToUpper()) {
                        text.color = new Color(13f/255f, 1, 0, 128f / 255f);
                    }
                }
            }

            if (methodList.Count == 0) text.color = new Color(1, 0, 0, 128f / 255f);
            else if(methodList[0].GetComponent<CmdPrefabData>().textToWrite.Split(' ')[0] != "") text.color = new Color(1, 1, 1, 128f / 255f);
        }
        else if (textFieldProperties.Length >= 2) {
            ResetPrefab();
            isShowingHelp = false;

            foreach (object obj in commandList) {
                if (obj is CommandConsole command && command.CommandsClassType != null) {
                    int commandPropertyLength = command.CommandsClassType.Count;

                    if (string.Equals(textFieldProperties[0], command.CommandName, StringComparison.CurrentCultureIgnoreCase)) {
                        if (commandPropertyLength >= 1 && commandPropertyLength >= textFieldProperties.Length - 1) {
                            Type actualType = command.CommandsClassType[textFieldProperties.Length - 2].parameterType;
                            string actualProperty = textFieldProperties[textFieldProperties.Length - 1];
                            int actualPropertyLength = actualProperty.Length;

                            if (actualType.IsEnum) {
                                foreach (string enumTxt in Enum.GetNames(actualType)) {
                                    string cutEnum = actualPropertyLength < enumTxt.Length ? enumTxt.Remove(actualPropertyLength) : enumTxt;
                                    if (cutEnum == actualProperty) {
                                        CreateParameterPrefab(enumTxt, textField.Length, actualPropertyLength);
                                        if(commandPropertyLength == textFieldProperties.Length - 1 && actualProperty == enumTxt) text.color = new Color(13f/255f, 1, 0, 128f / 255f);
                                        else text.color = new Color(1, 1, 1, 128f / 255f);
                                    }
                                }
                                
                                if(methodList.Count == 0) text.color = new Color(1, 0, 0, 128f / 255f);
                            }
                            else if (actualType == typeof(bool)) {
                                CreateParameterPrefab("true", textField.Length, actualPropertyLength);
                                CreateParameterPrefab("false", textField.Length, actualPropertyLength);

                                string trueCut = actualPropertyLength < 4 ? "true".Remove(actualPropertyLength) : "true";
                                string falseCut = actualPropertyLength < 5 ? "false".Remove(actualPropertyLength) : "false";
                                
                                if(commandPropertyLength == textFieldProperties.Length && (actualProperty == "true" || actualProperty == "false")) text.color = new Color(13f/255f, 1, 0, 128f / 255f);
                                else if(commandPropertyLength == textFieldProperties.Length && (actualProperty != trueCut || actualProperty != falseCut)) text.color = new Color(1, 0, 0, 128f / 255f);
                                else text.color =  new Color(1, 1, 1, 128f / 255f);
                            }
                            else {
                                string typeName = actualType.Name;
                                CreateParameterPrefab(typeName, textField.Length, actualPropertyLength);
                                text.color =  new Color(1, 1, 1, 128f / 255f);
                            }
                            
                        }
                        else {
                            text.color = new Color(1, 0, 0, 128f / 255f);
                        }
                    }
                }
            }
        }
    }

    #endregion CHECK INPUTFIELD
    
    #region CREATE PREFAB

    /// <summary>
    /// Create a prefab for the possible methods
    /// </summary>
    /// <param name="textToShow"></param>
    /// <returns></returns>
    private void CreateMethodPrefab(string textToShow, int actualTextWriteLength) {
        GameObject gam = Instantiate(textPrefab, textTransformArea);
        CmdPrefabData data = gam.GetComponent<CmdPrefabData>();
        data.MethodText.text = textToShow;

        string TextToWrite = textToShow.Split(' ')[0] + (textToShow.Split(' ').Length > 1 ? " " : "");
        if (TextToWrite.Length >= actualTextWriteLength) TextToWrite = TextToWrite.Remove(0,actualTextWriteLength);
        data.textToWrite = TextToWrite;

        methodList.Add(gam);
    }

    /// <summary>
    /// Create a parameter Prefab for the possible parameters
    /// </summary>
    /// <param name="textToShow"></param>
    /// <returns></returns>
    private void CreateParameterPrefab(string textToShow, int textSize, int actualTextWriteLength) {
        GameObject gam = Instantiate(parameterPrefab, parameterTransformArea);
        RectTransform trans = gam.GetComponent<RectTransform>();
        CmdPrefabData data = gam.GetComponent<CmdPrefabData>();
        data.MethodText.text = textToShow;

        string TextToWrite = textToShow.Split(' ')[0] + (textToShow.Split(' ').Length > 1 ? " " : "");
        if (TextToWrite.Length >= actualTextWriteLength) TextToWrite = TextToWrite.Remove(0,actualTextWriteLength);
        data.textToWrite = TextToWrite;
        
        trans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, textSize * 12, textToShow.Length * 20f);
        trans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -38 + (-38 * methodList.Count), 35);
        trans.anchorMin = new Vector2(0, 0);
        trans.anchorMax = new Vector2(0, 0);

        methodList.Add(gam);
    }

    #endregion CREATE PREFAB

    /// <summary>
    /// Reset all the prefab that are shown actually
    /// </summary>
    private void ResetPrefab() {
        foreach (GameObject child in methodList) {
            Destroy(child);
        }

        methodList.Clear();
    }

    #region CHANGE INPUTFIELD
    /// <summary>
    /// Fill the tabulation text
    /// </summary>
    public void MakeTabulation(string tabText) {
        if (inputField != null) {
            inputField.text += tabText;
            inputField.caretPosition = inputField.text.Length;
            
            SelectInputField();
            ResetPrefab();
            CheckForHelp();
        }
    }
    
    /// <summary>
    /// Remove the last word in the inputfield
    /// </summary>
    private void RemoveText() {
        string lastWord = inputField.text.Split(' ')[inputField.text.Split(' ').Length - 1];
        inputField.text = lastWord.Length == inputField.text.Length ? "" : inputField.text.Remove(inputField.text.Length - (lastWord.Length + 1));
    }
    #endregion CHANGE INPUTFIELD
    
    #region HELP COMMAND
    /// <summary>
    /// Show help
    /// </summary>
    private void ShowHelp(bool showAll = false) {
        ResetPrefab();

        isShowingHelp = true;

        List<CommandConsoleBase> cmdList = new List<CommandConsoleBase>();
        foreach (object obj in commandList) {
            if (obj is CommandConsoleBase command) {
                cmdList.Add(command);
            }
        }
        List<CommandConsoleBase> cmdListSorted = cmdList.OrderByDescending(cmdL => cmdL.CommandName).ToList();
        foreach (CommandConsoleBase cmd in cmdListSorted) {
            if (showAll) {
                if (cmd.CommandName.ToUpper() != "HELP") CreateMethodPrefab(cmd.DescriptionCommand, 0);
            }
            else {
                if (cmd.CommandName.ToUpper() != "HELP") CreateMethodPrefab(cmd.CommandName, 0);
            }
        }

        SelectInputField();
    }

    /// <summary>
    /// Show the help text
    /// </summary>
    private void ShowHelpText() {
        CreateMethodPrefab("help", 0);
    }
    #endregion HELP COMMAND
    
    /// <summary>
    /// Select the InputField
    /// </summary>
    private void SelectInputField() {
        inputField.Select();
        inputField.ActivateInputField();
    }

    public void AddCommand(CommandConsole command) {
        foreach (object obj in commandList) {
            if (obj is CommandConsole cmd && cmd.CommandName.ToUpper() == command.CommandName.ToUpper()) return;
        }
        
        commandList.Add(command);
    }
}

#region HANDLE COMMANDS
/// <summary>
/// Class for a command
/// </summary>
public class CommandClass {
    public Type parameterType = null;

    public CommandClass(Type parameterType) {
        this.parameterType = parameterType;
    }
}

/// <summary>
/// Base of the console command class
/// </summary>
public class CommandConsoleBase {
    private string commandName;
    private string descriptionCommand;
    private List<CommandClass> commandsClassType;

    public string CommandName => commandName;
    public string DescriptionCommand => descriptionCommand;
    public List<CommandClass> CommandsClassType => commandsClassType;

    protected CommandConsoleBase(string commandName, string descriptionCommand, List<CommandClass> commandsClassType) {
        this.commandName = commandName;
        this.descriptionCommand = descriptionCommand;
        this.commandsClassType = commandsClassType;
    }
}

/// <summary>
/// Function to handle the commands
/// </summary>
public class CommandConsole : CommandConsoleBase {
    private Action<List<string>> actionEvent = null;

    public CommandConsole(string commandName, string descriptionCommand, List<CommandClass> commandsClassType, Action<List<string>> actionEvent) : base(commandName, descriptionCommand, commandsClassType) {
        this.actionEvent = actionEvent;
    }

    public void InvokeCommand(List<string> parameterString) => actionEvent.Invoke(parameterString);
}
#endregion HANDLE COMMANDS