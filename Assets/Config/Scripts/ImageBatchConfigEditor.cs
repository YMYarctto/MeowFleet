using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ImageBatchConfig))]
public class ImageBatchConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Apply"))
        {
            ((ImageBatchConfig)target).Apply();
        }
    }
}
