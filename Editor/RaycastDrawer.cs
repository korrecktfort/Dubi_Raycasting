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
       
        position.height = EditorGUIUtility.singleLineHeight;
                
        property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
        if (!property.isExpanded)
        {
            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
            return;
        }

        SerializedProperty originTransform = property.FindPropertyRelative("originTransform");
        SerializedProperty localOffset = property.FindPropertyRelative("localOffset");
        SerializedProperty localDirection = property.FindPropertyRelative("localDirection");
        SerializedProperty distance = property.FindPropertyRelative("distance");
        SerializedProperty radius = property.FindPropertyRelative("radius");
        SerializedProperty raycastAll = property.FindPropertyRelative("raycastAll");
        SerializedProperty layerMask = property.FindPropertyRelative("layerMask");
        SerializedProperty triggerInteraction = property.FindPropertyRelative("triggerInteraction");
        SerializedProperty sortAlongRay = property.FindPropertyRelative("sortAlongRay");
        SerializedProperty ascendingOrder = property.FindPropertyRelative("ascendingOrder");
        SerializedProperty surfaceNormal = property.FindPropertyRelative("surfaceNormal");
        SerializedProperty useRayDir = property.FindPropertyRelative("useRayDir");
        SerializedProperty customCheckDirection = property.FindPropertyRelative("customCheckDir");
        SerializedProperty isLocalCheckDir = property.FindPropertyRelative("isLocalCheckDir");
        SerializedProperty useCustomSurfaceCheck = property.FindPropertyRelative("useCustomSurfaceCheck");
        SerializedProperty useInvalidLayer = property.FindPropertyRelative("useInvalidLayer");
        SerializedProperty invalidLayer = property.FindPropertyRelative("invalidLayer");
        SerializedProperty drawGizmos = property.FindPropertyRelative("drawGizmos");
        SerializedProperty color = property.FindPropertyRelative("color");
        SerializedProperty drawColliderHit = property.FindPropertyRelative("drawColliderHit");

        EditorGUI.indentLevel++;
        this.lastHeight = position.height;        
        AddPropertyField(ref position, originTransform, "The world origin of the Raycast. It will align to its position before casting.");
        AddPropertyField(ref position, localOffset, "The local offset added to the origin transform position before casting.");
        AddPropertyField(ref position, localDirection, "The local direction of the Raycast. It will align before casting.");
        AddPropertyField(ref position, distance, "The distance of the Raycast. It clamps to 0.0f.");
        FloatMax(BaseValueHelper.GetValueProp(distance), 0.0f);
        
        /// Radius options
        AddPropertyField(ref position, radius, "The radius of the Raycast. It clamps to 0.0f.");
        FloatMax(BaseValueHelper.GetValueProp(radius), 0.0f);
        if (BaseValueHelper.GetValueProp(radius).floatValue > 0.0f)
        {
            EditorGUI.indentLevel++;
            AddPropertyField(ref position, surfaceNormal, "If true, it will check the surface normal of the hit collider.");
            if (BaseValueHelper.GetValueProp(surfaceNormal).boolValue)
            {
                AddPropertyField(ref position, useRayDir, "Uses the ray hit normal for surface checks. Radial ray hits have a normal based on the angle the radius hit the collider, this can be overridden with more options here.");
                if (!BaseValueHelper.GetValueProp(useRayDir).boolValue)
                {
                    AddPropertyField(ref position, customCheckDirection, "The custom check direction of the Raycast.");
                    AddPropertyField(ref position, isLocalCheckDir, "If true, it will use the check direction above as a local direction value.");
                    AddPropertyField(ref position, useCustomSurfaceCheck, "If true, it will use the defined custom check method for surface checks. See Raycast.SurfaceNormalManualRaycast(RaycastHit) for how it is done.");
                }
            }
            EditorGUI.indentLevel--;
        }

        /// Raycast all options
        AddPropertyField(ref position, raycastAll, "If true, it will cast all the colliders in distance range. Otherwise, it will cast the first collider hit.");
        if (BaseValueHelper.GetValueProp(raycastAll).boolValue)
        {
            EditorGUI.indentLevel++;
            AddPropertyField(ref position, sortAlongRay, "If true, it will sort the hit colliders along the raycast direction.");
            if (BaseValueHelper.GetValueProp(sortAlongRay).boolValue)
            {
                AddPropertyField(ref position, ascendingOrder, "If true, it will sort the hit colliders in ascending order.");
            }
            EditorGUI.indentLevel--;
        }

        AddPropertyField(ref position, layerMask, "The layer mask of the Raycast.");
        AddPropertyField(ref position, triggerInteraction, "The trigger interaction of the Raycast.");
        


        /// Invalid layer options
        AddPropertyField(ref position, useInvalidLayer, "If true, it will make the cast result invalid if the layer has been hit.");
        if (BaseValueHelper.GetValueProp(useInvalidLayer).boolValue)
        {
            AddPropertyField(ref position, invalidLayer, "The invalid layer of the Raycast.");
        }

        /// Draw options
        AddPropertyField(ref position, drawGizmos, "If true, it will draw the Raycast gizmos.");
        if (BaseValueHelper.GetValueProp(drawGizmos).boolValue)
        {
            EditorGUI.indentLevel++;
            AddPropertyField(ref position, color, "The color of the Raycast gizmos.");
            AddPropertyField(ref position, drawColliderHit, "If true, it will draw the collider hit gizmos.");
            EditorGUI.indentLevel--;
        }

        if (EditorGUI.EndChangeCheck())
            property.serializedObject.ApplyModifiedProperties();

        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();       
    }

    float lastHeight = 0.0f;
    void AddPropertyField(ref Rect position, SerializedProperty property, string toolTip)
    {
        position.y += this.lastHeight + EditorGUIUtility.standardVerticalSpacing;
        position.height = EditorGUI.GetPropertyHeight(property);
        EditorGUI.PropertyField(position, property, new GUIContent(property.displayName, toolTip));
        this.lastHeight = position.height;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        if (!property.isExpanded)
        {
            return lineHeight;
        }

        /// Standart Settings
        float height = 8.0f * lineHeight;

        /// Surface Normal Settings
        SerializedProperty radius = property.FindPropertyRelative("radius");
        SerializedProperty sortAlongRay = property.FindPropertyRelative("sortAlongRay");
        SerializedProperty surfaceNormal = property.FindPropertyRelative("surfaceNormal");
        SerializedProperty useRayDir = property.FindPropertyRelative("useRayDir");
        SerializedProperty drawGizmos = property.FindPropertyRelative("drawGizmos");
        SerializedProperty raycastAll = property.FindPropertyRelative("raycastAll");
        SerializedProperty customCheckDirection = property.FindPropertyRelative("customCheckDir");
        SerializedProperty useInvalidLayer = property.FindPropertyRelative("useInvalidLayer");

        if (BaseValueHelper.GetValueProp(raycastAll).boolValue)
        {
            height += lineHeight;
            if (BaseValueHelper.GetValueProp(sortAlongRay).boolValue)
            {
                height += lineHeight;
            }
        }

        if(BaseValueHelper.GetValueProp(radius).floatValue > 0.0f)
        {
            height += lineHeight;

            if(BaseValueHelper.GetValueProp(surfaceNormal).boolValue)
            {
                height += lineHeight;

                if (!BaseValueHelper.GetValueProp(useRayDir).boolValue)
                {
                    height += lineHeight * 2.0f + EditorGUI.GetPropertyHeight(customCheckDirection);
                }
            }
        }

        height += lineHeight * (BaseValueHelper.GetValueProp(useInvalidLayer).boolValue ? 2.0f : 1.0f);

        /// Gizmos Settings
        height += lineHeight * 2.0f;
        if (BaseValueHelper.GetValueProp(drawGizmos).boolValue)
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
