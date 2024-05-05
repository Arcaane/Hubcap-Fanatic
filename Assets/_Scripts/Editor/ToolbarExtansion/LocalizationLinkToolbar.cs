using Helper.Editor;
using UnityEditor;
using UnityEngine;

namespace Toolbar {
    [InitializeOnLoad]
    public class LocalizationLinkToolbar {

        /// <summary>
        /// Method called during an initialization (reload, save, ...)
        /// </summary>
        static LocalizationLinkToolbar() => ToolbarExt.rightToolbarGUI.Add(new DrawerAction(0, LocalizationLinkButton));

        /// <summary>
        /// Method which draw the buttons inside the toolbar
        /// </summary>
        private static void LocalizationLinkButton() {
            if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("d_CloudConnect").image, "Open Localization Sheet"), GUILayout.Width(30))) {
                Application.OpenURL("https://docs.google.com/spreadsheets/d/13kBB4zdnwEmjgYzG_pA_Nowvi1j5VUBc9BnjjIg4kR8/edit?usp=sharing");
            }
        }
    }
}
