using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AAA_NewSaveSystem.Scripts.SaveSystem
{
    public class RootSaver : MonoBehaviour
    {
        public static Action ObjectsReady;
        public static Action LoadCompleted;
        
        [SerializeField] private int _count;

        private static Dictionary<int, Object> _objects = new();

        private int _totalObjects;
        private int _currentIndex;
        private int _componentIndex;

        public static int GetCurrentObjectIDByPreviousID(int id)
        {
            try
            {
                var result = _objects[id];
                return result.GetInstanceID();
            }
            catch
            {
                return 0;
            }
        }

        public static void AddObject(Object obj, int id)
        {
            _objects.Add(id, obj);
        }

        private void StartLoad()
        {
            _count++;
        }
        
        private void LoadComplete()
        {
            _count--;

            if (_count == 0)
            {
                StartCoroutine(FinishedRoutine());
            }
        }

        private IEnumerator FinishedRoutine()
        {
            yield return null;
            ObjectsReady?.Invoke();
            yield return null;
            LoadCompleted?.Invoke();
        }

        private void Awake()
        {
            LoadCompleted += () =>
            {
                Debug.Log($"LoadCompleted {_currentIndex} GameObjects, {_componentIndex} Components");
            };
        }

        [ContextMenu("Save")]
        public void Save()
        {
            _currentIndex = 0;
            _componentIndex = 0;
            string savePath = Path.Combine(Application.persistentDataPath, "sceneData.json");
            string json = JsonUtility.ToJson(SaveObject(gameObject));
            File.WriteAllText(savePath, json);
            Debug.Log($"Saved {_currentIndex} GameObjects, {_componentIndex} Components");
        }

        private GameObjectData SaveObject(GameObject go)
        {
            List<GameObjectData> children = new();

            GameObjectData gameObjectData = new GameObjectData
            {
                name = go.name,
                id = _currentIndex,
                instanceId = go.GetInstanceID(),
                children = children,
            };

            foreach (Component component in go.GetComponents<Component>())
            {
                ComponentData compData = ComponentSerializer.SerializeComponent(component);
                gameObjectData.components.Add(compData);
                _componentIndex++;
            }

            _currentIndex++;

            for (var i = 0; i < go.transform.childCount; i++)
            {
                Transform child = go.transform.GetChild(i);
                children.Add(SaveObject(child.gameObject));
            }

            gameObjectData.count = _currentIndex;

            return gameObjectData;
        }

        [ContextMenu("Load")]
        public void Load()
        {
            StopAllCoroutines();
            _objects = new();
            _count = 0;
            string savePath = Path.Combine(Application.persistentDataPath, "sceneData.json");

            if (!File.Exists(savePath)) return;

            string json = File.ReadAllText(savePath);
            GameObjectData rootData = JsonUtility.FromJson<GameObjectData>(json);

            string savePathLoad = Path.Combine(Application.persistentDataPath, "sceneDataLoad.json");
            File.WriteAllText(savePathLoad, json);

            _currentIndex = 0;
            _componentIndex = 0;
            _totalObjects = rootData.count - 1;
            
            for (int i = transform.childCount - 1; i > 0; i--)
            {
                Destroy(transform.GetChild(i));
            }
            
            StartCoroutine(LoadRoutine(rootData));
        }

        private IEnumerator LoadRoutine(GameObjectData rootData)
        {
            StartLoad();
            
            foreach (GameObjectData child in rootData.children)
            {
                StartCoroutine(LoadObject(child, transform));
                yield return null;
            }
            
            yield return null;
            LoadComplete();
        }

        private IEnumerator LoadObject(GameObjectData gameObjectData, Transform parent)
        {
            StartLoad();
            _currentIndex++;
            GameObject newObject = new GameObject();
            AddObject(newObject, gameObjectData.instanceId);
            newObject.name = gameObjectData.name;
            newObject.transform.SetParent(parent);

            foreach (GameObjectData child in gameObjectData.children)
            {
                StartCoroutine(LoadObject(child, newObject.transform));
                yield return null;
            }

            foreach (ComponentData compData in gameObjectData.components)
            {
                ComponentSerializer.DeserializeComponent(newObject, compData);
                _componentIndex++;
            }
            
            yield return null;
            LoadComplete();
        }
    }
}