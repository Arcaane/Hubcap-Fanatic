namespace Helper.EditorDrawer {
    using UnityEngine;
    using HubcapPickupable;
    using UnityEditor;
    
    [CustomEditor(typeof(CarPickupableManager))]
    public class CarPickupableManagerEditor : Editor {
        private SerializedProperty destroyProperty = null;
        
        public override void OnInspectorGUI() {
            serializedObject.Update();
            destroyProperty = serializedObject.FindProperty("destroyPickupable");
            
            GUI.enabled = false;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            GUI.enabled = true;
            
            destroyProperty.boolValue = GUILayout.Toggle(destroyProperty.boolValue, "Destroy Pickupable On Collision", "Button");
            DrawPropertiesExcluding(serializedObject, new []{ "m_Script", "destroyPickupable"});
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}