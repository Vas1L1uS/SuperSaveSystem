using AAAProject.Scripts;
using UnityEngine;

namespace AAAProject.CharacterMovement.Scripts
{
    public class NavMeshInput : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private NavMeshMovement _navMeshMovement;
        [SerializeField] private InputPlace _inputPlace;
        [SerializeField] private LayerMask _moveMask;

        [Header("Tap effect")] 
        [SerializeField] private Transform _effectParent;
        [SerializeField] private GameObject _effectPrefab;
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && _inputPlace.Pressed)
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _moveMask))
                {
                    _navMeshMovement.SetDestination(hit.point);

                    if (_effectPrefab != null)
                        Instantiate(_effectPrefab, hit.point, Quaternion.identity, _effectParent);
                }
            }
        }
    }
}