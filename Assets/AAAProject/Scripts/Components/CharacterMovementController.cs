using System;
using AAAProject.Scripts.Extensions;
using UnityEngine;

namespace AAAProject.Scripts.Components
{
    public class CharacterMovementController : MBehaviour
    {
        [SerializeField]private CharacterController _characterController;
        [SerializeField]private float               _speed;

        private                 Transform           _target;

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        private void Update()
        {
            _characterController.SimpleMove(GetMoveVector());
        }

        private Vector3 GetMoveVector()
        {
            if (_target == null) return Vector3.zero;

            Vector3 direction = _target.position - transform.position;
            direction.y = 0;
            Vector3 directionNormalized = direction.normalized;
            return directionNormalized * _speed;
        } 
    }
}
