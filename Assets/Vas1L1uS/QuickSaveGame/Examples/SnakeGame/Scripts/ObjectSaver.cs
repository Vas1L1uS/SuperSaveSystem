using UnityEngine;
using Vas1L1uS.QuickSaveGame.Core.Scripts.Core;

namespace Vas1L1uS.QuickSaveGame.Examples.SnakeGame.Scripts
{
    public class ObjectSaver : MonoBehaviour
    {
        [SerializeField] private bool _saveChildren;
        
        private void Awake()
        {
            SaveManager.SaveStarted += Save;
            SaveManager.LoadStarted += DestroyGameObject;
        }

        private void Save()
        {
            SaveManager.AddGameObjectForSave(gameObject, _saveChildren);
            SaveManager.SaveStarted -= Save;
        }

        private void DestroyGameObject(bool loadStarted)
        {
            if (loadStarted == false) return;
            
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            SaveManager.SaveStarted -= Save;
            SaveManager.LoadStarted -= DestroyGameObject;
        }
    }
}