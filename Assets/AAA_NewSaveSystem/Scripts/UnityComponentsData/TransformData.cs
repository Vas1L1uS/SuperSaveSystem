using System;
using UnityEngine;

namespace AAA_NewSaveSystem.Scripts.UnityComponentsData
{
    [Serializable]
    public struct TransformData
    {
        public Vector3 LocalPosition;
        public Quaternion LocalRotation;
        public Vector3 LocalScale;
    }
}