using System;
using AAAProject.Scripts.Entities;
using AAAProject.Scripts.Extensions;
using UnityEngine;

namespace AAAProject.Scripts.Components
{
    public class TurretController : MBehaviour
    {
        public event Action TargetChanged;
        
        public float CurrentReloadTime => _currentReloadTime;
        public Transform CurrentTarget => _currentTarget;

        [SerializeField]private Bullet    _bulletPrefab;
        [SerializeField]private LayerMask _triggerLayer;
        [SerializeField]private float     _attackRadius = 15f;
        [SerializeField]private float     _bulletSpeed;
        [SerializeField]private float     _reloadTime = 0.5f;

        [SerializeField]private Transform _currentTarget;
        [SerializeField]private float     _currentReloadTime;

        public void Init(Transform target, float currentReloadTime)
        {
            _currentTarget = target;
            _currentReloadTime = currentReloadTime;
        }

        private void Update()
        {
            if (_currentReloadTime < _reloadTime)
            {
                _currentReloadTime += Time.deltaTime;
            }
            else
            {
                if (_currentTarget != null)
                {
                    Shoot(GetMoveVector());
                    _currentReloadTime = 0;
                }
                else
                {
                    FindNearestTarget();
                }
            }
        }

        private void FindNearestTarget()
        {
            Collider[] triggers = Physics.OverlapSphere(this.transform.position, _attackRadius, _triggerLayer);

            if (triggers.Length > 0)
            {
                Transform nearestTarget = triggers[0].gameObject.transform;
                float minDistance = Vector3.Distance(transform.position, nearestTarget.position);

                for (int i = 0; i < triggers.Length; i++)
                {
                    float distance = Vector3.Distance(transform.position, triggers[i].gameObject.transform.position);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestTarget = triggers[i].gameObject.transform;
                    }
                }

                _currentTarget = nearestTarget;
                TargetChanged?.Invoke();
            }
        }

        private void Shoot(Vector3 direction)
        {
            Instantiate(_bulletPrefab, transform.position + Vector3.up, Quaternion.identity, Root.GetTransform()).SetMoveDirection(direction);
        }

        private Vector3 GetMoveVector()
        {
            Vector3 direction = _currentTarget.position - transform.position;
            Vector3 directionNormalized = direction.normalized;
            return directionNormalized * _bulletSpeed;
        }
    }
}
