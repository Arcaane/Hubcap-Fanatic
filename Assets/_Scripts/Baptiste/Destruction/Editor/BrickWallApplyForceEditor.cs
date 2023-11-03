using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BrickWallApplyForce))]
public class BrickWallApplyForceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        BrickWallApplyForce vatSample = (target as BrickWallApplyForce);

        if (Application.isPlaying)
        {
            if (GUILayout.Button("Apply Force"))
            {
                vatSample.ApplyForce();
            }
        }
        else
        {
            GUILayout.Label("Enter PlayMode to be able to Apply Force");
        }
    }
}
