using System.Collections.Generic;
using UnityEngine;

namespace AAAProject.Scripts
{
    public class Root : Entity
    {
        [SerializeField]private GameObject       _baseLevelPrefab;
        [SerializeField]private EntityCollection _entityCollectionScrObj;

        private static readonly Dictionary<string, Entity> _entities = new();

        public static Entity GetEntityByKey(string key)
        {
            if (key == null) return null;
            
            if (_entities.ContainsKey(key))
            {
                return _entities[key];
            }

            return null;
        }
        
        public static void AddEntity(Entity entity)
        {
            _entities.Add(entity.Key, entity);
        }

        private static void ClearPlayerPrefs()
        {
            foreach (var entity in _entities)
            {
                for (int i = 0; i < _entities.Count; i++)
                {
                    PlayerPrefs.DeleteKey(entity.Key);
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            Key = "AAA*Root";
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                Init(Key, _entityCollectionScrObj);

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
