using AAAProject.Scripts.Components;
using UnityEngine;

namespace AAAProject.Scripts.Entities
{
    [SelectionBase]
    public class Enemy : Entity
    {
        public class EnemyCtx : EntityCtx
        {
            public string  TargetKey;
            public Vector3 Velocity;
            public Vector3 AngularVelocity;
        }
        
        [SerializeField]private MovementController _movementController;
        [SerializeField]private Entity          _target;

        [ContextMenu("InitComponents")]
        public void InitComponents()
        {
            _movementController.Init(_target?.transform, ((EnemyCtx)_ctx).Velocity, ((EnemyCtx)_ctx).AngularVelocity);
        }

        protected override void SaveAdditionalParams()
        {
            ((EnemyCtx)_ctx).TargetKey = _target?.Key;
            ((EnemyCtx)_ctx).Velocity = _movementController.Rigidbody.velocity;
            ((EnemyCtx)_ctx).AngularVelocity = _movementController.Rigidbody.angularVelocity;
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
