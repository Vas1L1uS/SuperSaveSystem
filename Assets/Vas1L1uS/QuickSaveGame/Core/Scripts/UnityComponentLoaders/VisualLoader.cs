using UnityEngine;
using Vas1L1uS.QuickSaveGame.Core.Scripts.Core;

namespace Vas1L1uS.QuickSaveGame.Core.Scripts.UnityComponentLoaders
{
    public class VisualLoader : MonoBehaviour
    {
        [SerializeField] private GameObject _myPrefab;

        private void Awake()
        {
            SaveManager.LoadFinished += Init;
        }

        private void Init(bool loaded)
        {
            if (loaded == false) return;
            
            var myPrefabMeshFilter = _myPrefab.GetComponent<MeshFilter>();
            var myPrefabMeshRenderer = _myPrefab.GetComponent<MeshRenderer>();
            var meshF = GetComponent<MeshFilter>();
            var meshR = GetComponent<MeshRenderer>();
            meshF.CopyValues(myPrefabMeshFilter);
            meshR.CopyValues(myPrefabMeshRenderer);
        }
        
        private void OnDestroy()
        {
            SaveManager.LoadFinished -= Init;
        }
    }
}