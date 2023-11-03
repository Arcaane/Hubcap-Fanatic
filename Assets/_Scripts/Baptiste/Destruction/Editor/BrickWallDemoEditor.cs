using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BrickWallDemo))]
public class BrickWallDemoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        BrickWallDemo brickWallDemo = (BrickWallDemo)target;
        if (GUILayout.Button("Reset the Brick"))
        {
            brickWallDemo.ResetBrick();
        }
    }
}
