using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SaveSystem.Core.Scripts.UnityComponentsData
{
    [Serializable]
    public struct MeshData
    {
        public Matrix4x4[] bindposes;
        public Bounds bounds;
        public Color[] colors;
        public Color32[] colors32;
        public Vector3[] normals;
        public Vector4[] tangents;
        public int[] triangles;
        public Vector2[] uv;
        public Vector2[] uv2;
        public Vector2[] uv3;
        public Vector2[] uv4;
        public Vector2[] uv5;
        public Vector2[] uv6;
        public Vector2[] uv7;
        public Vector2[] uv8;
        public Vector3[] vertices;
        public BoneWeight[] boneWeights;
        public IndexFormat indexFormat;
        public GraphicsBuffer.Target indexBufferTarget;
        public int subMeshCount;
        public GraphicsBuffer.Target vertexBufferTarget;
        public string name;
    }
}