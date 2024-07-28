using UnityEngine;
using Vas1L1uS.QuickSaveGame.Core.Scripts.Core;

namespace Vas1L1uS.QuickSaveGame.Core.Scripts.UnityComponentLoaders
{
    public class ColliderLoader : MonoBehaviour
    {
        [SerializeField] private GameObject _myPrefab;

        private void Awake()
        {
            SaveManager.LoadFinished += Init;
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
            SaveManager.LoadFinished -= Init;
        }
    }
}