using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameRestartButton : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty scriptProperty = serializedObject.FindProperty("m_Script");
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(scriptProperty);
        EditorGUI.EndDisabledGroup();

        SerializedProperty property = serializedObject.GetIterator();
        property.NextVisible(true);
        while (property.NextVisible(false))
        {
            EditorGUILayout.PropertyField(property, true);
        }

        GUILayout.Space(10);

        GameManager myComponent = (GameManager)target;
        if (GUILayout.Button("RESTART THE GAME"))
        {
            myComponent.RestartTheGame();
        }

        serializedObject.ApplyModifiedProperties();
    }
}