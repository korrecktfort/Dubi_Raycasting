using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CapsuleData
{
    public enum Origin
    {
        Top,
        InnerTop,
        Center,
        InnerBottom,
        Bottom,
    }

    Vector3 top, center, bottom, rightAxis, upAxis, forwardAxis, innerTop, innerBottom, topOffset, bottomOffset;
    float height, halfHeight, radius;
    Origin originType = Origin.Center;
    bool overlapping = false;

    public bool Overlapping => this.overlapping;

#if UNITY_EDITOR
    [SerializeField] bool drawGizmos = false;
    [SerializeField] Color color = Color.white;
    public bool DrawGizmos => this.drawGizmos;
    public Color GizmoColor => this.color;
#endif

    public Vector3 Top 
    { 
        get => this.top;
        set => SetCapsuleData(value, this.bottom, this.radius);        
    }

    public Vector3 Center 
    { 
        get => this.center;
        set => SetCapsuleData(value, this.upAxis, this.height, this.radius);        
    }

    public Vector3 Bottom 
    { 
        get => this.bottom;
        set => SetCapsuleData(this.top, value, this.radius);        
    }

    public Vector3 InnerTop 
    { 
        get => this.innerTop;
        set => SetCapsuleData(this.radius, value, this.innerBottom);        
    }

    public Vector3 InnerBottom 
    { 
        get => this.innerBottom;
        set => SetCapsuleData(this.radius, this.innerTop, value);
    }

    public float Height 
    { 
        get => height; 
        set => SetCapsuleData(this.center, this.upAxis, value, this.radius); 
    }

    public float TopOffset 
    {
        set
        {
            this.topOffset = this.upAxis * value;           
            SetCapsuleData(this.center, this.upAxis, this.height, this.radius);            
        }
    }

    public float BottomOffset 
    {
        set
        {
            this.bottomOffset = this.upAxis * value;
            SetCapsuleData(this.center, this.upAxis, this.height, this.radius);
        }
    }

    public float Radius 
    { 
        get => radius; 
        set => SetCapsuleData(value, this.innerTop, this.innerBottom); 
    }

    public float HalfHeight 
    { 
        get => halfHeight; 
        set => SetCapsuleData(this.bottom, value * 2.0f, this.upAxis, this.radius); 
    }

    public Vector3 UpAxis 
    { 
        get => this.upAxis; 
        set 
        {
            this.upAxis = value.normalized;
            float dot = Vector3.Dot(Vector3.up, this.upAxis);

            if (Mathf.Abs(dot) > 0.99999f)
                this.forwardAxis = Vector3.Cross(Vector3.right, this.upAxis).normalized;
            else
                this.forwardAxis = Vector3.Cross(Vector3.up, this.upAxis).normalized;

            this.rightAxis = Vector3.Cross(this.upAxis, this.forwardAxis).normalized;

            AxisChanged();
        } 
    }

    public Vector3 RightAxis
    {
        get => this.rightAxis;        
    }

    public Vector3 ForwardAxis
    {
        get => this.forwardAxis;        
    }

    public Vector3 OriginPosition
    {
        get
        {
            switch (this.originType)
            {
                case Origin.Bottom:
                    return this.bottom;
                case Origin.InnerBottom:
                    return this.innerBottom;
                case Origin.Center:
                    return this.center;
                case Origin.InnerTop:
                    return this.innerTop;
                case Origin.Top:
                    return this.top;
                default:
                    return this.center;
            }
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

    public CapsuleData() { }

    public CapsuleData(Vector3 center, Vector3 upAxis, float height, float radius)
    {
        SetCapsuleData(center, upAxis, height, radius);
    }   

    public CapsuleData(Vector3 bottom, float height, Vector3 upAxis, float radius)
    {
        SetCapsuleData(bottom, height, upAxis, radius);
    }

    public CapsuleData(Vector3 top, Vector3 bottom, float radius)
    {
        SetCapsuleData(top, bottom, radius);
    }

    public CapsuleData(CapsuleCollider col)
    {
        SetCapsuleData(col);
    }

    public void SetCapsuleData(CapsuleCollider col)
    {
        switch (col.direction)
        {
            case 0:
                this.UpAxis = col.transform.right;
                break;
            case 1:
                this.UpAxis = col.transform.up;
                break;
            case 2:
                this.UpAxis = col.transform.forward;
                break;
        }

        this.radius = col.radius;
        this.height = col.height;
        this.halfHeight = this.height * 0.5f;

        Vector3 offset = this.upAxis * this.halfHeight;
        Vector3 radiusOffset = this.upAxis * this.radius;

        this.center = col.transform.position + col.center;
        this.top = this.center + offset;
        this.bottom = this.center - offset;
        this.innerTop = this.top - radiusOffset;
        this.innerBottom = this.bottom + radiusOffset;

        ApplyOffsets();
    }

    public void SetCapsuleData(Vector3 center, Vector3 upAxis, float height, float radius)
    {
        this.UpAxis = upAxis;
        this.radius = radius;
        this.height = height;
        this.halfHeight = this.height * 0.5f;

        Vector3 offset = this.upAxis * this.halfHeight;        
        Vector3 radiusOffset = this.upAxis * this.radius;

        this.center = center;
        this.top = center + offset;
        this.bottom = center - offset;
        this.innerTop = this.top - radiusOffset;
        this.innerBottom = this.bottom + radiusOffset;

        ApplyOffsets();
    }

    public void SetCapsuleData(Vector3 bottom, float height, Vector3 upAxis, float radius)
    {
        this.UpAxis = upAxis;
        this.radius = radius;
        this.height = height;
        this.halfHeight = this.height * 0.5f;

        Vector3 offset = this.upAxis * this.halfHeight;        
        Vector3 radiusOffset = this.upAxis * this.radius;

        this.center = bottom + offset;
        this.top = this.center + offset;
        this.bottom = bottom;
        this.innerTop = this.top - radiusOffset;
        this.innerBottom = this.bottom + radiusOffset;

        ApplyOffsets();
    }

    public void SetCapsuleData(Vector3 top, Vector3 bottom, float radius)
    {
        Vector3 v = top - bottom;
        this.UpAxis = v.normalized;
        this.radius = radius;
        this.height = v.magnitude;
        this.halfHeight = this.height * 0.5f;

        Vector3 offset = this.upAxis * this.halfHeight;       
        Vector3 radiusOffset = this.upAxis * this.radius;

        this.center = bottom + offset;
        this.top = this.center + offset;
        this.bottom = bottom;
        this.innerTop = this.top - radiusOffset;
        this.innerBottom = this.bottom + radiusOffset;

        ApplyOffsets();
    }

    public void SetCapsuleData(float radius, Vector3 innerTop, Vector3 innerBottom)
    {
        Vector3 v = innerTop - innerBottom;
        this.UpAxis = v.normalized;
        this.radius = radius;
        this.height = v.magnitude + 2.0f * radius;
        this.halfHeight = this.height * 0.5f;
                
        Vector3 halfOffset = this.upAxis * (this.height - this.radius * 2.0f);
        Vector3 radiusOffset = this.upAxis * this.radius;

        this.top = innerTop + radiusOffset;
        this.bottom = innerBottom - radiusOffset;
        this.center = this.bottom + halfOffset;
        this.innerBottom = innerBottom;
        this.innerTop = innerTop;

        ApplyOffsets();
    }

    public void SetAxis(Vector3 right, Vector3 up, Vector3 forward)
    {
        this.rightAxis = right;
        this.upAxis = up;
        this.forwardAxis = forward;
        AxisChanged();        
    }

    void AxisChanged()
    {
        Vector3 heightOffset = this.upAxis * this.height;
        Vector3 halfOffset = this.upAxis * this.halfHeight;
        Vector3 radiusOffset = this.upAxis * this.radius;        

        switch (this.originType)
        {
            case Origin.Bottom:
                this.center = this.bottom + halfOffset;
                this.top = this.center + halfOffset;
                this.innerBottom = this.bottom + radiusOffset;
                this.innerTop = this.top - radiusOffset;
                break;

            case Origin.InnerBottom:
                this.bottom = this.innerBottom - radiusOffset;
                this.center = this.bottom + halfOffset;
                this.top = this.center + halfOffset;                
                this.innerTop = this.top - radiusOffset;
                break;

            case Origin.Center:
                this.bottom = this.center - halfOffset;
                this.top = this.center + halfOffset;
                this.innerBottom = this.bottom + radiusOffset;
                this.innerTop = this.top - radiusOffset;
                break;

            case Origin.InnerTop:
                this.top = this.innerTop + radiusOffset;
                this.bottom = this.top - halfOffset * 2.0f;
                this.innerBottom = this.bottom + radiusOffset;
                this.center = this.bottom + halfOffset;
                break;

            case Origin.Top:
                this.bottom = this.top - halfOffset * 2.0f;
                this.innerTop = this.top - radiusOffset;
                this.innerBottom = this.bottom + radiusOffset;
                this.center = this.bottom + halfOffset;
                break;
        }

        ApplyOffsets();
    }

    void ApplyOffsets()
    {
        if(this.bottomOffset != Vector3.zero)
        {
            this.bottom += this.bottomOffset;
            this.innerBottom += this.bottomOffset;
        }        

        if(this.topOffset != Vector3.zero)
        {
            this.top += this.topOffset;
            this.innerTop += this.topOffset;
        }
    }

    public bool Overlaps(LayerMask layerMask, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
    {
        this.overlapping = Physics.CheckCapsule(this.innerBottom, this.innerTop, this.radius, layerMask, triggerInteraction); ;
        return this.overlapping;
    }

    public bool Overlaps()
    {
        this.overlapping = Physics.CheckCapsule(this.innerBottom, this.innerTop, this.radius);
        return this.overlapping;
    }
}