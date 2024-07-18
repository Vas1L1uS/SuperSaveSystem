using AAA_NewSaveSystem.Scripts.SaveSystem.Core;
using AAA_NewSaveSystem.Scripts.SaveSystem.UnityComponentsData;
using UnityEngine;

namespace AAA_NewSaveSystem.Scripts.SaveSystem.UnityComponentLoaders
{
    public class RigidbodyLoader : MonoBehaviour
    {
        [SerializeField] private GameObject _myPrefab;

        [HideInInspector] [SerializeField] private bool _isKinematic;
        [HideInInspector] [SerializeField] private Vector3 _velocity;
        [HideInInspector] [SerializeField] private Vector3 _angularVelocity;

        private Rigidbody _myRb;

        private void Awake()
        {
            _myRb = GetComponent<Rigidbody>();
            SaveManager.Loaded += Init;
            SaveManager.SavingStarted += SaveData;
        }

        private void Init(bool loaded)
        {
            if (loaded == false)
            {
                return;
            }

            Rigidbody prefabRb = _myPrefab.GetComponent<Rigidbody>();
            _myRb.CopyValues(prefabRb);
            _myRb.isKinematic = _isKinematic;
            
            if (_myRb.isKinematic == false)
            {
                _myRb.velocity = _velocity;
                _myRb.angularVelocity = _angularVelocity;
            }
        }

        private void SaveData()
        {
            _isKinematic = _myRb.isKinematic;
            _velocity = _myRb.velocity;
            _angularVelocity = _myRb.angularVelocity;
        }
        
        private void OnDestroy()
        {
            SaveManager.Loaded -= Init;
            SaveManager.SavingStarted -= SaveData;
        }
    }
}