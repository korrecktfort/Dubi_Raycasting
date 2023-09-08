using Dubi.RaycastExtension;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dubi.BaseValues;
using UnityEditor;

[CustomPropertyDrawer(typeof(CapsuleDataValue))]
public class CapsuleDataValueDrawer : BaseValueDrawer<CapsuleDataObject>
{
    public override float ValueFieldWidth()
    {
        return 0.0f;
    }
}
