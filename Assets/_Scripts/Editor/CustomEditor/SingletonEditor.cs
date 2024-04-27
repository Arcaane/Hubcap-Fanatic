using System.Linq;
using System.Text.RegularExpressions;
using Helper;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Singleton<>), true)]
public class SingletonEditor : Editor {
    private SerializedProperty toggle = null;
    
    public override void OnInspectorGUI() {
        serializedObject.Update();
        toggle = serializedObject.FindProperty("dontDestroyOnLoad");
        GUI.backgroundColor = new Color(0, 176f/256f, 13f/256f, 1);
        using (new GUILayout.VerticalScope(EditorStyles.helpBox)) {
            GUILayout.Space(5);
            using (new GUILayout.HorizontalScope()) {
                GUILayout.Space(5);
                string[] split =  Regex.Split(target.GetType().Name, @"(?<!^)(?=[A-Z])");
                GUILayout.Label(split.Aggregate("", (current, t) => current + t + " ").ToUpper(), new GUIStyle() { fontSize = 12, normal = new GUIStyleState() { textColor = new Color(84f/256f, 122f/256f, 87f/256f, 1) }, margin = new RectOffset(0,0,3,0)});
                GUILayout.FlexibleSpace();
                GUILayout.Label(EditorGUIUtility.IconContent("TreeEditor.Trash", "Don't destroy on load ?"), new GUIStyle() { margin = new RectOffset(0,0,2,0)});
                EditorGUILayout.PropertyField(toggle, GUIContent.none, GUILayout.Width(25), GUILayout.ExpandHeight(true));
                //toggle.boolValue = EditorGUILayout.Toggle(, toggle.boolValue, GUILayout.Width(100));
            }
            GUILayout.Space(5);
        }

        GUI.backgroundColor = Color.white;

        GUILayout.Space(8);
        
        DrawPropertiesExcluding(serializedObject, "dontDestroyOnLoad", "m_Script");
        serializedObject.ApplyModifiedProperties();
    }
}