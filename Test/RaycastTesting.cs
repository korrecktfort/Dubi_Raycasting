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

        private void Awake()
        {
            this.rayCast.Setup();
        }

        public void Update()
        {
            if (this.rayCast.UpdateRaycast())
            {
                Collider collider = this.rayCast.Hit.collider;
            }
        }
    }

   
}
