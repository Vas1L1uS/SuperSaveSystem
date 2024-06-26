using System.Collections.Generic;
using AAAProject.Scripts.Components;
using UnityEngine;

namespace AAAProject.Scripts.Entities
{
    [SelectionBase]
    public class EnemiesManager : Entity
    {
        public class EnemiesManagerCtx : EntityCtx
        {
            public List<string> EnemiesKeys;
        }

        [SerializeField]private Spawner _spawner;

        [SerializeField]private List<Enemy> _enemies;

        [ContextMenu("InitComponents")]
        public void InitComponents()
        {
            _spawner.NewObject += obj => 
            {
                AddEnemy(obj.GetComponent<Enemy>());
            };
        }

        protected override void SaveAdditionalParams()
        {
            SetEnemiesKeys();
        }

        protected override void NewEntityCtx()
        {
            _ctx = new EnemiesManagerCtx();
        }

        private void Start()
        {
            SetEnemies();
            InitComponents();
            CheckEnemyCount();
        }

        private void SetEnemies()
        {
            _enemies = new();
            
            if (((EnemiesManagerCtx)_ctx).EnemiesKeys == null) return;
            
            for (int i = 0; i < ((EnemiesManagerCtx)_ctx).EnemiesKeys.Count; i++)
            {
                AddEnemy(Root.GetEntityByKey(((EnemiesManagerCtx)_ctx).EnemiesKeys[i]) as Enemy);
            }

            foreach (var enemy in _enemies)
            {
                enemy.Died += () => RemoveEnemy(enemy);
            }
        }

        private void SetEnemiesKeys()
        {
            ((EnemiesManagerCtx)_ctx).EnemiesKeys = new();

            for (int i = 0; i < _enemies.Count; i++)
            {
                ((EnemiesManagerCtx)_ctx).EnemiesKeys.Add(_enemies[i].Key);
            }
        }

        private void AddEnemy(Enemy entity)
        {
            _enemies.Add(entity);
            entity.Died += () => RemoveEnemy(entity);
        }
        
        private void RemoveEnemy(Enemy entity)
        {
            _enemies.Remove(entity);
            CheckEnemyCount();
        }

        private void CheckEnemyCount()
        {
            while (_enemies.Count < 10)
            {
                _spawner.Spawn();
            }
        }
    }
}
