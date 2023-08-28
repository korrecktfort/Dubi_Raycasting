using Dubi.RaycastExtension;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(Raycast))]
public class RaycastDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.BeginChangeCheck();
        float lineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        position.height = EditorGUIUtility.singleLineHeight;
                
        property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
        if (!property.isExpanded)
        {
            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
            return;
        }

        //position = EditorGUI.IndentedRect(position);

        EditorGUI.indentLevel++;

        /// Settings
        position.y += lineHeight;
        SerializedProperty distance = BaseValueHelper.GetValueProp(property.FindPropertyRelative("distance"));
        EditorGUI.PropertyField(position, property.FindPropertyRelative("distance"));      
        
        position.y += lineHeight;
        SerializedProperty radius = BaseValueHelper.GetValueProp(property.FindPropertyRelative("radius"));
        EditorGUI.PropertyField(position, property.FindPropertyRelative("radius"));
        position.y += lineHeight;
        SerializedProperty raycastAll = BaseValueHelper.GetValueProp(property.FindPropertyRelative("raycastAll"));
        EditorGUI.PropertyField(position, property.FindPropertyRelative("raycastAll"));
        position.y += lineHeight;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("layerMask"));
        position.y += lineHeight;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("triggerInteraction"));

        FloatMax(distance, 0.0f);

        if (raycastAll.boolValue)
        {
            SerializedProperty sortAlongRay = BaseValueHelper.GetValueProp(property.FindPropertyRelative("sortAlongRay"));
            position.y += lineHeight;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("sortAlongRay"));
            if (sortAlongRay.boolValue)
            {
                position.y += lineHeight;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("ascendingOrder"));
            }
        }

        FloatMax(radius, 0.0f);

        if(radius.floatValue > 0.0f)
        {
            position.y += lineHeight;
            /// Surface Normal Check           
            SerializedProperty surfaceNormal = BaseValueHelper.GetValueProp(property.FindPropertyRelative("surfaceNormal"));
            EditorGUI.PropertyField(position, property.FindPropertyRelative("surfaceNormal"));

            if (surfaceNormal.boolValue)
            {
                position.y += lineHeight;
                SerializedProperty useRayDir = BaseValueHelper.GetValueProp(property.FindPropertyRelative("useRayDir"));
                EditorGUI.PropertyField(position, property.FindPropertyRelative("useRayDir"));
                if (!useRayDir.boolValue)
                {
                    position.y += lineHeight;
                    SerializedProperty customCheckDirection = property.FindPropertyRelative("customCheckDir");
                    EditorGUI.PropertyField(position, customCheckDirection);
                    position.y += EditorGUI.GetPropertyHeight(customCheckDirection);
                    EditorGUI.PropertyField(position, property.FindPropertyRelative("isLocalCheckDir"));
                    position.y += lineHeight;
                    EditorGUI.PropertyField(position, property.FindPropertyRelative("useCustomSurfaceCheck"));
                }
            }
        }

        position.y += lineHeight;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("useInvalidLayer"));
        SerializedProperty useInvalidLayer = BaseValueHelper.GetValueProp(property.FindPropertyRelative("useInvalidLayer"));
        if (useInvalidLayer.boolValue)
        {
            position.y += lineHeight;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("invalidLayer"));
        }

        /// Debugging & Gizmos
        position.y += lineHeight;
        EditorGUI.LabelField(position, "Debugging", EditorStyles.boldLabel);
        position.y += lineHeight;
        SerializedProperty drawGizmos = BaseValueHelper.GetValueProp(property.FindPropertyRelative("drawGizmos"));
        if (drawGizmos.boolValue)
        {
            float x = position.x;
            float rectWidth = position.width;
            float width = EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth;
            position.width = width;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("drawGizmos"));

            position.x += position.width;
            position.width = rectWidth - position.width;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("color"), GUIContent.none);

            position.y += lineHeight;
            position.width = rectWidth;
            position.x = x;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("drawColliderHit"));
        }
        else
        {
            EditorGUI.PropertyField(position, property.FindPropertyRelative("drawGizmos"));
        }

        if (EditorGUI.EndChangeCheck())
            property.serializedObject.ApplyModifiedProperties();

        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();       
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        if (!property.isExpanded)
        {
            return lineHeight;
        }

        /// Standart Settings
        float height = 6.0f * lineHeight;

        /// Surface Normal Settings
        SerializedProperty radius = BaseValueHelper.GetValueProp(property.FindPropertyRelative("radius"));
        SerializedProperty surfaceNormal = BaseValueHelper.GetValueProp(property.FindPropertyRelative("surfaceNormal"));
        SerializedProperty useRayDir = BaseValueHelper.GetValueProp(property.FindPropertyRelative("useRayDir"));
        SerializedProperty drawGizmos = BaseValueHelper.GetValueProp(property.FindPropertyRelative("drawGizmos"));
        SerializedProperty raycastAll = BaseValueHelper.GetValueProp(property.FindPropertyRelative("raycastAll"));
        SerializedProperty customCheckDirection = BaseValueHelper.GetValueProp(property.FindPropertyRelative("customCheckDir"));
        SerializedProperty useInvalidLayer = BaseValueHelper.GetValueProp(property.FindPropertyRelative("useInvalidLayer"));

        if (raycastAll.boolValue)
        {
            height += lineHeight;
            if (BaseValueHelper.GetValueProp(property.FindPropertyRelative("sortAlongRay")).boolValue)
            {
                height += lineHeight;
            }
        }

        if(radius.floatValue > 0.0f)
        {
            height += lineHeight;

            if(surfaceNormal.boolValue)
            {
                height += lineHeight;

                if (!useRayDir.boolValue)
                {
                    height += lineHeight * 2.0f + EditorGUI.GetPropertyHeight(customCheckDirection);
                }
            }
        }

        height += lineHeight * (useInvalidLayer.boolValue ? 2.0f : 1.0f);

        /// Gizmos Settings
        height += lineHeight * 2.0f;
        if (drawGizmos.boolValue)
            height += lineHeight;       

        return height;
    }

    void FloatMax(SerializedProperty floatProp, float minValue)
    {
        EditorGUI.BeginChangeCheck();
        floatProp.floatValue = Mathf.Max(minValue, floatProp.floatValue);
        if (EditorGUI.EndChangeCheck())
            floatProp.serializedObject.ApplyModifiedProperties();
    }
}
