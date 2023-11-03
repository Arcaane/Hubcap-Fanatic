using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(BrickWallBuilder))]
public class BrickWallBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BrickWallBuilder brickWallBuilder = (BrickWallBuilder)target;

        if (GUILayout.Button("Add Brick"))
        {
            brickWallBuilder.AddBrick();
        }

        GUILayout.Label("Wall Size:");
        brickWallBuilder.wallWidth = EditorGUILayout.IntField("Width", brickWallBuilder.wallWidth);
        brickWallBuilder.wallHeight = EditorGUILayout.IntField("Height", brickWallBuilder.wallHeight);

        if (GUILayout.Button("Build Wall"))
        {
            brickWallBuilder.BuildWall();
        }
        
        if (GUILayout.Button("Clear List"))
        {
            brickWallBuilder.ClearChildren();
        }

        // Iterate through bricks
        for (int i = 0; i < brickWallBuilder.bricks.Count; i++)
        {
            GUILayout.Label("Brick Properties:");
            brickWallBuilder.bricks[i].position = EditorGUILayout.Vector3Field("Position", brickWallBuilder.bricks[i].position);

            Vector4 quaternionVector = brickWallBuilder.bricks[i].rotation.ToVector4();
            quaternionVector = EditorGUILayout.Vector4Field("Rotation", quaternionVector);
            brickWallBuilder.bricks[i].rotation = new Quaternion(quaternionVector.x, quaternionVector.y, quaternionVector.z, quaternionVector.w);

            brickWallBuilder.bricks[i].scale = EditorGUILayout.Vector3Field("Scale", brickWallBuilder.bricks[i].scale);
            brickWallBuilder.bricks[i].mesh = (Mesh)EditorGUILayout.ObjectField("Mesh", brickWallBuilder.bricks[i].mesh, typeof(Mesh), false);
            brickWallBuilder.bricks[i].material = (Material)EditorGUILayout.ObjectField("Material", brickWallBuilder.bricks[i].material, typeof(Material), false);
            brickWallBuilder.bricks[i].prefab = (GameObject)EditorGUILayout.ObjectField("Brick GameObject", brickWallBuilder.bricks[i].prefab, typeof(GameObject), false);
        }
    }
}




