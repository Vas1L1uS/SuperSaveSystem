using System;
using AAA_NewSaveSystem.Scripts.SaveSystem;
using UnityEngine;

namespace AAA_NewSaveSystem.Scripts
{
    [Serializable]
    public class CubeController : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;

        public void Init(GameObject prefab)
        {
            _prefab = prefab;
        }

        private void Awake()
        {
            RootSaver.LoadCompleted += InitDefault;
        }

        private void InitDefault()
        {
            var meshFilter = GetComponent<MeshFilter>();
            meshFilter.mesh = _prefab.GetComponent<MeshFilter>().sharedMesh;
            var meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.material = _prefab.GetComponent<MeshRenderer>().sharedMaterial;
        }
    }
}