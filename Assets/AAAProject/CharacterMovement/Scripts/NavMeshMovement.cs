using AAAProject.Scripts.Extensions;
using UnityEngine;
using UnityEngine.AI;

namespace AAAProject.CharacterMovement.Scripts
{
    public class NavMeshMovement : MBehaviour
    {
        [SerializeField]  private NavMeshAgent _agent;
        [Header("Target settings")] 
        [SerializeField] private bool _goToTarget;
        [SerializeField] private Transform _target;
        [Header("MoveAnimation settings")]
        [SerializeField] private AnimMoveController _animMoveController;

        public void SetDestination(Vector3 position)
        {
            _agent.SetDestination(position);
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }
        
        protected override void Awake()
        {
            if (_target != null) _agent.SetDestination(_target.position);
            
            base.Awake();
        }

        private void Update()
        {
            if (_goToTarget)
            {
                _agent.SetDestination(_target.position);
            }

            if (_animMoveController != null)
            {
                _animMoveController.SetSpeed(_agent.velocity.magnitude);
            }
        }

        protected override void Pause()
        {
            _agent.isStopped = true;
            base.Pause();
        }

        protected override void Resume()
        {
            _agent.isStopped = false;
            base.Resume();
        }
    }
}
