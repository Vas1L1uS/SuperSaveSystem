using AAAProject.Scripts.Extensions;
using UnityEngine;

namespace AAAProject.Scripts
{
    public class FollowController : MBehaviour
    {
        public Transform Target { get => _target; set => SetTarget(value); }
        
        [Header("Movement")]
        [SerializeField] private Transform _target;
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _smoothTime = 0.3f;
        [Header("Rotation")]
        [SerializeField] private bool _rotate = true;
        [SerializeField] private bool _followLook = true;
        [SerializeField] private float _rotationSpeed = 5f;

        private Vector3 _moveVelocity = Vector3.zero;
        private Vector3 _startDirection = Vector3.zero;
        private float _startDistance;
        private Quaternion _startTargetRotation;

        protected override void Awake()
        {
            SetTarget(_target);
            base.Awake();
        }
        
        private void SetTarget(Transform target)
        {
            if (target == null) return;
            
            _target = target;
            _startDirection = transform.position - _target.position;
            _startTargetRotation = _target.rotation;
        }

        private void FixedUpdate()
        {
            if(_target != null)
            {
                var deltaRotation = Quaternion.Inverse(_startTargetRotation) * _target.rotation;
                Vector3 pos = Vector3.zero;
                
                if (_rotate)
                {
                    Vector3 offset = deltaRotation * _startDirection;
                    pos = _target.position + offset;
                }
                else
                {
                    pos = _target.position + _startDirection;
                }

                transform.position = Vector3.SmoothDamp(transform.position, pos, ref _moveVelocity, _smoothTime, _moveSpeed);

                if (_followLook)
                {
                    Vector3 direction = (_target.position - transform.position).normalized;
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, _rotationSpeed * Time.fixedDeltaTime);
                }
            }
        }
    }
}