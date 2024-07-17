using AAA_NewSaveSystem.Scripts.SaveSystem.Core;
using UnityEngine;

namespace AAA_NewSaveSystem.Scripts.Demo
{
    public class ObjectSaver : MonoBehaviour
    {
        [SerializeField] private bool _saveChildren;
        
        private void Awake()
        {
            RootSaver.SavingStarted += Save;
            RootSaver.LoadingStarted += DestroyGameObject;
        }

        private void Save()
        {
            RootSaver.AddGameObjectForSave(gameObject, _saveChildren);
            RootSaver.SavingStarted -= Save;
        }

        private void DestroyGameObject(bool loadingStarted)
        {
            if (loadingStarted == false) return;
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            RootSaver.SavingStarted -= Save;
            RootSaver.LoadingStarted -= DestroyGameObject;
        }
    }
}