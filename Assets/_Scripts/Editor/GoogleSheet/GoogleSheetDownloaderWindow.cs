using System.Globalization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace HubcapLocalisation.GoogleSheet {
    public class GoogleSheetDownloaderWindow : EditorWindow {
        private static GoogleSheetDownloaderWindow window;
        private SerializedObject serializedObject = null;
        
        [SerializeField] private string sheetTableID = "";
        [SerializeField] private GoogleSheetSync sheet;
        [SerializeField] private Object SaveFolder = null;
        [SerializeField] private TextAsset dataToLoad = null;
        
        private const string UrlPattern = "https://docs.google.com/spreadsheets/d/{0}/export?format=csv&gid={1}";
        
        [UnityEditor.MenuItem("GoogleSheet/GoogleSheet Downloader")]
        private static void OpenWindow() {
            if (window == null) {
                window = CreateInstance<GoogleSheetDownloaderWindow>();
                window.titleContent = new GUIContent("GoogleSheet Downloader");
                window.minSize = new Vector2(400, 600);
                window.maxSize = new Vector2(400, 600);
            }
            window.Show();
        }

        /// <summary>
        /// Draw the window editor
        /// </summary>
        private void OnGUI() {
            ScriptableObject target = this;
            serializedObject = new SerializedObject(target);
            serializedObject.Update();

            GUI.backgroundColor = new Color(0.25f, 0.25f, 0.25f, 1);
            using (new GUILayout.VerticalScope(EditorStyles.helpBox)) {
                GUI.backgroundColor = Color.white;
                GUILayout.Label("DOWNLOAD GOOGLE SHEET DATA", new GUIStyle(EditorStyles.label) {fontStyle = FontStyle.Bold, fontSize = 8});
                GUILayout.Space(4);
                
                EditorGUILayout.PropertyField(serializedObject.FindProperty("sheetTableID"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("sheet"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("SaveFolder"));
                
                GUILayout.Space(8);

                GUI.enabled = sheetTableID != "" && sheet.name != "" && SaveFolder != null;
                if (GUILayout.Button("Save variables")) SaveVariables();
                if (GUILayout.Button("Download Data")) DownloadData();
                GUI.enabled = true;
            }
            
            GUILayout.Space(8);
            GUI.backgroundColor = new Color(0.25f, 0.25f, 0.25f, 1);
            using (new GUILayout.VerticalScope(EditorStyles.helpBox)) {
                GUI.backgroundColor = Color.white;
                GUILayout.Label("LOAD DATA FROM FILE", new GUIStyle(EditorStyles.label) {fontStyle = FontStyle.Bold, fontSize = 8});

                EditorGUILayout.PropertyField(serializedObject.FindProperty("dataToLoad"));
                GUI.enabled = dataToLoad != null;
                if (GUILayout.Button("Load variables")) LoadVariables();
                GUI.enabled = true;
            }
            
            serializedObject.ApplyModifiedProperties();
        }
        
        /// <summary>
        /// Download data from google sheet
        /// </summary>
        private async void DownloadData() {
            string folder = AssetDatabase.GetAssetPath(SaveFolder);
            
            UnityWebRequest webRequest = UnityWebRequest.Get(string.Format(UrlPattern, sheetTableID, sheet.id));
            webRequest.SendWebRequest();

            while (!webRequest.isDone) { }
            
            if (webRequest.error == null) {
                string path = System.IO.Path.Combine(folder, sheet.name + ".csv");
                await System.IO.File.WriteAllBytesAsync(path, webRequest.downloadHandler.data);
            }
            else {
                Debug.LogError($"WebRequest failed : {webRequest.error}");
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Save variables that are on the editor window
        /// </summary>
        private void SaveVariables() {
            if (sheetTableID == "" || sheet.name == "" || SaveFolder == null) return;
            string data = $"{sheetTableID},{sheet.name},{sheet.id},{AssetDatabase.GetAssetPath(SaveFolder)}";

            string path = System.IO.Path.Combine(AssetDatabase.GetAssetPath(SaveFolder), sheet.name + "_Save.csv");
            System.IO.File.WriteAllText(path, data);
            AssetDatabase.Refresh();
        }
        
        /// <summary>
        /// Load all variables from a file
        /// </summary>
        private void LoadVariables() {
            string[] splitData = dataToLoad.text.Split(',');
            sheetTableID = splitData[0];
            sheet = new GoogleSheetSync() {
                name = splitData[1],
                id = int.Parse(splitData[2], NumberStyles.Integer)
            };
            SaveFolder = AssetDatabase.LoadAssetAtPath<Object>(splitData[3]);
        }
    }
}
