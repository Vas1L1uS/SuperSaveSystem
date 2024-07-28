using UnityEngine;

namespace SaveSystem.Core.Scripts
{
    public static class UnityComponentsOverwriter
    {
        public static void CopyValues(this MeshRenderer a, MeshRenderer b)
        {
            a.materials = b.sharedMaterials;
            a.additionalVertexStreams = b.additionalVertexStreams;
            a.enlightenVertexStream = b.enlightenVertexStream;
            //a.receiveGI = b.receiveGI;
            //a.scaleInLightmap = b.scaleInLightmap;
            //a.stitchLightmapSeams = b.stitchLightmapSeams;
            //a.bounds = b.bounds;
            a.lightmapIndex = b.lightmapIndex;
            //a.localBounds = b.localBounds;
            a.probeAnchor = b.probeAnchor;
            a.receiveShadows = b.receiveShadows;
            a.rendererPriority = b.rendererPriority;
            a.sortingOrder = b.sortingOrder;
            a.forceRenderingOff = b.forceRenderingOff;
            a.lightmapScaleOffset = b.lightmapScaleOffset;
            a.lightProbeUsage = b.lightProbeUsage;
            a.rayTracingMode = b.rayTracingMode;
            a.realtimeLightmapIndex = b.realtimeLightmapIndex;
            a.reflectionProbeUsage = b.reflectionProbeUsage;
            a.renderingLayerMask = b.renderingLayerMask;
            a.shadowCastingMode = b.shadowCastingMode;
            a.sortingLayerName = b.sortingLayerName;
            a.staticShadowCaster = b.staticShadowCaster;
            a.allowOcclusionWhenDynamic = b.allowOcclusionWhenDynamic;
            a.motionVectorGenerationMode = b.motionVectorGenerationMode;
            //a.realtimeLightmapScaleOffset = b.realtimeLightmapScaleOffset;
            a.sortingLayerID = b.sortingLayerID;
            a.lightProbeProxyVolumeOverride = b.lightProbeProxyVolumeOverride;
        }

        public static void CopyValues(this MeshFilter a, MeshFilter b)
        {
            a.mesh = b.sharedMesh;
        }

        public static void CopyValues(this Rigidbody a, Rigidbody b)
        {
            a.constraints = b.constraints;
            a.interpolation = b.interpolation;
            a.excludeLayers = b.excludeLayers;
            a.includeLayers = b.includeLayers;
            a.collisionDetectionMode = b.collisionDetectionMode;
        }
        
        public static void CopyValues(this Collider a, Collider b)
        {
            if (b.sharedMaterial != null)
            {
                a.sharedMaterial = b.sharedMaterial;
            }
            
            a.enabled = b.enabled;
            a.contactOffset = b.contactOffset;
            a.excludeLayers = b.excludeLayers;
            a.includeLayers = b.includeLayers;
            a.isTrigger = b.isTrigger;
            a.providesContacts = b.providesContacts;
            a.hasModifiableContacts = b.hasModifiableContacts;
            a.layerOverridePriority = b.layerOverridePriority;
        }
    }
}