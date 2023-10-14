using Dubi.RaycastExtension;
using System;
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

                if(!raycast.DrawGizmos)
                    continue;

                if (raycast.Hits.Length > 0)
                    for (int i = 0; i < raycast.Hits.Length; i++)
                        Handles.Label(raycast.Hits[i].point, i.ToString());

                /// if ray hits
                if (raycast.Valid)
                {
                    /// Draw grey line after hit
                    if (!raycast.RaycastAll)
                        RaycastGizmos.DrawNormal(raycast.Hit.point, raycast.worldDirection, Color.grey, Mathf.Max(0.0f, raycast.Distance - raycast.Hit.distance), 1.0f);

                    /// Draw ray before hit
                    if (raycast.Radius > 0.0f)
                    {
                        RaycastGizmos.DrawGizmoSphere(raycast.worldOrigin, raycast.Radius, raycast.GizmosColor);

                        if (raycast.RaycastAll)
                            RaycastGizmos.DrawWireCapsule(raycast.worldOrigin, raycast.worldOrigin + raycast.worldDirection * raycast.Distance, raycast.Radius, raycast.GizmosColor, 2.0f);
                        else
                            RaycastGizmos.DrawWireCapsule(raycast.worldOrigin, raycast.worldOrigin + raycast.worldDirection * raycast.Hit.distance, raycast.Radius, raycast.GizmosColor, 2.0f);
                    }
                    else
                    {
                        if (raycast.RaycastAll)
                            RaycastGizmos.DrawNormal(raycast.worldOrigin, raycast.worldDirection, raycast.GizmosColor, raycast.Distance, 2.0f);
                        else
                            RaycastGizmos.DrawNormal(raycast.worldOrigin, raycast.worldDirection, raycast.GizmosColor, raycast.Hit.distance, 2.0f);
                    }

                    /// Draw hit normal
                    if (raycast.RaycastAll)
                    {
                        foreach (RaycastHit hit in raycast.Hits)
                            RaycastGizmos.DrawRayCastHit(hit, raycast.GizmosColor);
                    }
                    else
                        RaycastGizmos.DrawRayCastHit(raycast.Hit, raycast.GizmosColor);

                    /// Draw hit collider
                    if (raycast.DrawColliderHit)
                    {
                        if (raycast.RaycastAll)
                        {
                            foreach (RaycastHit hit in raycast.Hits)
                                RaycastGizmos.DrawCollider(hit.collider, raycast.GizmosColor);
                        }
                        else
                            RaycastGizmos.DrawCollider(raycast.Hit.collider, raycast.GizmosColor);
                    }
                }
                else
                {
                    /// Draw ray without hit                    
                    if (raycast.Radius > 0.0f)
                    {
                        RaycastGizmos.DrawGizmoSphere(raycast.worldOrigin, raycast.Radius, raycast.GizmosColor);
                        RaycastGizmos.DrawWireCapsule(raycast.worldOrigin, raycast.worldOrigin + raycast.worldDirection * raycast.Distance, raycast.Radius, raycast.GizmosColor);
                    }
                    else
                    {
                        RaycastGizmos.DrawNormal(raycast.worldOrigin, raycast.worldDirection, raycast.GizmosColor, raycast.Distance);
                    }
                }
            }
        }
    }
}
