using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace AAAProject.Scripts
{
    [CreateAssetMenu(fileName = "EntityCollection", menuName = "EntityCollection", order = 0)]
    public class EntityCollection : ScriptableObject
    {
        [SerializeField]private List<Entity> _entities;
        [SerializeField]private List<string> _entityPaths;

        public Entity GetEntityByPath(string path)
        {
            int index = 0;
            
            for (int i = 0; i < _entities.Count; i++)
            {
                if (_entityPaths[i] == path)
                {
                    index = i;
                    break;
                }
            }
            
            return _entities[index];
        }

        [ExecuteInEditMode]
        [ContextMenu("InitTypeNames")]
        public void InitTypeNames()
        {
            _entityPaths.Clear();
            
            for (int i = 0; i < _entities.Count; i++)
            {
                string path = _entities[i].PrefabPath;
                _entityPaths.Add(path);
            }
        }
    }
}
