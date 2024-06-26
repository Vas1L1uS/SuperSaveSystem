using System;
using AAAProject.Scripts.Entities;
using AAAProject.Scripts.Extensions;
using UnityEngine;

namespace AAAProject.Scripts.Components
{
    public class VisorController : MBehaviour
    {
        public event Action TargetChanged;
        
        public Transform CurrentTarget => _currentTarget;
        
        [SerializeField]private LayerMask _triggerLayer;
        [SerializeField]private float     _visorRadius = 30f;

        [SerializeField]private Transform _currentTarget;

        public void Init(Transform target)
        {
            _currentTarget = target;
        }

        private void Update()
        {
            if (_currentTarget == null)
            {
                FindNearestTarget();
            }
        }

        private void FindNearestTarget()
        {
            Collider[] triggers = Physics.OverlapSphere(this.transform.position, _visorRadius, _triggerLayer);

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
    }
}
