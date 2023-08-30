using Dubi.RaycastExtension;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class RaycastGizmoDrawer : MonoBehaviour
{
    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy | GizmoType.Pickable)]
    static void FindAndDrawRaycast(MonoBehaviour target, GizmoType gizmoType)
    {
        if (!EditorApplication.isPlaying)
            return;

        Type type = target.GetType();

        var fields = type.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        foreach (var field in fields)
        {
            if (field.FieldType == typeof(Raycast))
            {
                var raycast = field.GetValue(target) as Raycast;
                
                if (raycast == null)
                    continue;

                raycast.OnDrawGizmos();
            }
        }
    }
}
