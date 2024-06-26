using System;
using AAAProject.Scripts.Components;
using UnityEngine;

namespace AAAProject.Scripts.Entities
{
    [SelectionBase]
    public class Enemy : Entity
    {
        public class EnemyCtx : EntityCtx
        {
            public string TargetKey;
            public float  CurrentHealth;
            public float  CurrentReloadTime;
        }

        public event Action Died;

        [SerializeField]private MeleeController             _meleeController;
        [SerializeField]private HealthSystem                _healthSystem;
        [SerializeField]private CharacterMovementController _movementController;
        [SerializeField]private VisorController             _visorController;

        private Entity _target;

        [ContextMenu("InitComponents")]
        public void InitComponents()
        {
            _visorController.Init(_target?.transform);
            _meleeController.Init(((EnemyCtx)_ctx).CurrentReloadTime);
            _visorController.TargetChanged += () => 
            {
                _target = _visorController.CurrentTarget.GetComponent<Entity>();
                _movementController.SetTarget(_target.transform);
            };
            _movementController.SetTarget(_target?.transform);
            _healthSystem.Died += () => {
                Died?.Invoke();
                Destroy(this.gameObject);
            };
        }
        
        protected override void NoHasKeyOnLoad()
        {
            base.NoHasKeyOnLoad();
            ((EnemyCtx)_ctx).CurrentHealth = _healthSystem.CurrentHealth;
            ((EnemyCtx)_ctx).CurrentReloadTime = _meleeController.CurrentReloadTime;
        }

        protected override void SaveAdditionalParams()
        {
            ((EnemyCtx)_ctx).TargetKey = _target?.Key; 
            ((EnemyCtx)_ctx).CurrentHealth = _healthSystem.CurrentHealth;
            ((EnemyCtx)_ctx).CurrentReloadTime = _meleeController.CurrentReloadTime;
        }
        
        protected override void SetAdditionalParams()
        {
            base.SetAdditionalParams();
            _healthSystem.SetHealth(((EnemyCtx)_ctx).CurrentHealth);
        }

        protected override void NewEntityCtx()
        {
            _ctx = new EnemyCtx();
        }

        private void Start()
        {
            Entity targetEntity = Root.GetEntityByKey(((EnemyCtx)_ctx).TargetKey);

            if (targetEntity != null)
            {
                _target = targetEntity;
            }
            
            InitComponents();
        }
    }
}
