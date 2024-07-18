using AAA_NewSaveSystem.Scripts.SaveSystem.Core;
using UnityEngine;

namespace AAA_NewSaveSystem.Scripts.SaveSystem.UnityComponentLoaders
{
    public class VisualLoader : MonoBehaviour
    {
        [SerializeField] private GameObject _myPrefab;

        private void Awake()
        {
            SaveManager.Loaded += Init;
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
            SaveManager.Loaded -= Init;
        }
    }
}