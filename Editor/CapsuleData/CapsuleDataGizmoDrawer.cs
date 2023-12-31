using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CapsuleDataGizmoDrawer : MonoBehaviour
{
    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy | GizmoType.Pickable)]
    static void FindAndDrawCapsuleData(MonoBehaviour target, GizmoType gizmoType)
    {
        if (!EditorApplication.isPlaying)
            return;

        System.Type type = target.GetType();

        var fields = type.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        foreach (var field in fields)
        {
            if (field.FieldType == typeof(CapsuleData))
            {
                var capsuleData = field.GetValue(target) as CapsuleData;

                if (capsuleData == null)
                    continue;

                if (!capsuleData.DrawGizmos)
                    continue;

                RaycastGizmos.DrawWireCapsule(capsuleData, capsuleData.GizmoColor, capsuleData.Overlapping ? 3.0f : 1.0f);

                /// Draw Overlapping Colliders
                using (new Handles.DrawingScope(capsuleData.GizmoColor))
                {
                    foreach (Collider col in capsuleData.OverlappingCollider())
                        RaycastGizmos.DrawCollider(col, capsuleData.GizmoColor);
                }

                /// Draw Origin
                using (new Handles.DrawingScope(capsuleData.GizmoColor))
                {
                    Vector3 cubeSize = Vector3.one * 0.01f;

                    switch (capsuleData.OriginType)
                    {
                        case Origin.InnerBottom:
                            Handles.DrawWireCube(capsuleData.InnerBottom, cubeSize);
                            break;
                        case Origin.InnerTop:
                            Handles.DrawWireCube(capsuleData.InnerTop, cubeSize);
                            break;
                        case Origin.Bottom:
                            Handles.DrawWireCube(capsuleData.Bottom, cubeSize);
                            break;
                        case Origin.Top:
                            Handles.DrawWireCube(capsuleData.Top, cubeSize);
                            break;
                        case Origin.Center:
                            Handles.DrawWireCube(capsuleData.Center, cubeSize);
                            break;

                    }
                }
            }
        }
    }
}
