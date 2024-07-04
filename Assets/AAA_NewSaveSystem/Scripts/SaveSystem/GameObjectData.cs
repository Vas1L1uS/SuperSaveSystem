using System;
using System.Collections.Generic;

namespace AAA_NewSaveSystem.Scripts.SaveSystem
{
    [Serializable]
    public class GameObjectData
    {
        public string name;
        public int id;
        public int instanceId;
        public List<GameObjectData> children = new();
        public List<ComponentData> components = new List<ComponentData>();
    }
}