using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class CapsuleData2D
{
    Vector2 center, top, bottom, innerTop, innerBottom, pinchedBottom, pinchedTop, bottomOffset, topOffset, right, up;
    float radius, height, halfHeight, pinchedRadius;
    Origin originType = Origin.Center;
    bool overlapping = false;

    public bool Overlapping => this.overlapping;

#if UNITY_EDITOR
    [SerializeField] bool drawGizmos = false;
    [SerializeField] Color gizmoColor = Color.white;
    public bool DrawGizmos => this.drawGizmos;
    public Color GizmoColor => this.gizmoColor;
#endif

    public Vector2 Up
    {
        get => this.up;

        set
        {
            this.up = value.normalized;
            this.right = -Vector2.Perpendicular(this.up);
            AxisChanged();
        }
    }

    public Vector2 Right
    {
        get => this.right;

        set
        {
            this.right = value.normalized;
            this.up = Vector2.Perpendicular(this.right);
            AxisChanged();
        }
    }

    public Origin OriginType
    {
        get => this.originType;
        set
        {
            if (this.originType == value)
                return;

            this.originType = value;
            AxisChanged();
        }
    }

    public Vector2 Top
    {
        get => this.top;
        set
        {
            this.top = value;
            this.bottom = this.top - this.up * this.height;
            this.center = this.bottom + this.up * this.halfHeight;
            this.innerBottom = this.bottom + this.up * this.radius;
            this.innerTop = this.top - this.up * this.radius;

            ApplyOffsets();
        }
    }

    public Vector2 Center
    {
        get => this.center;
        set
        {
            this.center = value;
            this.bottom = this.center - this.up * this.halfHeight;
            this.top = this.center + this.up * this.halfHeight;
            this.innerBottom = this.bottom + this.up * this.radius;
            this.innerTop = this.top - this.up * this.radius;
            ApplyOffsets();
        }
    }

    public Vector2 Bottom
    {
        get => this.bottom;
        set
        {
            this.bottom = value;
            this.top = this.bottom + this.up * this.height;
            this.center = this.bottom + this.up * this.halfHeight;
            this.innerBottom = this.bottom + this.up * this.radius;
            this.innerTop = this.top - this.up * this.radius;
            ApplyOffsets();
        }
    }

    public Vector2 InnerTop
    {
        get => this.innerTop;
        set
        {
            this.innerTop = value;
            this.top = this.innerTop + this.up * this.radius;
            this.bottom = this.top - this.up * this.height;
            this.center = this.bottom + this.up * this.halfHeight;
            this.innerBottom = this.bottom + this.up * this.radius;
            ApplyOffsets();
        }
    }

    public Vector2 InnerBottom
    {
        get => this.innerBottom;
        set
        {
            this.innerBottom = value;
            this.bottom = this.innerBottom - this.up * this.radius;
            this.top = this.bottom + this.up * this.height;
            this.center = this.bottom + this.up * this.halfHeight;
            this.innerTop = this.top - this.up * this.radius;
            ApplyOffsets();
        }
    }

    public float Radius
    {
        get => this.radius;
        set
        {
            this.radius = value;
            AxisChanged();
        }
    }

    public float Height
    {
        get => this.height;
        set
        {
            this.height = value;
            this.halfHeight = this.height * 0.5f;
            AxisChanged();
        }
    }

    public CapsuleData2D() { }

    public CapsuleData2D(Vector2 center, Vector2 up, float radius, float height)
    {
        SetCapsuleData(center, up, radius, height);
    }

    public CapsuleData2D(Vector2 bottom, float height, Vector2 up, float radius)
    {
        SetCapsuleData(bottom, height, up, radius);
    }

    public CapsuleData2D(Vector2 top, Vector2 bottom, float radius)
    {
        SetCapsuleData(top, bottom, radius);
    }

    public CapsuleData2D(CapsuleCollider2D col)
    {
        SetCapsuleData(col);
    }

    public void SetCapsuleData(Vector2 center, Vector2 up, float radius, float height)
    {
        this.up = up.normalized;
        this.right = -Vector2.Perpendicular(this.up);
        this.radius = radius;
        this.height = height;
        this.halfHeight = this.height * 0.5f;

        Vector2 offset = this.up * this.halfHeight;
        Vector2 radiusOffset = this.up * this.radius;

        this.center = center;
        this.bottom = this.center - offset;
        this.top = this.center + offset;
        this.innerBottom = this.bottom + radiusOffset;
        this.innerTop = this.top - radiusOffset;

        ApplyOffsets();
    }

    public void SetCapsuleData(Vector2 bottom, float height, Vector2 up, float radius)
    {
        this.up = up.normalized;
        this.right = -Vector2.Perpendicular(this.up);
        this.radius = radius;
        this.height = height;
        this.halfHeight = this.height * 0.5f;

        Vector2 offset = this.up * this.halfHeight;
        Vector2 radiusOffset = this.up * this.radius;

        this.bottom = bottom;
        this.center = this.bottom + offset;
        this.top = this.center + offset;
        this.innerBottom = this.bottom + radiusOffset;
        this.innerTop = this.top - radiusOffset;

        ApplyOffsets();
    }

    public void SetCapsuleData(Vector2 top, Vector2 bottom, float radius)
    {
        this.up = (top - bottom).normalized;
        this.right = -Vector2.Perpendicular(this.up);

        this.radius = radius;
        this.height = Vector2.Distance(top, bottom);
        this.halfHeight = this.height * 0.5f;

        Vector2 offset = this.up * this.halfHeight;
        Vector2 radiusOffset = this.up * this.radius;

        this.top = top;
        this.bottom = bottom;
        this.center = this.bottom + offset;
        this.innerBottom = this.bottom + radiusOffset;
        this.innerTop = this.top - radiusOffset;

        ApplyOffsets();
    }

    public void SetCapsuleData(CapsuleCollider2D col)
    {
        Transform colTransform = col.transform;
        switch (col.direction)
        {
            case CapsuleDirection2D.Vertical:
                this.up = colTransform.up;
                this.right = colTransform.right;
                break;

            case CapsuleDirection2D.Horizontal:
                this.up = colTransform.right;
                this.right = -colTransform.up;
                break;
        }

        this.radius = col.size.x * 0.5f;
        this.height = col.size.y;
        this.halfHeight = this.height * 0.5f;

        Vector2 offset = this.up * this.halfHeight;
        Vector2 radiusOffset = this.up * this.radius;

        this.center = colTransform.TransformPoint(col.offset);
        this.bottom = this.center - offset;
        this.top = this.center + offset;
        this.innerBottom = this.bottom + radiusOffset;
        this.innerTop = this.top - radiusOffset;

        ApplyOffsets();
    }

    void AxisChanged()
    {
        DeplyOffsets();

        Vector2 heightOffset = this.up * this.height;
        Vector2 halfOffset = this.up * this.halfHeight;
        Vector2 radiusOffset = this.up * this.radius;

        switch (this.originType)
        {
            case Origin.Bottom:
                this.center = this.bottom + halfOffset;
                this.top = this.bottom + heightOffset;
                this.innerBottom = this.bottom + radiusOffset;
                this.innerTop = this.top - radiusOffset;
                break;

            case Origin.Center:
                this.bottom = this.center - halfOffset;
                this.top = this.center + halfOffset;
                this.innerBottom = this.bottom + radiusOffset;
                this.innerTop = this.top - radiusOffset;
                break;

            case Origin.Top:
                this.center = this.top - halfOffset;
                this.bottom = this.top - heightOffset;
                this.innerBottom = this.bottom + radiusOffset;
                this.innerTop = this.top - radiusOffset;
                break;

            case Origin.InnerBottom:
                this.bottom = this.innerBottom - radiusOffset;
                this.top = this.bottom + heightOffset;
                this.center = this.bottom + halfOffset;
                this.innerTop = this.top - radiusOffset;
                break;

            case Origin.InnerTop:
                this.top = this.innerTop + radiusOffset;
                this.bottom = this.top - heightOffset;
                this.center = this.bottom + halfOffset;
                this.innerBottom = this.bottom + radiusOffset;
                break;
        }

        ApplyOffsets();
    }

    void DeplyOffsets()
    {
        if(this.bottomOffset != Vector2.zero)
        {
            this.bottom -= this.bottomOffset;
            this.innerBottom -= this.bottomOffset;
        }

        if(this.topOffset != Vector2.zero)
        {
            this.top -= this.topOffset;
            this.innerTop -= this.topOffset;
        }
    }

    void ApplyOffsets()
    {
        if(this.bottomOffset != Vector2.zero)
        {
            this.bottom += this.bottomOffset;
            this.innerBottom += this.bottomOffset;
        }

        if(this.topOffset != Vector2.zero)
        {
            this.top += this.topOffset;
            this.innerTop += this.topOffset;
        }
    }

    public bool Overlaps()
    {
        Vector2 size = new Vector2(this.radius * 2.0f, this.height);
        this.overlapping = Physics2D.OverlapCapsule(this.center, size, CapsuleDirection2D.Vertical, Vector2.Angle(this.up, Vector2.up));
        return this.overlapping;
    }

    public bool Overlaps(LayerMask layerMask)
    {
        Vector2 size = new Vector2(this.radius * 2.0f, this.height);
        this.overlapping = Physics2D.OverlapCapsule(this.center, size, CapsuleDirection2D.Vertical, Vector2.Angle(this.up, Vector2.up), layerMask);
        return this.overlapping;
    }

    public Collider2D[] OverlappingCollider()
    {
        Vector2 size = new Vector2(this.radius * 2.0f, this.height);
        return Physics2D.OverlapCapsuleAll(this.center, size, CapsuleDirection2D.Vertical, Vector2.Angle(this.up, Vector2.up));
    }

    public Collider2D[] OverlappingCollider(LayerMask layerMask)
    {
        Vector2 size = new Vector2(this.radius * 2.0f, this.height);
        return Physics2D.OverlapCapsuleAll(this.center, size, CapsuleDirection2D.Vertical, Vector2.Angle(this.up, Vector2.up), layerMask);
    }   
}
