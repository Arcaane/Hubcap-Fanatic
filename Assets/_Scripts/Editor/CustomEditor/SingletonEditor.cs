namespace Helper.EditorDrawer {
    using UnityEditor;

    [CustomEditor(typeof(Singleton<>), true)]
    public class SingletonEditor : Editor {
        public override void OnInspectorGUI() {
            serializedObject.Update();
            
            StaticCustomDrawer.DrawSingletonCustomEditor(serializedObject, target.GetType().Name);
            DrawPropertiesExcluding(serializedObject, "dontDestroyOnLoad", "m_Script");
            
            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomEditor(typeof(SingletonCarBehaviour<>), true)]
    public class SingletonCarBehaviourEditor : Editor {
        public override void OnInspectorGUI() {
            serializedObject.Update();

            StaticCustomDrawer.DrawSingletonCustomEditor(serializedObject, target.GetType().Name);
            DrawPropertiesExcluding(serializedObject, "dontDestroyOnLoad", "m_Script");
            
            serializedObject.ApplyModifiedProperties();
        }
    }
    
    [CustomEditor(typeof(SingletonUpdatesHandler<>), true)]
    public class SingletonUpdatesHandlerEditor : Editor {
        public override void OnInspectorGUI() {
            serializedObject.Update();

            StaticCustomDrawer.DrawSingletonCustomEditor(serializedObject, target.GetType().Name);
            DrawPropertiesExcluding(serializedObject, "dontDestroyOnLoad", "m_Script");
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}