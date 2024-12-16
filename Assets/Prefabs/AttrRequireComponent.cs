#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class RequireComponentAttribute : PropertyAttribute
{
    public System.Type componentType;

    public RequireComponentAttribute(System.Type type)
    {
        componentType = type;
    }
}

[CustomPropertyDrawer(typeof(RequireComponentAttribute))]
public class RequireComponentPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        RequireComponentAttribute requireComponent = (RequireComponentAttribute)attribute;

        if (property.propertyType == SerializedPropertyType.ObjectReference)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Show the object field with a filter
            GameObject gameObject = (GameObject)EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(GameObject), true);

            if (gameObject != null && gameObject.GetComponent(requireComponent.componentType) == null)
            {
                Debug.LogWarning($"The selected GameObject does not have the required component: {requireComponent.componentType}");
                property.objectReferenceValue = null; // Clear the reference if it doesn't have the component
            }
            else
            {
                property.objectReferenceValue = gameObject;
            }

            EditorGUI.EndProperty();
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Use RequireComponent with GameObject.");
        }
    }
}
#endif