namespace Helper.EditorDrawer {
    using HubcapManager;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(PoolManager))]
    public class PoolManagerEditor : Editor {
        private SerializedProperty poolDatasProperty = null;
        private string searchKey = "";
        private bool isFolderOpen = false;
        
        public override void OnInspectorGUI() {
            serializedObject.Update();
            
            StaticCustomDrawer.DrawSingletonCustomEditor(serializedObject, target.GetType().Name);

            isFolderOpen = EditorGUILayout.BeginFoldoutHeaderGroup(isFolderOpen, "Pool keys");
            
            if (isFolderOpen) {
                GUI.backgroundColor = new Color(0f, 0.79f, 1f);
                using (new GUILayout.VerticalScope(EditorStyles.helpBox)) {
                    GUI.backgroundColor = Color.white;
                    GUILayout.Space(4);
                    poolDatasProperty = serializedObject.FindProperty("poolDatas");
                    StaticCustomDrawer.DrawBaseListDataEditor(poolDatasProperty, searchKey, "poolKey");
                    searchKey = EditorGUILayout.TextField("Search Element :", searchKey, new GUIStyle(EditorStyles.textField));
                    GUILayout.Space(4);
                }

                GUILayout.Space(4);

                StaticCustomDrawer.DrawCustomListEditor(poolDatasProperty, DrawCustomElements, "poolKey", searchKey);
                if (GUILayout.Button("Reorder based on name")) ((PoolManager) target).ReorderListByName();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draw all parameter for each element of the list
        /// </summary>
        /// <param name="index"></param>
        private void DrawCustomElements(int index) {
            EditorGUILayout.PropertyField(poolDatasProperty.GetArrayElementAtIndex(index).FindPropertyRelative("poolKey"));
            EditorGUILayout.PropertyField(poolDatasProperty.GetArrayElementAtIndex(index).FindPropertyRelative("prefab"));
            EditorGUILayout.PropertyField(poolDatasProperty.GetArrayElementAtIndex(index).FindPropertyRelative("amountToSpawnAtStart"));
        }
    }
}