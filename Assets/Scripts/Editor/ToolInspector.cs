using UnityEditor;
using UnityEngine;

public class ToolInspector : Editor
{
    
}

[CustomEditor(typeof(SaveCollider))]
public class SaveColliderInspector : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Load main collider", GUILayout.MaxWidth(200)))
        {
            foreach (var temp in targets)
            {
                SaveCollider myScript = (SaveCollider)temp;
                myScript.LoadMainCollider();
            }
        }

        if (GUILayout.Button("Save main collider", GUILayout.MaxWidth(200)))
        {
            foreach (var temp in targets)
            {
                SaveCollider myScript = (SaveCollider)temp;
                myScript.SaveMainCollider();
            }
        }

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Load touch collider", GUILayout.MaxWidth(200)))
        {
            foreach (var temp in targets)
            {
                SaveCollider myScript = (SaveCollider)temp;
                myScript.LoadTouchCollider();
            }
        }

        if (GUILayout.Button("Save touch collider", GUILayout.MaxWidth(200)))
        {
            foreach (var temp in targets)
            {
                SaveCollider myScript = (SaveCollider)temp;
                myScript.SaveTouchCollider();
            }
        }

        GUILayout.EndHorizontal();

        DrawDefaultInspector();
    }
}

[CustomEditor(typeof(SaveSlotPos))]
public class SaveSlotPosInspector : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Load slot pos", GUILayout.MaxWidth(200)))
        {
            foreach (var temp in targets)
            {
                SaveSlotPos myScript = (SaveSlotPos)temp;
                myScript.LoadPos();
            }
        }

        if (GUILayout.Button("Save slot pos", GUILayout.MaxWidth(200)))
        {
            foreach (var temp in targets)
            {
                SaveSlotPos myScript = (SaveSlotPos)temp;
                myScript.SavePos();

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        GUILayout.EndHorizontal();

        DrawDefaultInspector();
    }
}
