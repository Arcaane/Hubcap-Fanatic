using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Helper.Editor;
using UnityEditor.SceneManagement;

namespace Toolbar {
    [InitializeOnLoad]
    public class LevelSelectionToolbar {
        private static int levelIndex => PlayerPrefs.GetInt("SceneDirectoryIndex", 0);
        private static int value = 0;
        private const int levelSelectionSize = 225;
        
        /// <summary>
        /// Method called during an initialization (reload, save, ...)
        /// </summary>
        static LevelSelectionToolbar() => ToolbarExt.leftToolbarGUI.Add(new DrawerAction(50, LevelSelection));

        /// <summary>
        /// Draw the Dropdown for the level selection
        /// </summary>
        private static void LevelSelection() {
            GUILayout.FlexibleSpace();
            GUILayout.Label("Scene :");
            
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.Popup(levelIndex, GetLevelPossibilities(Path.GetFileName(EditorSceneManager.GetActiveScene().path).Split(".")[0]).ToArray(), GUILayout.Width(levelSelectionSize));
            if (EditorGUI.EndChangeCheck()) {
                for (int i = 0; i < EditorSceneManager.sceneCount; i++) {
                    if (!EditorSceneManager.GetSceneAt(i).isDirty) continue;
                    EditorSceneManager.SaveScene(EditorSceneManager.GetSceneAt(i));
                }
                
                OpenNewScene(value);
            }
        }

        #region Helper
        /// <summary>
        /// Retrieve all the scene that are selectable
        /// </summary>
        /// <returns></returns>
        private static List<string> GetLevelPossibilities(string currentSceneName) {
            List<string> possibilities = EditorBuildSettings.scenes.Select(file => Path.GetFileName(file.path).Split(".")[0]).ToList();
            
            if (!possibilities.Contains(currentSceneName)) {
                possibilities.Add("** " + currentSceneName);
                PlayerPrefs.SetInt("SceneDirectoryIndex", possibilities.Count - 1);
            }
            else {
                PlayerPrefs.SetInt("SceneDirectoryIndex", possibilities.IndexOf(currentSceneName));
            }

            return possibilities;
        }

        /// <summary>
        /// Open the new scene and save the index value
        /// </summary>
        /// <param name="value"></param>
        private static void OpenNewScene(int value) {
            EditorSceneManager.OpenScene(EditorBuildSettings.scenes[value].path);
            PlayerPrefs.SetInt("SceneDirectoryIndex", value);
        }
        #endregion Helper
    }
}