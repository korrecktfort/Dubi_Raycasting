using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dubi.BaseValues;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dubi.RaycastExtension
{
    [System.Serializable]
    public class Raycast
    {
        #region Propreties
        public bool Valid => (!this.raycastAll.Value && this.rayCastHit.collider != null) 
            || (this.raycastAll.Value && this.rayCastHits.Length > 0);

        public RaycastHit Hit 
        {
            get 
            {
                if (this.raycastAll.Value && this.rayCastHits.Length > 0)
                    return this.rayCastHits[0];

                return this.rayCastHit; 
            } 
        }

        public RaycastHit[] Hits => this.rayCastHits;    
        
        public Vector3 Origin { set => this.origin = value; }

        public Vector3 CustomCheckDirection { set => this.customCheckDir.Value = value; }

        public Vector3 Direction { get => this.localDirection.Value; set => this.localDirection.Value = value; }

        public float Distance { get => this.distance.Value; set => this.distance.Value = value; }

        public float Radius { get => this.radius.Value; set => this.radius.Value = value; }

        public Vector3 Offset { get => localOffset.Value; set => localOffset.Value = value; }

        public bool RaycastAll { get => this.raycastAll.Value; }
        #endregion

        #region Fields
        Vector3 origin = Vector3.zero;
        RaycastHit rayCastHit = new RaycastHit();
        RaycastHit[] rayCastHits = new RaycastHit[0];

        [SerializeField] TransformValue originTransform = null;
        [SerializeField] Vector3Value localOffset = null;
        [SerializeField] Vector3Value localDirection = null;

        [SerializeField] FloatValue distance = new FloatValue(1.0f);
        [SerializeField] FloatValue radius = new FloatValue(0.0f);
        [SerializeField] BoolValue raycastAll = new BoolValue(false);       
        [SerializeField] LayerMaskValue layerMask = default;
        [SerializeField] QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;

        [SerializeField] BoolValue surfaceNormal = new BoolValue(false);
        [SerializeField] BoolValue useRayDir = new BoolValue(true);
        [SerializeField] Vector3Value customCheckDir = null;
        [SerializeField] BoolValue isLocalCheckDir = new BoolValue(false);
        [SerializeField] BoolValue useCustomSurfaceCheck = new BoolValue(false);

        /// Sorting hit points
        [SerializeField] BoolValue sortAlongRay = new BoolValue(false);
        [SerializeField] BoolValue ascendingOrder = new BoolValue(false);

        /// Invalid Layer
        [SerializeField, Tooltip("Select layers that make the cast invalid if hit")] BoolValue useInvalidLayer = new BoolValue(false);
        [SerializeField] LayerMaskValue invalidLayer;
        bool setup = false;
        #endregion

        #region Gizmos
        public Vector3 worldOrigin, worldDirection;
        [SerializeField] BoolValue drawGizmos = new BoolValue(false);
        [SerializeField] BoolValue drawColliderHit = new BoolValue(false);
        [SerializeField] ColorValue color = new ColorValue(Color.white);
        public bool DrawGizmos => this.drawGizmos.Value;
        public bool DrawColliderHit => this.drawColliderHit.Value;
        public Color GizmosColor => this.color.Value;
        #endregion

        public Raycast() { }
                
        public void Setup(Transform localTransform, Vector3 origin, Vector3 direction)
        {           
            this.originTransform.Value = localTransform;
            this.origin = origin;
            this.localDirection.Value = direction;
            this.setup = true;
        }
        
        public void Setup()
        {
            if(this.originTransform.Value != null)
            {
                Setup(this.originTransform.Value, Vector3.zero, this.localDirection.Value);
                return;
            }
        }

        public bool UpdateRaycast()
        {
            if (!this.setup)
                Setup();

            Ray ray = new Ray();

            if(this.originTransform.Value != null)
            {
                Matrix4x4 m = this.originTransform.Value.localToWorldMatrix;                
                ray = new Ray(m.MultiplyPoint(this.localOffset.Value), m.MultiplyVector(this.localDirection.Value));
            }
            else
            {
                ray = new Ray(this.origin + this.localOffset.Value, this.localDirection.Value);
            }            

            this.worldOrigin = ray.origin;
            this.worldDirection = ray.direction;

            if(this.radius.Value > 0.0f)
            {
                if (this.raycastAll.Value)
                {
                    RaycastHit[] hits = Physics.SphereCastAll(ray, this.radius.Value, this.distance.Value, this.layerMask.Value.value, this.triggerInteraction);
                    if(hits.Length > 0)
                    {
                        hits = RemoveInvalid(hits);

                        if (this.surfaceNormal.Value)
                        {
                            if(this.useCustomSurfaceCheck.Value)
                                for (int i = 0; i < hits.Length; i++)
                                    hits[i] = SurfaceNormalManualRaycast(hits[i]);
                            else
                                for (int i = 0; i < hits.Length; i++)
                                hits[i] = SurfaceNormal(hits[i]);
                        }

                        if(this.sortAlongRay.Value)
                            SortHitsAlongRay(ray, hits);

                        this.rayCastHits = hits;
                        return hits.Length > 0 && hits[0].collider != null;
                    }
                }
                else
                {
                    if (Physics.SphereCast(ray, this.radius.Value, out RaycastHit hit, this.distance.Value, this.layerMask.Value.value, this.triggerInteraction))
                    {
                        hit = Validate(hit);

                        if (this.surfaceNormal.Value)
                            hit = this.useCustomSurfaceCheck.Value ? SurfaceNormalManualRaycast(hit) : SurfaceNormal(hit);

                        this.rayCastHit = hit;
                        return hit.collider != null;
                    }
                }
            }
            else
            {
                if (this.raycastAll.Value)
                {
                    RaycastHit[] hits = Physics.RaycastAll(ray, this.distance.Value, this.layerMask.Value.value, this.triggerInteraction);
                    if(hits.Length > 0)
                    {
                        hits = RemoveInvalid(hits);

                        if(this.sortAlongRay.Value)
                            SortHitsAlongRay(ray, hits);
                        
                        this.rayCastHits = hits;
                        return hits.Length > 0 && hits[0].collider != null;
                    }
                }
                else
                {
                    if (Physics.Raycast(ray, out RaycastHit hit, this.distance.Value, this.layerMask.Value.value, this.triggerInteraction))
                    {
                        hit = Validate(hit);

                        this.rayCastHit = hit;
                        return hit.collider != null;
                    }
                }
            }

            this.rayCastHits = new RaycastHit[0];
            this.rayCastHit = new RaycastHit();
            return false;
        }

        RaycastHit Validate(RaycastHit hit)
        {
            if (hit.collider != null && this.useInvalidLayer.Value && this.invalidLayer.Contains(hit.collider.gameObject.layer))
                hit = new RaycastHit();

            return hit;
        }

        RaycastHit SurfaceNormal(RaycastHit hit)
        {
            Collider collider = hit.collider;
            if(collider != null)
            {        
                float distance = 0.02f;
                Vector3 normal = hit.normal.normalized;
                Vector3 point = collider.ClosestPoint(hit.point);
                Vector3 dir = this.useRayDir.Value ? normal : -this.customCheckDir.Value;                
                Vector3 origin = point + distance * 0.5f * dir;

                if (this.isLocalCheckDir.Value)
                    dir = this.originTransform.Value.localToWorldMatrix.MultiplyVector(dir);

                Ray ray = new Ray(origin, -dir);                
                
                if(collider.Raycast(ray, out RaycastHit colHit, distance))
                    hit.normal = colHit.normal;                
            }

            return hit;
        }

        RaycastHit SurfaceNormalManualRaycast(RaycastHit hit)
        {
            Collider collider = hit.collider;
            if(collider != null)
            {
                float radius = 0.01f;
                float halfDistance = radius + 0.005f;
                Vector3 normal = hit.normal.normalized;
                Vector3 point = hit.point;
                Vector3 dir = this.customCheckDir.Value;

                Ray ray = new Ray(point - dir * halfDistance, dir);
                if (Physics.SphereCast(ray, radius, out RaycastHit surfaceHit, halfDistance * 2.0f, this.layerMask.Value, this.triggerInteraction))
                    hit.normal = surfaceHit.normal;
            }

            return hit;
        }

        struct RaycastSortItem
        {
            public RaycastSortItem(RaycastHit hit, float distance)
            {
                this.hit = hit;
                this.distance = distance;
            }

            public RaycastHit hit;
            public float distance;
        }

        RaycastHit[] SortHitsAlongRay(Ray ray, RaycastHit[] hits)
        {
            int length = hits.Length;
            if (length > 0)
            {
                /// w-value of vector 4 is the distance of the projected vector
                Vector3 direction = ray.direction;
                Vector3 origin = ray.origin;
                RaycastSortItem[] sort = new RaycastSortItem[length];
                for (int i = 0; i < length; i++)
                    sort[i] = new RaycastSortItem(hits[i], Vector3.Dot(hits[i].point - origin, direction));                

                System.Array.Sort(sort, this.ascendingOrder.Value ? CompareByWAsc : CompareByWDesc);

                for (int i = 0; i < length; i++)
                    hits[i] = sort[i].hit;
            }

            return hits;
        }

        int CompareByWDesc(RaycastSortItem r0, RaycastSortItem r1)
        {
            if (r0.distance > r1.distance)
                return -1;

            if (r0.distance < r1.distance)
                return 1;

            return 0;
        }

        int CompareByWAsc(RaycastSortItem r0, RaycastSortItem r1)
        {
            if (r0.distance > r1.distance)
                return 1;

            if (r0.distance < r1.distance)
                return -1;

            return 0;
        }

        RaycastHit[] RemoveInvalid(RaycastHit[] hits)
        {
            List<RaycastHit> list = new List<RaycastHit>();
            foreach (RaycastHit hit in hits)
            {
                if(hit.collider != null)
                {
                    if (this.useInvalidLayer.Value && !this.invalidLayer.Contains(hit.collider.gameObject.layer))
                    {                   
                        list.Add(hit);
                        continue;
                    }

                    list.Add(hit);
                }
            }
            return list.ToArray();
        }

        public Vector3 NormalFromAllHits(Collider collider)
        {
            foreach (RaycastHit hit in this.rayCastHits)
                if (hit.collider == collider)
                    return hit.normal;

            return Vector3.zero;
        }
    }   
}