namespace Helper.EditorDrawer {
    using HubcapInterface;
    using UnityEditor;
    using UnityEngine;
    
    [CustomEditor(typeof(ButtonManager))]
    public class ButtonManagerEditor : Editor {
        private SerializedProperty buttonEventsList = null;
        private string[] exludePropertiesToDraw = {
            "dontDestroyOnLoad", "m_Script", "northSelectable", "eastSelectable", "southSelectable", "westSelectable", "transitionDuration",
            "buttonHoverEvents", "onClickEvents",
            "baseSliderData", "disableSliderData", "shadowDistance", "horLayoutFront","horLayoutBack", "showTextWhenDisable", "showIconBackground", "iconSprite", "textSliderFront",
            "textSliderBack", "iconSliderFront","iconSliderBack"
        };
        private string[] exludePropertiesToDrawB = {
            "dontDestroyOnLoad", "m_Script", "northSelectable", "eastSelectable", "southSelectable", "westSelectable", "transitionDuration",
            "buttonHoverEvents", "onClickEvents",
            "baseSliderData", "disableSliderData", "fillAmount", "disableAmount", "currentValue", "sliderdata","side", "sliderTxt", "sliderColorImage", "sliderBackgroundImage", 
            "sliderShadowImage", "useDisableAmount"
        };

        public override void OnInspectorGUI() {
            serializedObject.Update();
            buttonEventsList = serializedObject.FindProperty("buttonHoverEvents");

            //DRAW UI SELECTABLE
            GUI.backgroundColor = new Color(.25f, .25f, .25f, 1);
            using (new GUILayout.VerticalScope(EditorStyles.helpBox)) {
                GUI.backgroundColor = Color.white;
                GUILayout.Label("SELECTABLE NAVIGATION", new GUIStyle(EditorStyles.label) {fontStyle = FontStyle.Bold, fontSize = 8});
                EditorGUILayout.PropertyField(serializedObject.FindProperty("northSelectable"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("eastSelectable"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("southSelectable"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("westSelectable"));
                GUILayout.Space(4);
            }

            GUILayout.Space(8);

            //DRAW BASE SLIDER DATA
            GUI.backgroundColor = new Color(.25f, .25f, .25f, 1);
            using (new GUILayout.VerticalScope(EditorStyles.helpBox)) {
                GUI.backgroundColor = Color.white;
                GUILayout.Label("BASE SLIDER DATA", new GUIStyle(EditorStyles.label) {fontStyle = FontStyle.Bold, fontSize = 8});
                DrawPropertiesExcluding(serializedObject, exludePropertiesToDraw);
                using (new GUILayout.HorizontalScope()) {
                    GUILayout.Space(12);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("baseSliderData"), true);
                }

                using (new GUILayout.HorizontalScope()) {
                    GUILayout.Space(12);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("disableSliderData"), true);
                }

                DrawPropertiesExcluding(serializedObject, exludePropertiesToDrawB);
                GUILayout.Space(4);
            }

            GUILayout.Space(8);

            //DRAW BUTTONS DATA
            GUI.backgroundColor = new Color(.25f, .25f, .25f, 1);
            using (new GUILayout.VerticalScope(EditorStyles.helpBox)) {
                GUI.backgroundColor = Color.white;
                GUILayout.Label("BUTTONS DATA", new GUIStyle(EditorStyles.label) {fontStyle = FontStyle.Bold, fontSize = 8});
                EditorGUILayout.PropertyField(serializedObject.FindProperty("transitionDuration"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onClickEvents"));
            }

            GUILayout.Space(8);
            
            StaticCustomDrawer.DrawBaseListDataEditor(buttonEventsList);
            GUILayout.Space(4);
            StaticCustomDrawer.DrawCustomListEditor(buttonEventsList, DrawCustomElements);

            serializedObject.ApplyModifiedProperties();
        }
        
        /// <summary>
        /// Draw the child parameters of each element of the list
        /// </summary>
        /// <param name="index"></param>
        private void DrawCustomElements(int index) {
            SerializedProperty type = buttonEventsList.GetArrayElementAtIndex(index).FindPropertyRelative("eventType");
            GUILayout.Label($"{buttonEventsList.GetArrayElementAtIndex(index).displayName} : {(ButtonEventType) type.enumValueIndex}", new GUIStyle() {fontSize = 9, normal = new GUIStyleState() {textColor = Color.white}});
            GUILayout.Space(5);

            EditorGUILayout.PropertyField(type);

            switch ((ButtonEventType) type.enumValueIndex) {
                case ButtonEventType.Scale:
                    DrawCustomFields("parentTransform", index, "targetSize");
                    break;
                case ButtonEventType.ColorImage:
                    DrawCustomFields("spriteImage", index, "targetColor");
                    break;
                case ButtonEventType.ColorText:
                    DrawCustomFields("text", index, "targetColor");
                    break;
                case ButtonEventType.SpriteSwipe:
                    DrawCustomFields("spriteImage", index, "targetSprite");
                    break;
                case ButtonEventType.Alpha:
                    DrawCustomFields("canvasGroup", index, "targetAlpha");
                    break;
                case ButtonEventType.FillAmount:
                    DrawCustomFields("spriteImage", index, "targetAmount");
                    break;
                case ButtonEventType.DoTweenEffect:
                    DrawCustomFields("doTweenEffect", index, "");
                    break;
                case ButtonEventType.None: break;
            }
        }

        /// <summary>
        /// Draw custom fields based on enum
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="index"></param>
        /// <param name="propertyName"></param>
        private void DrawCustomFields(string prop, int index, string propertyName) {
            EditorGUILayout.PropertyField(buttonEventsList.GetArrayElementAtIndex(index).FindPropertyRelative(prop));
            if(propertyName != "") EditorGUILayout.PropertyField(buttonEventsList.GetArrayElementAtIndex(index).FindPropertyRelative(propertyName));
        }
    }
}