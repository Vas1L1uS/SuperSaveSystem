using System;
using UnityEngine;

namespace SaveSystem.Core.Scripts.UnityComponentsData
{
    [Serializable]
    public struct TransformData
    {
        public Vector3 LocalPosition;
        public Quaternion LocalRotation;
        public Vector3 LocalScale;
        public int ParentInstanceId;
    }
}