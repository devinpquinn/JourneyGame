using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Choice))]
public class ChoiceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Calculate rects
        Rect choiceTextRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        Rect nextNodeRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUIUtility.singleLineHeight);
        Rect isDiceCheckRect = new Rect(position.x, position.y + 2 * (EditorGUIUtility.singleLineHeight + 2), position.width, EditorGUIUtility.singleLineHeight);
        Rect abilityToCheckRect = new Rect(position.x, position.y + 3 * (EditorGUIUtility.singleLineHeight + 2), position.width, EditorGUIUtility.singleLineHeight);
        Rect nextNodeOnFailureRect = new Rect(position.x, position.y + 4 * (EditorGUIUtility.singleLineHeight + 2), position.width, EditorGUIUtility.singleLineHeight);
        Rect isLuckCheckRect = new Rect(position.x, position.y + 5 * (EditorGUIUtility.singleLineHeight + 2), position.width, EditorGUIUtility.singleLineHeight);

        // Draw fields
        EditorGUI.PropertyField(choiceTextRect, property.FindPropertyRelative("choiceText"));
        EditorGUI.PropertyField(nextNodeRect, property.FindPropertyRelative("nextNode"));
        EditorGUI.PropertyField(isDiceCheckRect, property.FindPropertyRelative("isDiceCheck"));

        // Conditionally draw abilityToCheck and nextNodeOnFailure
        if (property.FindPropertyRelative("isDiceCheck").boolValue)
        {
            EditorGUI.PropertyField(abilityToCheckRect, property.FindPropertyRelative("abilityToCheck"));
            EditorGUI.PropertyField(nextNodeOnFailureRect, property.FindPropertyRelative("nextNodeOnFailure"));
        }

        // Draw isLuckCheck field
        EditorGUI.PropertyField(isLuckCheckRect, property.FindPropertyRelative("isLuckCheck"));

        // Conditionally draw luck thresholds
        if (property.FindPropertyRelative("isLuckCheck").boolValue)
        {
            SerializedProperty luckThresholds = property.FindPropertyRelative("luckThresholds");
            for (int i = 0; i < luckThresholds.arraySize; i++)
            {
                Rect thresholdRect = new Rect(position.x, position.y + (6 + i * 2) * (EditorGUIUtility.singleLineHeight + 2), position.width, EditorGUIUtility.singleLineHeight);
                Rect nodeRect = new Rect(position.x, position.y + (7 + i * 2) * (EditorGUIUtility.singleLineHeight + 2), position.width, EditorGUIUtility.singleLineHeight);

                EditorGUI.PropertyField(thresholdRect, luckThresholds.GetArrayElementAtIndex(i).FindPropertyRelative("threshold"));
                EditorGUI.PropertyField(nodeRect, luckThresholds.GetArrayElementAtIndex(i).FindPropertyRelative("node"));
            }

            if (GUI.Button(new Rect(position.x, position.y + (6 + luckThresholds.arraySize * 2) * (EditorGUIUtility.singleLineHeight + 2), position.width, EditorGUIUtility.singleLineHeight), "Add Threshold"))
            {
                luckThresholds.InsertArrayElementAtIndex(luckThresholds.arraySize);
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int lines = 6; // Base lines for choiceText, nextNode, isDiceCheck, isLuckCheck
        if (property.FindPropertyRelative("isDiceCheck").boolValue)
        {
            lines += 2; // Additional lines for abilityToCheck and nextNodeOnFailure
        }
        if (property.FindPropertyRelative("isLuckCheck").boolValue)
        {
            lines += property.FindPropertyRelative("luckThresholds").arraySize * 2 + 1; // Additional lines for each luck threshold and the "Add Threshold" button
        }
        return lines * (EditorGUIUtility.singleLineHeight + 2);
    }
}
