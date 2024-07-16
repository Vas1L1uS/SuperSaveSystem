using AAA_NewSaveSystem.Scripts.SaveSystem.Core;
using UnityEngine;

namespace AAA_NewSaveSystem.Scripts.SaveSystem.UnityComponentLoaders
{
    public class RigidbodyLoader : MonoBehaviour
    {
        [SerializeField] private GameObject _myPrefab;

        private Rigidbody _myRb;
        private bool _isKinematic;
        
        private void Awake()
        {
            _myRb = GetComponent<Rigidbody>();
            _isKinematic = _myRb.isKinematic;
            _myRb.isKinematic = true;
            RootSaver.Loaded += Init;
        }

        private void Init(bool loaded)
        {
            if (loaded == false)
            {
                _myRb.isKinematic = _isKinematic;
                return;
            }

            Rigidbody prefabRb = _myPrefab.GetComponent<Rigidbody>();
            _myRb.CopyValues(prefabRb);
        }
        
        private void OnDestroy()
        {
            RootSaver.Loaded -= Init;
        }
    }
}