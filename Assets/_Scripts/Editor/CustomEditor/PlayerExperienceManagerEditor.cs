namespace Helper.EditorDrawer {
    using HubcapCarBehaviour;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(PlayerExperienceManager))]
    public class PlayerExperienceManagerEditor : Editor {
        private SerializedProperty experienceList = null;
        private bool isListOpen = false;
        
        public override void OnInspectorGUI() {
            serializedObject.Update();
            experienceList = serializedObject.FindProperty("xpRequiredPerLevel");
            
            GUI.enabled = false;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            GUI.enabled = true;
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentLevel"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentExperienceAmount"));

            isListOpen = EditorGUILayout.Foldout(isListOpen, "Experience Per Level List", true);
            if (isListOpen) {
                StaticCustomDrawer.DrawBaseListDataEditor(experienceList);
                StaticCustomDrawer.DrawCustomListEditor(experienceList, DrawCustomElements);
                GUILayout.Space(4);
            }
            
            DrawPropertiesExcluding(serializedObject, "m_Script", "currentLevel", "currentExperienceAmount", "xpRequiredPerLevel");
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draw the element for each element of the list
        /// </summary>
        /// <param name="index"></param>
        private void DrawCustomElements(int index) {
            using (new EditorGUILayout.HorizontalScope()) {
                GUILayout.Label($"Level {(index + 1 < 10 ? "0" : "")}{index + 1}");
                GUILayout.FlexibleSpace();
                
                GUI.enabled = false;
                GUILayout.Label("xp :", GUILayout.Width(35));
                GUI.enabled = true;
                EditorGUILayout.PropertyField(experienceList.GetArrayElementAtIndex(index).FindPropertyRelative("xpRequired"), GUIContent.none, GUILayout.Width(75));
                
                GUI.enabled = false;
                GUILayout.Label("sum :", GUILayout.Width(35));
                EditorGUILayout.PropertyField(experienceList.GetArrayElementAtIndex(index).FindPropertyRelative("previousXpSum"), GUIContent.none, GUILayout.Width(75));
                GUI.enabled = true;
            }
        }
    }
}