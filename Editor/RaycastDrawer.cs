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
        
        position = EditorGUI.IndentedRect(position);

        /// Settings
        position.y += lineHeight;
        SerializedProperty distance = property.FindPropertyRelative("distance");
        EditorGUI.PropertyField(position, distance);      
        
        position.y += lineHeight;
        SerializedProperty radius = property.FindPropertyRelative("radius");
        EditorGUI.PropertyField(position, radius);
        position.y += lineHeight;
        SerializedProperty raycastAll = property.FindPropertyRelative("raycastAll");
        EditorGUI.PropertyField(position, raycastAll);
        position.y += lineHeight;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("layerMask"));
        position.y += lineHeight;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("triggerInteraction"));

        SerializedProperty distanceValue = BaseValueHelper.GetValueProp(distance);
        FloatMax(distanceValue, 0.0f);

        if (raycastAll.boolValue)
        {
            SerializedProperty sortAlongRay = property.FindPropertyRelative("sortAlongRay");
            position.y += lineHeight;
            EditorGUI.PropertyField(position, sortAlongRay);
            if (sortAlongRay.boolValue)
            {
                position.y += lineHeight;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("ascendingOrder"));
            }
        }

        SerializedProperty radiusValue = BaseValueHelper.GetValueProp(radius);
        FloatMax(radiusValue, 0.0f);

        if(radiusValue.floatValue > 0.0f)
        {
            position.y += lineHeight;
            /// Surface Normal Check           
            SerializedProperty surfaceNormal = property.FindPropertyRelative("surfaceNormal");
            EditorGUI.PropertyField(position, surfaceNormal);

            if (surfaceNormal.boolValue)
            {
                position.y += lineHeight;
                SerializedProperty useRayDir = property.FindPropertyRelative("useRayDir");
                EditorGUI.PropertyField(position, useRayDir);
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

        SerializedProperty useInvalidLayer = property.FindPropertyRelative("useInvalidLayer");
        position.y += lineHeight;
        EditorGUI.PropertyField(position, useInvalidLayer);
        if (useInvalidLayer.boolValue)
        {
            position.y += lineHeight;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("invalidLayer"));
        }

        /// Debugging & Gizmos
        position.y += lineHeight;
        EditorGUI.LabelField(position, "Debugging", EditorStyles.boldLabel);
        position.y += lineHeight;
        SerializedProperty drawGizmos = property.FindPropertyRelative("drawGizmos");
        if (drawGizmos.boolValue)
        {
            float x = position.x;
            float rectWidth = position.width;
            float width = EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth;
            position.width = width;
            EditorGUI.PropertyField(position, drawGizmos);

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
            EditorGUI.PropertyField(position, drawGizmos);
        }

        if (EditorGUI.EndChangeCheck())
            property.serializedObject.ApplyModifiedProperties();
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
        SerializedProperty radius = property.FindPropertyRelative("radius");
        SerializedProperty radiusValue = BaseValueHelper.GetValueProp(radius);
        SerializedProperty surfaceNormal = property.FindPropertyRelative("surfaceNormal");
        SerializedProperty useRayDir = property.FindPropertyRelative("useRayDir");
        SerializedProperty drawGizmos = property.FindPropertyRelative("drawGizmos");
        SerializedProperty raycastAll = property.FindPropertyRelative("raycastAll");
        SerializedProperty customCheckDirection = property.FindPropertyRelative("customCheckDir");
        SerializedProperty useInvalidLayer = property.FindPropertyRelative("useInvalidLayer");

        if (raycastAll.boolValue)
        {
            height += lineHeight;
            if (property.FindPropertyRelative("sortAlongRay").boolValue)
            {
                height += lineHeight;
            }
        }

        if(radiusValue.floatValue > 0.0f)
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
