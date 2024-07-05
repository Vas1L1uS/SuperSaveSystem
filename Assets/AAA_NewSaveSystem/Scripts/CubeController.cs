using System;
using AAA_NewSaveSystem.Scripts.SaveSystem;
using UnityEngine;

namespace AAA_NewSaveSystem.Scripts
{
    [Serializable]
    public class CubeController : MonoBehaviour
    {
        private void Awake()
        {
            RootSaver.LoadCompleted += Init;
        }

        private void Init()
        {
            Debug.Log(GetComponent<MeshRenderer>().material.color);
        }
    }
}