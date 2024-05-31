using UnityEngine;

namespace Helper.EditorDrawer {
    using UnityEditor;
    using HubcapCarBehaviour;

    [CustomEditor(typeof(CarDeliveryHandler))]
    public class CarDeliveryHandlerEditor : Editor {
        private SerializedProperty destroyProperty = null;
        
        public override void OnInspectorGUI() {
            serializedObject.Update();
            destroyProperty = serializedObject.FindProperty("canDestroyDelivery");
            
            GUI.enabled = false;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            GUI.enabled = true;

            destroyProperty.boolValue = GUILayout.Toggle(destroyProperty.boolValue, "Destroy Delivery On Collision", "Button");

            if (!destroyProperty.boolValue) {
                GUILayout.Space(8);
                DrawPropertiesExcluding(serializedObject, new []{ "m_Script", "canDestroyDelivery"});
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}