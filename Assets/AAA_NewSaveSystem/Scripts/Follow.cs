using System;
using UnityEngine;

namespace AAA_NewSaveSystem.Scripts
{
    [Serializable]
    public class Follow : MonoBehaviour
    {
        [SerializeField] private float _force;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Transform _target;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private Mesh _mesh;
        [SerializeField] private Material _material;

        private void Start()
        {
            bool result = _meshFilter.mesh.name == _mesh.name;
            Debug.Log(result);
        }

        private void FixedUpdate()
        {
            if (_target != null && _rigidbody != null)
            {
                _rigidbody.AddForce((_target.position - transform.position).normalized * _force * Time.deltaTime);
            }
        }
    }
}