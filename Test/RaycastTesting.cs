using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Dubi.RaycastExtension
{
    public class RaycastTesting : MonoBehaviour
    {
        [SerializeField] Raycast rayCast = new Raycast();
        [SerializeField] LayerMask invalidLayer;


        private void Awake()
        {
            this.rayCast.Setup(this.transform, Vector3.zero, Vector3.forward);
        }

        public void Update()
        {
            if (this.rayCast.Cast())
            {
                Collider collider = this.rayCast.RaycastHit.collider;
            }
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            this.rayCast.OnDrawGizmos();
        }
#endif
    }

   
}
