using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CapsuleData2DGizmoDrawer : MonoBehaviour
{
    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy | GizmoType.Pickable)]
    static void FindAndDrawCapsuleData2D(MonoBehaviour target, GizmoType gizmoType)
    {
        if(!EditorApplication.isPlaying)
            return;

        System.Type type = target.GetType();

        var fields = type.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        foreach( var field in fields)
        {
            if(field.FieldType == typeof(CapsuleData2D))
            {
                CapsuleData2D capsuleData = field.GetValue(target) as CapsuleData2D;
                
                if(capsuleData == null)
                    continue;

                if(!capsuleData.DrawGizmos)
                    continue;

                using (new Handles.DrawingScope(capsuleData.GizmoColor))
                {
                    RaycastGizmos.DrawWireCapsule2D(capsuleData, capsuleData.GizmoColor, capsuleData.Overlapping ? 3.0f : 1.0f);
                }
            }
        }
    }
}
