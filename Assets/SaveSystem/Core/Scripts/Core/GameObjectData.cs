using System;
using System.Collections.Generic;

namespace SaveSystem.Core.Scripts.Core
{
    [Serializable]
    public class GameObjectData
    {
        public string name;
        public int instanceId;
        public bool activeSelf;
        public int layer;
        public string tag;
        public List<ComponentData> components = new();
        public List<GameObjectData> children = new();
    }

    [Serializable]
    public class RootSaverData
    {
        public int totalSaveObjects;
        public List<int> assets = new();
        public List<GameObjectData> children = new();
        public List<GameObjectData> otherSavedGameObjects = new();
    }
    
    [Serializable]
    public class ComponentData
    {
        public string typeName;
        public int instanceId;
        public string jsonData;
    }
}