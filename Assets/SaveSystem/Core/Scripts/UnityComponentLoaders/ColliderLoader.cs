using SaveSystem.Core.Scripts.Core;
using UnityEngine;

namespace SaveSystem.Core.Scripts.UnityComponentLoaders
{
    public class ColliderLoader : MonoBehaviour
    {
        [SerializeField] private GameObject _myPrefab;

        private void Awake()
        {
            SaveManager.Loaded += Init;
        }

        private void Init(bool loaded)
        {
            if (loaded == false) return;
            
            var myPrefabCollider = _myPrefab.GetComponent<Collider>();
            var myCollider = GetComponent<Collider>();
            myCollider.CopyValues(myPrefabCollider);
        }
        
        private void OnDestroy()
        {
            SaveManager.Loaded -= Init;
        }
    }
}