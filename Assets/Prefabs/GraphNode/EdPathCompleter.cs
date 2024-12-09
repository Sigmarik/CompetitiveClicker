using UnityEditor;

[CustomEditor(typeof(GraphNavigator))]
public class MyComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GraphNavigator navigator = (GraphNavigator)target;

        navigator.OnEditorUpdate();
    }
}
