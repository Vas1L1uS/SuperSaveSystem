using AAAProject.Scripts.Components;
using UnityEngine;

namespace AAAProject.Scripts.Entities
{
    [SelectionBase]
    public class Bullet : Entity
    {
        public class BulletCtx : EntityCtx
        {
            public Vector3 MoveDirection;
        }
        
        [SerializeField]private DamageSystem     _damageSystem;
        [SerializeField]private CollisionChecker _collisionChecker;
        [SerializeField]private Movement         _movement;

        [ContextMenu("InitComponents")]
        public void InitComponents()
        {
            _movement.SetDirection(((BulletCtx)_ctx).MoveDirection);
        }

        public void SetMoveDirection(Vector3 moveDirection)
        {
            ((BulletCtx)_ctx).MoveDirection = moveDirection;
        }

        protected override void Awake()
        {
            base.Awake();
            _collisionChecker.CollisionEntered += CollisionEntered;
        }

        protected override void SaveAdditionalParams()
        {
            ((BulletCtx)_ctx).MoveDirection = _movement.Direction; 
        }

        protected override void NewEntityCtx()
        {
            _ctx = new BulletCtx();
        }

        private void Start()
        {
            InitComponents();
        }
        
        private void CollisionEntered(GameObject obj)
        {
            if (obj.TryGetComponent(out HealthSystem health))
            {
                _damageSystem.MakeDamage(health);
            }
            
            Destroy(this.gameObject);
        }
    }
}
