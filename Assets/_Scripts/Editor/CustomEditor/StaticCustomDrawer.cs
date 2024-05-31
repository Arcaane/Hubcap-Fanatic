using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Helper.EditorDrawer {
    public static class StaticCustomDrawer {
        /// <summary>
        /// Draw a custom Singleton inspector
        /// </summary>
        /// <param name="toggle">The toggle which say if the singleton can be destroyed or not</param>
        /// <param name="serializedObject">The serializedObject of the class</param>
        /// <param name="targetTypeName">The name of the class (set "target.GetType().Name")</param>
        public static void DrawSingletonCustomEditor(SerializedObject serializedObject, string targetTypeName) {
            SerializedProperty toggle = serializedObject.FindProperty("dontDestroyOnLoad");

            GUI.backgroundColor = new Color(0, 176f / 256f, 13f / 256f, 1);
            using (new GUILayout.VerticalScope(EditorStyles.helpBox)) {
                GUILayout.Space(5);
                using (new GUILayout.HorizontalScope()) {
                    GUILayout.Space(5);
                    string[] split = Regex.Split(targetTypeName, @"(?<!^)(?=[A-Z])");
                    GUILayout.Label(split.Aggregate("", (current, t) => current + t + " ").ToUpper(), new GUIStyle() {fontSize = 12, normal = new GUIStyleState() {textColor = new Color(84f / 256f, 122f / 256f, 87f / 256f, 1)}, margin = new RectOffset(0, 0, 3, 0)});
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(EditorGUIUtility.IconContent("TreeEditor.Trash", "Don't destroy on load ?"), new GUIStyle() {margin = new RectOffset(0, 0, 2, 0)});
                    EditorGUILayout.PropertyField(toggle, GUIContent.none, GUILayout.Width(25), GUILayout.ExpandHeight(true));
                }

                GUILayout.Space(5);
            }

            GUI.backgroundColor = Color.white;

            GUILayout.Space(8);
        }

        /// <summary>
        /// Draw the base element of the list such as the Array size and some buttons to increase or decrease this list
        /// </summary>
        /// <param name="prop"></param>
        public static void DrawBaseListDataEditor(SerializedProperty prop, string searchKey = "", string stringPropertyName = "") {
            using (new GUILayout.HorizontalScope()) {
                SerializedProperty arraySizeProp = prop.FindPropertyRelative("Array.size");
                
                EditorGUILayout.PropertyField(arraySizeProp);
                if (GUILayout.Button("-", GUILayout.Width(30))) {
                    arraySizeProp.intValue -= 1;
                }

                if (GUILayout.Button("+", GUILayout.Width(30))) {
                    arraySizeProp.intValue += 1;
                    if(searchKey != "" && stringPropertyName != "") prop.GetArrayElementAtIndex(arraySizeProp.intValue - 1).FindPropertyRelative(stringPropertyName).stringValue = searchKey;
                }
            }
        }
        
        /// <summary>
        /// Draw all the elements of the custom list
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="drawCustomElements"></param>
        public static void DrawCustomListEditor(SerializedProperty prop, Action<int> drawCustomElements, string propertyNameToCheck = "", string key = "") {
            for (int index = 0; index < prop.arraySize; index++) {
                if (propertyNameToCheck != "" && !prop.GetArrayElementAtIndex(index).FindPropertyRelative(propertyNameToCheck).stringValue.Contains(key, StringComparison.OrdinalIgnoreCase)) continue;
                
                GUI.backgroundColor = index % 2 == 0 ? new Color(.15f, .15f, .15f, 1) : new Color(.5f, .5f, .5f, 1);
                using (new GUILayout.HorizontalScope(EditorStyles.helpBox)) {
                    using (new GUILayout.VerticalScope()) {
                        GUI.backgroundColor = Color.white;
                        drawCustomElements.Invoke(index);
                    }

                    GUILayout.Space(4);

                    if (GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Trash"), GUILayout.ExpandHeight(true), GUILayout.Width(30))) {
                        prop.DeleteArrayElementAtIndex(index);
                    }
                }
                GUILayout.Space(4);
            }
        }
    }
}