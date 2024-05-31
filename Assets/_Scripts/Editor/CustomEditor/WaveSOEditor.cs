namespace Helper.EditorDrawer {
    using HubcapEnemySpawn;
    using UnityEditor;
    using UnityEngine;
    
    [CustomEditor(typeof(WaveSO))]
    public class WaveSOEditor : Editor {
        private SerializedProperty waveDataProperty = null;
        private WaveSO waveSO = null;
        
        public override void OnInspectorGUI() {
            serializedObject.Update();
            waveSO = (WaveSO) target;
            
            waveDataProperty = serializedObject.FindProperty("waveData");
            StaticCustomDrawer.DrawBaseListDataEditor(waveDataProperty);
            GUILayout.Space(4);
            StaticCustomDrawer.DrawCustomListEditor(waveDataProperty, DrawCustomElements);
            if(GUILayout.Button("Reorder based on timer")) waveSO.ReorderSpawnsBasedOnTimer();
            
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draw the child parameters of each element of the list
        /// </summary>
        /// <param name="index"></param>
        private void DrawCustomElements(int index) {
            EditorGUILayout.PropertyField(waveDataProperty.GetArrayElementAtIndex(index).FindPropertyRelative("spawnTimer"));
            EditorGUILayout.PropertyField(waveDataProperty.GetArrayElementAtIndex(index).FindPropertyRelative("enemyToSpawn"));
            using (new GUILayout.HorizontalScope()) {
                EditorGUILayout.PropertyField(waveDataProperty.GetArrayElementAtIndex(index).FindPropertyRelative("spawnType"));
                if(GUILayout.Button(EditorGUIUtility.IconContent("Refresh"), GUILayout.Width(30))) waveSO.UpdateEnemyTypeBasedOnKey(index);
            }
            EditorGUILayout.PropertyField(waveDataProperty.GetArrayElementAtIndex(index).FindPropertyRelative("enemySpawnAmount"));

            if (index < waveSO.WaveData.Count && waveSO.GetObjectTypeAtIndex(index) != SpawnType.Enemy) return;
            GUILayout.Space(8);
            EditorGUILayout.PropertyField(waveDataProperty.GetArrayElementAtIndex(index).FindPropertyRelative("spawnEnemiesAtSamePos"));
            EditorGUILayout.PropertyField(waveDataProperty.GetArrayElementAtIndex(index).FindPropertyRelative("timeToSpawnEnemies"));
        }
    }
}