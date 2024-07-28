using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SaveSystem.Core.Scripts.UnityComponentsData
{
    [Serializable]
    public struct MaterialData
    {
        public string shaderName;
        public Color color;
        public Color[] colors;
        public LocalKeyword[] enabledKeywords;
        public bool enableInstancing;
        public Texture mainTexture;
        public int renderQueue;
        public string[] shaderKeywords;
        public MaterialGlobalIlluminationFlags globalIlluminationFlags;
        public Vector2 mainTextureOffset;
        public Vector2 mainTextureScale;
        public bool doubleSidedGI;
    }
}