#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InteractiveLoader))]
public class InteractiveLoaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        InteractiveLoader loader = (InteractiveLoader)target;

        if (GUILayout.Button("Load"))
            loader.load();

    }
}

#endif