using UnityEngine;
using UnityEditor;

// Place this script in "Editor" (Assets->Editor) folder.
// If there is no such folder, create it.
[CustomPropertyDrawer(typeof(ShowOnlyAttribute))]
public class ShowOnlyDrawer : PropertyDrawer
{
    /// <summary>
    /// Hides a field
    /// </summary>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label);
        GUI.enabled = true;
    }
}