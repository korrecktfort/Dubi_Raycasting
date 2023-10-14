using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;

public static class RaycastGizmos
{
#if UNITY_EDITOR
    static Color gCached, hCached;
    static Matrix4x4 gMatrixCached, hMatrixCached;

    static void CacheColor()
    {
        gCached = Gizmos.color;
        hCached = Handles.color;
    }

    static void ResetColor()
    {
        Gizmos.color = gCached;
        Handles.color = hCached;
    }
      

    static void CacheMatrix()
    {
        gMatrixCached = Gizmos.matrix;
        hMatrixCached = Handles.matrix;
    }

    static void SetMatrix(Matrix4x4 m)
    {
        Gizmos.matrix = m;
        Handles.matrix = m;
    }

    static void ResetMatrix()
    {
        Gizmos.matrix = gMatrixCached;
        Handles.matrix = hMatrixCached;
    }

    static void SetColor(this Color color)
    {
        Gizmos.color = color;
        Handles.color = color;
    }

    static Color Surface(this Color color)
    {
        color.a = 0.05f;
        return color;
    }

    static Color Outline(this Color color)
    {
        color.a = 0.45f;
        return color;
    }

    static Vector3 CamFWD()
    {
        Camera cam = SceneView.currentDrawingSceneView?.camera;

        if (cam == null)
            cam = Camera.current;

        if (cam == null)
            return Vector3.zero;

        return cam.transform.forward;
    }

    struct Line
    {
        public Line(Vector3 p0, Vector3 p1)
        {            
            this.forward = (p1 - p0).normalized;         
            float dot = Vector3.Dot(Vector3.up, this.forward);
            if (Mathf.Abs(dot) > 0.99999f)
                this.right = Vector3.Cross(Vector3.forward, this.forward).normalized;
            else
                this.right = Vector3.Cross(Vector3.up, this.forward).normalized;
            this.up = Vector3.Cross(this.forward, this.right);
        }
        
        public Vector3 forward;
        public Vector3 right;
        public Vector3 up;
    }   

    public static void DrawWireCapsule(CapsuleData capsuleData, Color c, float thickness = 0.0f)
    {
        CacheColor();
        c.Outline().SetColor();

        Vector3 p0 = capsuleData.InnerBottom;
        Vector3 p1 = capsuleData.InnerTop;
        Vector3 r = capsuleData.RightAxis;
        Vector3 u = capsuleData.UpAxis;
        Vector3 f = capsuleData.ForwardAxis;
        float rad = capsuleData.Radius;

        /// Arc Top
        Handles.DrawWireArc(p0, r, f, 180.0f, rad, thickness);
        Handles.DrawWireArc(p0, f, r, -180.0f, rad, thickness);
        Handles.DrawWireDisc(p0, u, rad, thickness);

        /// Arc Bottom
        Handles.DrawWireArc(p1, -r, f, 180.0f, rad, thickness);
        Handles.DrawWireArc(p1, -f, r, -180.0f, rad, thickness);
        Handles.DrawWireDisc(p1, u, rad, thickness);

        /// Lines
        Vector3 xOffset = r * rad;
        Vector3 zOffset = f * rad;

        Handles.DrawLine(p0 + xOffset, p1 + xOffset, thickness);
        Handles.DrawLine(p0 + -xOffset, p1 + -xOffset, thickness);

        Handles.DrawLine(p0 + zOffset, p1 + zOffset, thickness);
        Handles.DrawLine(p0 + -zOffset, p1 + -zOffset, thickness);

        ResetColor();
    }

    public static void DrawWireCapsule(Vector3 p0, Vector3 p1, float r, Color c, float thickness = 1.0f)
    {
        CacheColor();

        c.Outline().SetColor();
        float innerDistance = Vector3.Distance(p0, p1);
        bool atMin = Vector3.Distance(p0, p1) <= r * 2.0f && Vector3.Dot(Vector3.up, (p1 - p0).normalized) > 0.0f;

        if (p0 == p1 || atMin)
        {
            Handles.DrawWireDisc(p0, Vector3.forward, r, thickness);
            Handles.DrawWireDisc(p0, Vector3.right, r, thickness);
            Handles.DrawWireDisc(p0, Vector3.up, r, thickness);
            ResetColor();
            return;
        }        

        Line l = new Line(p0, p1);        

        /// Arc Top
        Handles.DrawWireArc(p0, l.up, l.right, 180.0f, r, thickness);
        Handles.DrawWireArc(p0, l.right, l.up, -180.0f, r, thickness);
        Handles.DrawWireDisc(p0, l.forward, r, thickness);

        /// Arc Bottom
        Handles.DrawWireArc(p1, -l.up, l.right, 180.0f, r, thickness);
        Handles.DrawWireArc(p1, -l.right, l.up, -180.0f, r, thickness);
        Handles.DrawWireDisc(p1, l.forward, r, thickness);

        /// Lines
        Vector3 xOffset = l.right * r;
        Vector3 zOffset = l.up * r;

        Handles.DrawLine(p0 + xOffset, p1 + xOffset, thickness);
        Handles.DrawLine(p0 + -xOffset, p1 + -xOffset, thickness);

        Handles.DrawLine(p0 + zOffset, p1 + zOffset, thickness);
        Handles.DrawLine(p0 + -zOffset, p1 + -zOffset, thickness);

        ResetColor();
    }

    public static void DrawSolidCapsule(Vector3 p0, Vector3 p1, float r, Color c, float thickness = 1.0f)
    {
        CacheColor();

        c.Surface().SetColor();
        bool atMin = Vector3.Distance(p0, p1) <= r * 2.0f && Vector3.Dot(Vector3.up, (p1 - p0).normalized) > 0.0f;

        if (p0 == p1 || atMin)
        {
            Gizmos.DrawSphere(p0, r);
            ResetColor();
            return;
        }

        Line l = new Line(p0, p1);

        /// Arc Top
        Handles.DrawSolidArc(p0, l.up, l.right, 180.0f, r);
        Handles.DrawSolidArc(p0, l.right, l.up, -180.0f, r);
        Handles.DrawSolidDisc(p0, l.forward, r);

        /// Arc Bottom
        Handles.DrawSolidArc(p1, -l.up, l.right, 180.0f, r);
        Handles.DrawSolidArc(p1, -l.right, l.up, -180.0f, r);
        Handles.DrawSolidDisc(p1, l.forward, r);

        /// Lines
        Vector3 xOffset = l.right * r;
        Vector3 zOffset = l.up * r;

        Handles.DrawSolidRectangleWithOutline(new Vector3[4]
        {
            p0 + xOffset,
            p1 + xOffset,
            p1 + -xOffset,
            p0 + -xOffset,
        }, Color.white, Color.clear);

        Handles.DrawSolidRectangleWithOutline(new Vector3[4]
        {
            p0 + zOffset,
            p1 + zOffset,
            p1 + -zOffset,
            p0 + -zOffset,
        }, Color.white, Color.clear);

        ResetColor();
    }

    public static void DrawGizmoSphere(Vector3 p0, float r, Color c)
    {
        CacheColor();

        Gizmos.color = c.Surface();
        Gizmos.DrawSphere(p0, r);

        Gizmos.color = c.Outline();
        Gizmos.DrawWireSphere(p0, r);

        ResetColor();
    }

    public static void DrawNormal(Vector3 origin, Vector3 dir, Color c, float distance = 1.0f, float thickness = 1.0f)
    {
        Vector3 p0 = origin;
        Vector3 p1 = origin + dir * distance;
        CacheColor();
        Handles.color = c;
        Handles.DrawLine(p0, p1, thickness);
        ResetColor();
    }

    public static void DrawHandleCube(Vector3 p, Vector3 s, Color c)
    {
        CacheColor();

        Handles.color = c.Surface();

        float x = p.x + s.x * 0.5f;
        float y = p.y + s.y * 0.5f;
        float z = p.z + s.z * 0.5f;

        Vector3 v0 = new Vector3(x, y, z);
        Vector3 v1 = new Vector3(x, y, -z);
        Vector3 v2 = new Vector3(-x, y, z);
        Vector3 v3 = new Vector3(-x, y, -z);
        Vector3 v4 = new Vector3(x, -y, z);
        Vector3 v5 = new Vector3(x, -y, -z);
        Vector3 v6 = new Vector3(-x, -y, z);
        Vector3 v7 = new Vector3(-x, -y, -z);

        Vector3[] front = new Vector3[4] { v2, v0, v4, v6 };
        Vector3[] right = new Vector3[4] { v1, v0, v4, v5 };
        Vector3[] up = new Vector3[4] { v1, v0, v2, v3 };

        Vector3[] nFront = new Vector3[4] { v3, v1, v5, v7 };
        Vector3[] nRight = new Vector3[4] { v3, v2, v6, v7 };
        Vector3[] nUp = new Vector3[4] { v5, v4, v6, v7 };

        Handles.DrawSolidRectangleWithOutline(front, c, Color.clear);
        Handles.DrawSolidRectangleWithOutline(right, c, Color.clear);
        Handles.DrawSolidRectangleWithOutline(up, c, Color.clear);

        Handles.DrawSolidRectangleWithOutline(nFront, c, Color.clear);
        Handles.DrawSolidRectangleWithOutline(nRight, c, Color.clear);
        Handles.DrawSolidRectangleWithOutline(nUp, c, Color.clear);

        Handles.color = c.Outline();
        Handles.DrawWireCube(p, s);

        ResetColor();
    }

    public static void DrawCollider(Collider collider, Color c)
    {
        CacheColor();
        CacheMatrix();

        SetMatrix(collider.transform.localToWorldMatrix);


        Color solid = c.Surface();
        Color lines = c.Outline();

        switch (collider)
        {
            case BoxCollider:
                BoxCollider boxCol = collider as BoxCollider;
                DrawHandleCube(boxCol.center, boxCol.size, c);
                break;

            case CapsuleCollider:
                CapsuleCollider capsCol = collider as CapsuleCollider;

                Vector3 ce = capsCol.center;
                float r = capsCol.radius;
                float d = capsCol.height * 0.5f - r;                
                Vector3 up = Vector3.up;
                Vector3 p0 = ce + d * up;
                Vector3 p1 = ce - d * up;
                                
                DrawSolidCapsule(p0, p1, r, c);                
                DrawWireCapsule(p0, p1, r, c);

                break;

            case SphereCollider:               
                SphereCollider sphereCol = collider as SphereCollider;

                DrawGizmoSphere(sphereCol.center, sphereCol.radius, c);                
                break;

            case MeshCollider:
                MeshCollider meshCol = collider as MeshCollider;
                Mesh mesh = meshCol.sharedMesh;                

                solid.SetColor();
                Gizmos.DrawMesh(mesh);

                lines.SetColor();
                Gizmos.DrawWireMesh(mesh);
                break;
        }

        ResetMatrix();
        ResetColor();
    }

    public static void DrawRayCastHit(RaycastHit hit, Color c)
    {
        Disc(hit.point, hit.normal, 0.1f, c, 2.0f);
        DrawNormal(hit.point, hit.normal, c, 0.2f, 2.0f);
    }

    public static void Disc(Vector3 position, Vector3 fwd, float radius, Color c, float thickness = 1.0f)
    {
        CacheColor();

        Handles.color = c.Surface();
        Handles.DrawSolidDisc(position, fwd, radius);

        Handles.color = c.Outline();
        Handles.DrawWireDisc(position, fwd, radius, thickness);

        ResetColor();
    }

    public static void Disc(float radius, Vector3 fwd, Color c, float thickness = 1.0f)
    {
        CacheColor();

        Handles.color = c.Surface();
        Handles.DrawSolidDisc(Vector3.zero, fwd, radius);

        Handles.color = c.Outline();
        Handles.DrawWireDisc(Vector3.zero, fwd, radius, thickness);

        ResetColor();
    }

    public static void Disc(float radius, Color c, float thickness = 1.0f)
    {
        Vector3 fwd = CamFWD();

        CacheColor();

        Handles.color = c.Surface();
        Handles.DrawSolidDisc(Vector3.zero, fwd, radius);

        Handles.color = c.Outline();
        Handles.DrawWireDisc(Vector3.zero, fwd, radius, thickness);

        ResetColor();
    }

    public static void DrawWireCapsule2D(CapsuleData2D capsuleData, Color c, float thickness = 0.0f)
    {
        CacheColor();
        c.Outline().SetColor();

        Vector3 forward = Vector3.forward;
        Vector3 right = capsuleData.Right;
        float radius = capsuleData.Radius;
        Vector3 p0 = capsuleData.InnerBottom;
        Vector3 p1 = capsuleData.InnerTop;
        Vector3 horOffset = right * capsuleData.Radius;
                
        Handles.DrawLine(p0 + horOffset, p1 + horOffset, thickness);
        Handles.DrawLine(p0 + -horOffset, p1 + -horOffset, thickness);

        Handles.DrawWireArc(p0, forward, right, 180.0f, radius, thickness);
        Handles.DrawWireArc(p1, forward, right, -180.0f, radius, thickness);

        c.Surface().SetColor();
        Handles.DrawSolidArc(p0, forward, right, 180.0f, radius);
        Handles.DrawSolidArc(p1, forward, right, -180.0f, radius);

        ResetColor();
    }

    public static void DiscArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius, float thickness, Color c)
    {
        CacheColor();
        c.Outline().SetColor();
        Handles.DrawWireArc(center, Vector3.up, Vector3.right, angle, radius, thickness);
        c.Surface().SetColor();
        Handles.DrawSolidArc(center, Vector3.up, Vector3.right, angle, radius);
        ResetColor();
    }
#endif
}
