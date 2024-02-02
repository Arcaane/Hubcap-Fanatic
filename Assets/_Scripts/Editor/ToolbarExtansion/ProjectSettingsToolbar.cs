using Helper.Editor;
using UnityEditor;
using UnityEngine;

namespace Toolbar {
    [InitializeOnLoad]
    public class ProjectSettingsToolbar {

        /// <summary>
        /// Method called during an initialization (reload, save, ...)
        /// </summary>
        static ProjectSettingsToolbar() => ToolbarExt.rightToolbarGUI.Add(new DrawerAction(0, PojectSettingsButton));

        /// <summary>
        /// Method which draw the buttons inside the toolbar
        /// </summary>
        private static void PojectSettingsButton() {
            if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("d_SettingsIcon").image, "Open Project Settings"), GUILayout.Width(30))) {
                EditorApplication.ExecuteMenuItem("Edit/Project Settings...");
            }
        }
    }
}