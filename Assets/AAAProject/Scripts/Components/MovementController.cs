using UnityEngine;

namespace AAAProject.Scripts.Components
{
    public class MovementController : MonoBehaviour
    {
        public Rigidbody Rigidbody => _rigidbody;

        [SerializeField]private Rigidbody _rigidbody;
        [SerializeField]private float     _speed;

        private Transform _target;
        private bool      _isInitialized;

        public void Init(Transform target, Vector3 velocity, Vector3 angularVelocity)
        {
            _rigidbody.velocity = velocity;
            _rigidbody.angularVelocity = angularVelocity;
            
            if (target == null) return;
            
            _isInitialized = true;
            _target = target;
        }

        private void Update()
        {
            if (!_isInitialized) return;
            
            _rigidbody.AddForce((_target.position - this.transform.position).normalized * _speed * Time.deltaTime, ForceMode.VelocityChange);
        }
    }
}
