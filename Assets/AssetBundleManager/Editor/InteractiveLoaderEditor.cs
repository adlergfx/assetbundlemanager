using Codice.CM.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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