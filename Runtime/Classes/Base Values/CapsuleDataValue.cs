using Dubi.BaseValues;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dubi.RaycastExtension
{
    [Serializable]
    public class CapsuleDataValue : GenericBaseValue<CapsuleData, CapsuleDataObject, BaseValueUpdater>
    {
        public CapsuleDataValue(CapsuleData value) : base(value)
        {
        }
    }
}