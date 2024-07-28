using System;
using UnityEngine;

namespace Vas1L1uS.QuickSaveGame.Core.Scripts.UnityComponentsData
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