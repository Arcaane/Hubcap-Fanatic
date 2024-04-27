using UnityEditor;
using UnityEngine;

namespace HubcapInterface {
    [CustomEditor(typeof(CustomButtonHandler))]
    public class CustomButtonEditor : Editor {
        private SerializedProperty buttonEventsList = null;

        public override void OnInspectorGUI() {
            serializedObject.Update();
            buttonEventsList = serializedObject.FindProperty("buttonHoverEvents");

            GUI.backgroundColor = new Color(.25f, .25f, .25f, 1);
            using (new GUILayout.VerticalScope(EditorStyles.helpBox)) {
                GUI.backgroundColor = Color.white;
                GUILayout.Label("SELECTABLE NAVIGATION", new GUIStyle(EditorStyles.label) { fontStyle = FontStyle.Bold });
                EditorGUILayout.PropertyField(serializedObject.FindProperty("northSelectable"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("eastSelectable"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("southSelectable"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("westSelectable"));
            }

            GUILayout.Space(8);
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("transitionDuration"));

            GUILayout.Label("BUTTON HOVER EVENTS", new GUIStyle() {fontSize = 9, fontStyle = FontStyle.Bold, normal = new GUIStyleState() {textColor = Color.white}});
            GUILayout.Space(5);



            using (new GUILayout.HorizontalScope()) {
                EditorGUILayout.PropertyField(buttonEventsList.FindPropertyRelative("Array.size"));
                if (GUILayout.Button("-", GUILayout.Width(30))) {
                    buttonEventsList.FindPropertyRelative("Array.size").intValue -= 1;
                }

                if (GUILayout.Button("+", GUILayout.Width(30))) {
                    buttonEventsList.FindPropertyRelative("Array.size").intValue += 1;
                }
            }

            for (int i = 0; i < buttonEventsList.arraySize; i++) {
                SerializedProperty type = buttonEventsList.GetArrayElementAtIndex(i).FindPropertyRelative("eventType");

                GUI.backgroundColor = new Color(.25f, .25f, .25f, 1);
                using (new GUILayout.HorizontalScope(EditorStyles.helpBox)) {
                    using (new GUILayout.VerticalScope()) {
                        GUI.backgroundColor = Color.white;

                        GUILayout.Label($"{buttonEventsList.GetArrayElementAtIndex(i).displayName} : {(ButtonEventType) type.enumValueIndex}", new GUIStyle() {fontSize = 9, normal = new GUIStyleState() {textColor = Color.white}});
                        GUILayout.Space(5);

                        EditorGUILayout.PropertyField(type);

                        switch ((ButtonEventType) type.enumValueIndex) {
                            case ButtonEventType.Scale: DrawCustomFields("parentTransform", i, "targetSize"); break;
                            case ButtonEventType.ColorImage: DrawCustomFields("spriteImage", i, "targetColor"); break;
                            case ButtonEventType.ColorText: DrawCustomFields("text", i, "targetColor"); break;
                            case ButtonEventType.SpriteSwipe: DrawCustomFields("spriteImage", i, "targetSprite"); break;
                            case ButtonEventType.Alpha: DrawCustomFields("canvasGroup", i, "targetAlpha"); break;
                            case ButtonEventType.FillAmount: DrawCustomFields("spriteImage", i, "targetAmount"); break;
                            case ButtonEventType.None: break;
                        }

                        GUILayout.Space(3);
                    }

                    if (GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Trash"), GUILayout.ExpandHeight(true), GUILayout.Width(30))) {
                        buttonEventsList.DeleteArrayElementAtIndex(i);
                    }
                }

                GUILayout.Space(4);
            }
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onClickEvents"));

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draw custom fields based on enum
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="index"></param>
        /// <param name="propertyName"></param>
        private void DrawCustomFields(string prop, int index, string propertyName) {
            EditorGUILayout.PropertyField(buttonEventsList.GetArrayElementAtIndex(index).FindPropertyRelative(prop));
            EditorGUILayout.PropertyField(buttonEventsList.GetArrayElementAtIndex(index).FindPropertyRelative(propertyName));
        }
    }
}
