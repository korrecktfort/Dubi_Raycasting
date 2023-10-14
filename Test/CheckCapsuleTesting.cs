using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dubi.RaycastExtension
{    
    public class CheckCapsuleTesting : MonoBehaviour
    {
        [SerializeField] CapsuleData caps = null;

        //[SerializeField] CapsuleData checkCapsule01 = new CapsuleData(Vector3.up, Vector3.zero, 0.2f);
        //[SerializeField] CapsuleData checkCapsule02 = new CapsuleData(Vector3.zero, 1.0f, Vector3.up, 0.2f);
        //[SerializeField] CapsuleData checkCapsule03 = new CapsuleData(new Vector3(0.0f, 0.5f, 0.0f), Vector3.up, 1.0f, 0.2f);

        //[SerializeField] float topOffset;
        //[SerializeField] float bottomOffset;

//#if UNITY_EDITOR
//        private void OnDrawGizmos()
//        {
//            if (this.transform.hasChanged)
//            {
//                this.checkCapsule01.SetAxis(this.transform.right, this.transform.up, this.transform.forward);                
//                this.transform.hasChanged = false;
//            }

//            this.checkCapsule01.TopOffset = this.topOffset;
//            this.checkCapsule01.BottomOffset = this.bottomOffset;

//            //using (new Handles.DrawingScope(this.transform.localToWorldMatrix))
//                RaycastGizmos.DrawWireCapsule(this.checkCapsule01, Color.red);

//            RaycastGizmos.DrawNormal(Vector3.zero, this.checkCapsule01.RightAxis, Color.red, 1.0f, 2.0f);
//            RaycastGizmos.DrawNormal(Vector3.zero, this.checkCapsule01.UpAxis, Color.green, 1.0f, 2.0f);
//            RaycastGizmos.DrawNormal(Vector3.zero, this.checkCapsule01.ForwardAxis, Color.blue, 1.0f, 2.0f);
//        }
//#endif
    }
}
