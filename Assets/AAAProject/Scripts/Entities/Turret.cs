using AAAProject.Scripts.Components;
using UnityEngine;

namespace AAAProject.Scripts.Entities
{
    [SelectionBase]
    public class Turret : Entity
    {
        public class TurretCtx : EntityCtx
        {
            public string TargetKey;
            public float  CurrentHealth;
            public float  CurrentReloadTime;
        }

        [SerializeField]private HealthSystem     _healthSystem;
        [SerializeField]private TurretController _turretController;

        private Entity _target;

        [ContextMenu("InitComponents")]
        public void InitComponents()
        {
            _turretController.TargetChanged += () => 
            {
                _target = _turretController.CurrentTarget.GetComponent<Entity>();
            };
            
            _turretController.Init(_target?.transform, ((TurretCtx)_ctx).CurrentReloadTime);
            _healthSystem.Died += () => Destroy(this.gameObject);
        }

        protected override void SaveAdditionalParams()
        {
            ((TurretCtx)_ctx).TargetKey = _target?.Key;
            ((TurretCtx)_ctx).CurrentHealth = _healthSystem.CurrentHealth;
            ((TurretCtx)_ctx).CurrentReloadTime = _turretController.CurrentReloadTime;
        }

        protected override void SetAdditionalParams()
        {
            base.SetAdditionalParams();
            _healthSystem.SetHealth(((TurretCtx)_ctx).CurrentHealth);
        }

        protected override void NoHasKeyOnLoad()
        {
            base.NoHasKeyOnLoad();
            ((TurretCtx)_ctx).CurrentHealth = _healthSystem.CurrentHealth;
            ((TurretCtx)_ctx).CurrentReloadTime = _turretController.CurrentReloadTime;
        }

        protected override void NewEntityCtx()
        {
            _ctx = new TurretCtx();
        }

        private void Start()
        {
            Entity targetEntity = Root.GetEntityByKey(((TurretCtx)_ctx).TargetKey);

            if (targetEntity != null)
            {
                _target = targetEntity;
            }
            
            InitComponents();
        }
    }
}
