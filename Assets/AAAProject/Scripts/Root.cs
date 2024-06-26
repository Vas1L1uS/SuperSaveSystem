using System.Collections.Generic;
using UnityEngine;

namespace AAAProject.Scripts
{
    public class Root : Entity
    {
        [SerializeField]private GameObject       _baseLevelPrefab;
        [SerializeField]private EntityCollection _entityCollection;

        private static readonly Dictionary<string, Entity> _entitiesInWorld = new();
        private static          Dictionary<string, Entity> _entitiesPrefabs;
        private static          Transform                  _myTransform;

        public static Entity GetEntityByKey(string key)
        {
            if (key == null) return null;
            
            if (_entitiesInWorld.ContainsKey(key))
            {
                return _entitiesInWorld[key];
            }

            return null;
        }

        public static Entity GetEntityPrefabByPrefabPath(string path)
        {
            if (path == null) return null;
            
            return _entitiesPrefabs[path];
        }
        
        public static void AddEntity(Entity entity)
        {
            _entitiesInWorld.Add(entity.Key, entity);
            entity.BeingDestroyed += () => _entitiesInWorld.Remove(entity.Key);
        }

        public static Transform GetTransform()
        {
            return _myTransform;
        }

        private static void ClearPlayerPrefs()
        {
            foreach (var entity in _entitiesInWorld)
            {
                for (int i = 0; i < _entitiesInWorld.Count; i++)
                {
                    PlayerPrefs.DeleteKey(entity.Key);
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            Key = "AAA*Root";
            _entitiesPrefabs = _entityCollection.GetEntityDictionary();
            _myTransform = transform;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                Init(Key);

                if (_ctx.ChildKeys.Count <= 0)
                {
                    Instantiate(_baseLevelPrefab, transform);
                }
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                ClearPlayerPrefs();
                Save();
            }
        }
    }
}
