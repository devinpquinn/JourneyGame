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

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int lines = property.FindPropertyRelative("isDiceCheck").boolValue ? 5 : 3;
        return lines * (EditorGUIUtility.singleLineHeight + 2);
    }
}
