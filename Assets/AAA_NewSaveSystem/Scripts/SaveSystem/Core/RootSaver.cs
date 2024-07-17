using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AAA_NewSaveSystem.Scripts.SaveSystem.Core
{
    public class RootSaver : MonoBehaviour
    {
        public static Action ObjectsCreated;
        public static Action<bool> Loaded;

        public string SaveName = "MySave";
        
        [SerializeField] private int _count;
        [SerializeField] private Object[] _assets;

        private static Dictionary<int, Object> _objects = new();
        private static readonly List<(GameObject gameObject, bool saveChildren)> _otherGameObjectsForSave = new();

        private int _totalObjects;
        private int _currentIndex;
        private int _componentIndex;

        public static int GetCurrentInstanceIDByPreviousInstanceID(int id)
        {
            try
            {
                Object result = _objects[id];
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

        public static void AddGameObjectForSave(GameObject go, bool saveChildren)
        {
            for (int i = 0; i < _otherGameObjectsForSave.Count; i++)
            {
                if (_otherGameObjectsForSave[i].gameObject != go) continue;

                throw new Exception("This object is already in the list to save");
            }
            
            _otherGameObjectsForSave.Add((go, saveChildren));
        }
        
        [ContextMenu("Save")]
        public void Save()
        {
            _currentIndex = 0;
            _componentIndex = 0;
            
            string savePath = Path.Combine(Application.persistentDataPath, $"{SaveName}.json");
            List<GameObjectData> children = new();

            for (int i = 0; i < transform.childCount; i++)
            {
                children.Add(SaveObject(transform.GetChild(i).gameObject, true));
            }

            List<int> assets = new(); 
            
            for (int i = 0; i < _assets.Length; i++)
            {
                assets.Add(_assets[i].GetInstanceID());
            }
            
            List<GameObjectData> otherGameObjectsForSave = new(); 
            
            for (int i = 0; i < _otherGameObjectsForSave.Count; i++)
            {
                otherGameObjectsForSave.Add(SaveObject(_otherGameObjectsForSave[i].gameObject, _otherGameObjectsForSave[i].saveChildren));
            }

            RootSaverData rootSaverData = new()
            {
                children = children,
                assets = assets,
                otherSavedGameObjects = otherGameObjectsForSave
            };

            string json = JsonUtility.ToJson(rootSaverData);
            File.WriteAllText(savePath, json);
            Debug.Log($"Saved {_currentIndex} GameObjects, {_componentIndex} Components");
        }

        [ContextMenu("ClearSave")]
        public void ClearSave()
        {
            string savePath = Path.Combine(Application.persistentDataPath, $"{SaveName}.json");
            File.Delete(savePath);
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
                StartCoroutine(StagesRoutine());
            }
        }

        private IEnumerator StagesRoutine()
        {
            yield return null;
            ObjectsCreated?.Invoke();
            yield return null;
            yield return null;
            yield return null;
            Loaded?.Invoke(true);
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            _objects = new();
        }

        private void Start()
        {
            Loaded += result =>
            {
                Debug.Log(result
                    ? $"Loaded {_currentIndex} GameObjects, {_componentIndex} Components"
                    : $"Save file not found");
            };
            
            Load();
        }

        private GameObjectData SaveObject(GameObject go, bool saveChildren)
        {
            _currentIndex++;
            List<GameObjectData> children = new();

            GameObjectData gameObjectData = new()
            {
                name = go.name,
                instanceId = go.GetInstanceID(),
                activeSelf = go.activeSelf,
                children = children,
                layer = go.layer,
                tag = go.tag,
            };

            foreach (Component component in go.GetComponents<Component>())
            {
                ComponentData compData = ComponentSerializer.SerializeComponent(component);
                gameObjectData.components.Add(compData);
                _componentIndex++;
            }

            if (saveChildren)
            {
                for (var i = 0; i < go.transform.childCount; i++)
                {
                    Transform child = go.transform.GetChild(i);
                    children.Add(SaveObject(child.gameObject, true));
                }
            }

            return gameObjectData;
        }

        [ContextMenu("Load")]
        public void Load()
        {
            StopAllCoroutines();
            _objects = new();
            _count = 0;
            string savePath = Path.Combine(Application.persistentDataPath, $"{SaveName}.json");

            if (!File.Exists(savePath))
            {
                Loaded?.Invoke(false);
                return;
            }

            string json = File.ReadAllText(savePath);
            RootSaverData rootData = JsonUtility.FromJson<RootSaverData>(json);

            _currentIndex = 0;
            _componentIndex = 0;

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            
            StartCoroutine(LoadRoutine(rootData));
        }

        private IEnumerator LoadRoutine(RootSaverData rootData)
        {
            StartLoad();

            for (int i = 0; i < rootData.assets.Count; i++)
            {
                _objects.Add(rootData.assets[i], _assets[i]);
            }
            
            foreach (GameObjectData child in rootData.children)
            {
                StartCoroutine(LoadObject(child, transform));
                yield return null;
            }
            
            foreach (GameObjectData other in rootData.otherSavedGameObjects)
            {
                StartCoroutine(LoadObject(other, null));
                yield return null;
            }
            
            yield return null;
            LoadComplete();
        }

        private IEnumerator LoadObject(GameObjectData gameObjectData, Transform parent)
        {
            StartLoad();
            _currentIndex++;
            GameObject newObject = new();
            AddObject(newObject, gameObjectData.instanceId);
            newObject.name = gameObjectData.name;
            newObject.layer = gameObjectData.layer;
            newObject.tag = gameObjectData.tag;
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
            
            newObject.SetActive(gameObjectData.activeSelf);
            yield return null;
            LoadComplete();
        }
    }
}