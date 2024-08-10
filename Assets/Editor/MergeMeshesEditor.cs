using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MergeMeshes), true), CanEditMultipleObjects]
public class MergeMeshesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Merge Objects"))
        {
            (target as MergeMeshes).mergeMeshes();
        }
        if (GUILayout.Button("Enable all"))
        {
            (target as MergeMeshes).enableAll();
        }
    }
}
