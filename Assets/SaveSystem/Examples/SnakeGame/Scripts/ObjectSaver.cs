using SaveSystem.Core.Scripts.Core;
using UnityEngine;

namespace SaveSystem.Examples.SnakeGame.Scripts
{
    public class ObjectSaver : MonoBehaviour
    {
        [SerializeField] private bool _saveChildren;
        
        private void Awake()
        {
            SaveManager.SavingStarted += Save;
            SaveManager.LoadingStarted += DestroyGameObject;
        }

        private void Save()
        {
            SaveManager.AddGameObjectForSave(gameObject, _saveChildren);
            SaveManager.SavingStarted -= Save;
        }

        private void DestroyGameObject(bool loadingStarted)
        {
            if (loadingStarted == false) return;
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            SaveManager.SavingStarted -= Save;
            SaveManager.LoadingStarted -= DestroyGameObject;
        }
    }
}