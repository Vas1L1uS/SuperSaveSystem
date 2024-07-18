using System;
using AAA_NewSaveSystem.Scripts.Demo.Player;
using AAA_NewSaveSystem.Scripts.SaveSystem.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AAA_NewSaveSystem.Scripts.Demo
{
    public class FoodSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _foodPrefab;
        [SerializeField] private Picker _picker;
        [SerializeField] private Size _mapZone;
        [Space]
        [SerializeField] private PickableItem _currentFood;

        private void Awake()
        {
            RootSaver.Loaded += Init;
        }

        private void Init(bool loaded)
        {
            if (_currentFood == null)
            {
                SpawnNewFood();
            }
            
            SubscribeOnComponents();
        }

        private void SubscribeOnComponents()
        {
            _picker.Picked += PickerOnPicked;
        }
        
        private void UnsubscribeFromComponents()
        {
            if (_picker != null) _picker.Picked -= PickerOnPicked;
        }

        private void PickerOnPicked(PickableItem item)
        {
            SpawnNewFood();
        }

        private void SpawnNewFood()
        {
            _currentFood = null;
            float x = Random.Range(_mapZone.SizeX.x, _mapZone.SizeX.y);
            float z = Random.Range(_mapZone.SizeY.x, _mapZone.SizeY.y);
            _currentFood = Instantiate(_foodPrefab, new Vector3(x, 0.25f, z), Quaternion.identity, transform).GetComponent<PickableItem>();
        }

        private void OnDestroy()
        {
            RootSaver.Loaded -= Init;
            UnsubscribeFromComponents();
        }

        [Serializable]
        public struct Size
        {
            public Vector2 SizeX;
            public Vector2 SizeY;
        }
    }
}