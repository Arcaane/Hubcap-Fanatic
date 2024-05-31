using System;
using System.Collections.Generic;
using System.Linq;

namespace Helper.EditorDrawer {
    using HubcapEnemySpawn;
    using UnityEditor;
    using UnityEngine;
    
    [CustomEditor(typeof(WaveSO))]
    public class WaveSOEditor : Editor {
        private SerializedProperty waveDataProperty = null;
        private GUIStyle FoldoutStyle => new(EditorStyles.foldout) {fontStyle = FontStyle.Bold}; 
        private WaveSO waveSO = null;
        private int currentSelected = 0;

        public override void OnInspectorGUI() {
            serializedObject.Update();
            waveSO = (WaveSO) target;
            
            waveDataProperty = serializedObject.FindProperty("waveData");
            StaticCustomDrawer.DrawBaseListDataEditor(waveDataProperty, "spawnType", currentSelected);
            
            List<string> strings = new() { "All" };
            strings.AddRange(Enum.GetNames(typeof(SpawnType)).ToList());
            currentSelected = GUILayout.Toolbar(currentSelected, strings.ToArray());
            
            GUILayout.Space(4);
            StaticCustomDrawer.DrawCustomListEditor(waveDataProperty, DrawCustomElements, "spawnType", currentSelected);
            if(GUILayout.Button("Reorder based on timer")) waveSO.ReorderSpawnsBasedOnTimer();
            
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draw the child parameters of each element of the list
        /// </summary>
        /// <param name="index"></param>
        private void DrawCustomElements(int index) {
            SerializedProperty elementProp = waveDataProperty.GetArrayElementAtIndex(index);
            //            if (currentSelected != 0 && currentSelected != elementProp.FindPropertyRelative("spawnType").enumValueIndex - 1) return;
            using (new GUILayout.HorizontalScope()) {
                GUILayout.Space(15);
                elementProp.isExpanded = EditorGUILayout.Foldout(elementProp.isExpanded, GetDataFoldoutName(elementProp, index), true, FoldoutStyle);
            }
            
            if (!elementProp.isExpanded) return;
            EditorGUILayout.PropertyField(elementProp.FindPropertyRelative("spawnTimer"));
            EditorGUILayout.PropertyField(elementProp.FindPropertyRelative("enemyToSpawn"));
            using (new GUILayout.HorizontalScope()) {
                EditorGUILayout.PropertyField(elementProp.FindPropertyRelative("spawnType"));
                if(GUILayout.Button(EditorGUIUtility.IconContent("Refresh"), GUILayout.Width(30))) waveSO.UpdateEnemyTypeBasedOnKey(index);
            }
            EditorGUILayout.PropertyField(elementProp.FindPropertyRelative("enemySpawnAmount"));

            if (index < waveSO.WaveData.Count && waveSO.GetObjectTypeAtIndex(index) != SpawnType.Enemy) return;
            GUILayout.Space(8);
            EditorGUILayout.PropertyField(elementProp.FindPropertyRelative("spawnEnemiesAtSamePos"));
            EditorGUILayout.PropertyField(elementProp.FindPropertyRelative("timeToSpawnEnemies"));
        }

        #region HELPER
        
        /// <summary>
        /// Get the name of a foldout based on its SerializedProperty
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private GUIContent GetDataFoldoutName(SerializedProperty prop, int index) {
            return new GUIContent($"  SPAWN {(index + 1 < 10 ? $"0{index + 1}" : index + 1)} - {prop.FindPropertyRelative("spawnTimer").floatValue} SEC", 
                GetTextureByType(prop.FindPropertyRelative("spawnType").enumValueIndex)); 
        }

        /// <summary>
        /// Get the texture to apply based on the spawnType of the element
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private Texture2D GetTextureByType(int index) {
            return index switch {
                0 => Resources.Load<Texture2D>("Icons/T_EnemyEditorIcon"),
                1 => Resources.Load<Texture2D>("Icons/T_DeliveryEditorIcon"),
                2 => Resources.Load<Texture2D>("Icons/T_ConvoyEditorIcon"),
                3 => Resources.Load<Texture2D>("Icons/T_BlockadeEditorIcon"),
                _ => null
            };
        }
        
        #endregion HELPER
    }
}