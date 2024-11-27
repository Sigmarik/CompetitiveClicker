using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GraphNavigator))]
public class MyComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GraphNavigator navigator = (GraphNavigator)target;

        navigator.FixPathSplines();
    }
}
