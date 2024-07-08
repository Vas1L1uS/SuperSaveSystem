using System;
using AAA_NewSaveSystem.Scripts.SaveSystem;
using UnityEngine;

namespace AAA_NewSaveSystem.Scripts.Components
{
    [Serializable]
    public class CubeController : MonoBehaviour
    {
        [SerializeField] private GameObject _myPrefab;
        
        [SerializeField] private GameObject _target;
        [SerializeField] private float _speed;
        
        private void Awake()
        {
            RootSaver.ObjectsReady += Init;
        }

        private void Init()
        {
            var myPrefabMeshFilter = _myPrefab.GetComponent<MeshFilter>();
            var myPrefabMeshRenderer = _myPrefab.GetComponent<MeshRenderer>();
            var meshF = GetComponent<MeshFilter>();
            var meshR = GetComponent<MeshRenderer>();
            meshF.CopyValues(myPrefabMeshFilter);
            meshR.CopyValues(myPrefabMeshRenderer);
        }

        private void OnDestroy()
        {
            RootSaver.ObjectsReady -= Init;
        }
    }
}