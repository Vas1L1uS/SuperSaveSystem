using System;

namespace Vas1L1uS.QuickSaveGame.Core.Scripts.UnityComponentsData
{
    [Serializable]
    public struct ColliderData
    {
        public bool enabled;
        public float contactOffset;
        public int excludeLayers;
        public int includeLayers;
        public bool isTrigger;
        public bool providesContacts;
        public bool hasModifiableContacts;
        public int layerOverridePriority;
    }
}