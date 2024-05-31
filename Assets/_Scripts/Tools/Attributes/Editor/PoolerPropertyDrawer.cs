using System.Collections.Generic;
using HubcapManager;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PoolerAttribute))]
public class PoolerPropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        using (new GUILayout.HorizontalScope()) {
            position.width -= 30;
            Rect buttonPosition = new Rect(position.x + position.width + 5, position.y, 30, position.height);

            EditorGUI.PropertyField(position, property, label);
            if (GUI.Button(buttonPosition, EditorGUIUtility.IconContent("CustomTool"))) {
                GenericMenu menu = new GenericMenu();

                List<string> keys = GameObject.FindObjectOfType<PoolManager>().GetAllPoolKeys();
                foreach (string key in keys) {
                    menu.AddItem(new GUIContent(key), keys.Contains(property.stringValue) && key == property.stringValue, () => {
                        property.stringValue = key;
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }
                menu.ShowAsContext();
            }
        }
    }
}