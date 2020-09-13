using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(PMToggle))]
public class PMToggle_Inspector : Editor
{
    SerializedProperty _property;
    PMToggle v;
    private void OnEnable()
    {
        v = (target as PMToggle);
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        v.IsOn = EditorGUILayout.Toggle("IsOn", v.IsOn);
    }
}
