using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(egUIManager))]
public class egCanvasEditor : Editor
{
    SerializedProperty scriptProperty;

    void OnEnable()
    {
        // Store the script property
        scriptProperty = serializedObject.FindProperty("m_Script");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();


        // Draw the default inspector, so that other variables are displayed as usual
        //DrawDefaultInspector();
        DrawPropertiesExcluding
        (
            serializedObject,
                //Control Bool Variables:
            "sessionClear",
            "darkenBackground",
            "soundPlayer",
            "victoryJingle",
            "endMusic",
            "retryButton",
            "NextButton",
            "MenuButton",
            "pausePanel",
            "isPaused",
            "exitBodyView",
            "seeBodyView"
            );


        // Get the serialized property of the boolean variable
        SerializedProperty showVariableProperty = serializedObject.FindProperty("usePauseButton");
        EditorGUILayout.PropertyField(showVariableProperty);

        EditorGUI.indentLevel++;

        // If pauseButtonIsUsed is false, hide myVariable
        if (showVariableProperty.boolValue)
        {
            // Disable the entire block of code
            EditorGUI.BeginDisabledGroup(false);

            // Draw the property field for myVariableProperty
            SerializedProperty myVariableProperty = serializedObject.FindProperty("pauseButtonPosition");
            EditorGUILayout.PropertyField(myVariableProperty);

            if (myVariableProperty.enumValueIndex == 4)
            {
                SerializedProperty myVariableProperty2 = serializedObject.FindProperty("pauseBtn_customPosition");
                EditorGUILayout.PropertyField(myVariableProperty2);
            }
            // End the disabled group
            EditorGUI.EndDisabledGroup();
        }
        else
        {
            EditorGUILayout.LabelField("Use egUIManager.PauseGame() for custom pause buttons.", EditorStyles.label);
        }

        EditorGUI.indentLevel--;

        //EditorGUILayout.LabelField("\n");

        //SerializedProperty cameraViewBoolProperty = serializedObject.FindProperty("bodyTrackerView");
        //EditorGUILayout.PropertyField(cameraViewBoolProperty);

        EditorGUI.indentLevel++;

        SerializedProperty cameraViewPosProperty = serializedObject.FindProperty("cameraViewPosition");
        EditorGUILayout.PropertyField(cameraViewPosProperty);
        if (cameraViewPosProperty.enumValueIndex == 4)
        {
            SerializedProperty myVariableProperty3 = serializedObject.FindProperty("camView_customPosition");
            EditorGUILayout.PropertyField(myVariableProperty3);
        }

        EditorGUI.indentLevel--;

        EditorGUILayout.LabelField("\n");
        EditorGUILayout.LabelField("To finish the game session, use egUIManager.EndGame().", EditorStyles.label);

        EditorGUILayout.LabelField("\n");
        EditorGUILayout.LabelField("Other Variables", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;

        DrawPropertiesExcluding(serializedObject, "m_Script", "usePauseButton");

        serializedObject.ApplyModifiedProperties();
    }
}