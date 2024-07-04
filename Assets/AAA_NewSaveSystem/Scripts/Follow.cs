using System;
using UnityEngine;

namespace AAA_NewSaveSystem.Scripts
{
    [Serializable]
    public class Follow : MonoBehaviour
    {
        [SerializeField] private GameObject _go;
        [SerializeField] private Transform _transform;
        [SerializeField] private GameObject _prefab;
    }
}