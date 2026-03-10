using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EventNodeData))]
public class EventNodeDataDrawer : PropertyDrawer
{
    private const float VerticalSpacing = 2f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect contentRect = EditorGUI.IndentedRect(position);
        Rect foldoutRect = new Rect(contentRect.x, contentRect.y, contentRect.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);

        if (property.isExpanded)
        {
            SerializedProperty nodeIdProperty = property.FindPropertyRelative("nodeId");
            SerializedProperty bodyTextProperty = property.FindPropertyRelative("bodyText");
            SerializedProperty effectsProperty = property.FindPropertyRelative("effects");
            SerializedProperty testAttributeProperty = property.FindPropertyRelative("testAttribute");
            SerializedProperty successNodeIdProperty = property.FindPropertyRelative("successNodeId");
            SerializedProperty failureNodeIdProperty = property.FindPropertyRelative("failureNodeId");
            SerializedProperty nextNodeIdProperty = property.FindPropertyRelative("nextNodeId");

            float y = foldoutRect.yMax + VerticalSpacing;

            DrawChildProperty(contentRect, nodeIdProperty, ref y);
            DrawChildProperty(contentRect, bodyTextProperty, ref y);
            DrawChildProperty(contentRect, effectsProperty, ref y);
            DrawChildProperty(contentRect, testAttributeProperty, ref y);

            bool hasTest = testAttributeProperty.enumValueIndex != (int)HeroAttribute.None;
            if (hasTest)
            {
                DrawChildProperty(contentRect, successNodeIdProperty, ref y);
                DrawChildProperty(contentRect, failureNodeIdProperty, ref y);
            }
            else
            {
                DrawChildProperty(contentRect, nextNodeIdProperty, ref y);
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight;

        if (!property.isExpanded)
        {
            return height;
        }

        SerializedProperty nodeIdProperty = property.FindPropertyRelative("nodeId");
        SerializedProperty bodyTextProperty = property.FindPropertyRelative("bodyText");
        SerializedProperty effectsProperty = property.FindPropertyRelative("effects");
        SerializedProperty testAttributeProperty = property.FindPropertyRelative("testAttribute");
        SerializedProperty successNodeIdProperty = property.FindPropertyRelative("successNodeId");
        SerializedProperty failureNodeIdProperty = property.FindPropertyRelative("failureNodeId");
        SerializedProperty nextNodeIdProperty = property.FindPropertyRelative("nextNodeId");

        height += VerticalSpacing + EditorGUI.GetPropertyHeight(nodeIdProperty, true);
        height += VerticalSpacing + EditorGUI.GetPropertyHeight(bodyTextProperty, true);
        height += VerticalSpacing + EditorGUI.GetPropertyHeight(effectsProperty, true);
        height += VerticalSpacing + EditorGUI.GetPropertyHeight(testAttributeProperty, true);

        bool hasTest = testAttributeProperty.enumValueIndex != (int)HeroAttribute.None;
        if (hasTest)
        {
            height += VerticalSpacing + EditorGUI.GetPropertyHeight(successNodeIdProperty, true);
            height += VerticalSpacing + EditorGUI.GetPropertyHeight(failureNodeIdProperty, true);
        }
        else
        {
            height += VerticalSpacing + EditorGUI.GetPropertyHeight(nextNodeIdProperty, true);
        }

        return height;
    }

    private static void DrawChildProperty(Rect contentRect, SerializedProperty property, ref float y)
    {
        float height = EditorGUI.GetPropertyHeight(property, true);
        Rect rect = new Rect(contentRect.x, y, contentRect.width, height);
        EditorGUI.PropertyField(rect, property, true);
        y += height + VerticalSpacing;
    }
}

[CustomPropertyDrawer(typeof(EventEffect))]
public class EventEffectDrawer : PropertyDrawer
{
    private const float VerticalSpacing = 2f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect contentRect = EditorGUI.IndentedRect(position);
        Rect foldoutRect = new Rect(contentRect.x, contentRect.y, contentRect.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);

        if (property.isExpanded)
        {
            SerializedProperty targetProperty = property.FindPropertyRelative("target");
            SerializedProperty amountProperty = property.FindPropertyRelative("amount");
            SerializedProperty eventToAddProperty = property.FindPropertyRelative("eventToAdd");

            float y = foldoutRect.yMax + VerticalSpacing;
            DrawChildProperty(contentRect, targetProperty, ref y);

            bool isUnlockEvent = targetProperty.enumValueIndex == (int)HeroEffectTarget.UnlockEvent;
            if (isUnlockEvent)
            {
                DrawChildProperty(contentRect, eventToAddProperty, ref y);
            }
            else
            {
                DrawChildProperty(contentRect, amountProperty, ref y);
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight;

        if (!property.isExpanded)
        {
            return height;
        }

        SerializedProperty targetProperty = property.FindPropertyRelative("target");
        SerializedProperty amountProperty = property.FindPropertyRelative("amount");
        SerializedProperty eventToAddProperty = property.FindPropertyRelative("eventToAdd");

        height += VerticalSpacing + EditorGUI.GetPropertyHeight(targetProperty, true);

        bool isUnlockEvent = targetProperty.enumValueIndex == (int)HeroEffectTarget.UnlockEvent;
        if (isUnlockEvent)
        {
            height += VerticalSpacing + EditorGUI.GetPropertyHeight(eventToAddProperty, true);
        }
        else
        {
            height += VerticalSpacing + EditorGUI.GetPropertyHeight(amountProperty, true);
        }

        return height;
    }

    private static void DrawChildProperty(Rect contentRect, SerializedProperty property, ref float y)
    {
        float height = EditorGUI.GetPropertyHeight(property, true);
        Rect rect = new Rect(contentRect.x, y, contentRect.width, height);
        EditorGUI.PropertyField(rect, property, true);
        y += height + VerticalSpacing;
    }
}
