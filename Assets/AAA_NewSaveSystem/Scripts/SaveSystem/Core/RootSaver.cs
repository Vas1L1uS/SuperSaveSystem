using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using AAA_NewSaveSystem.Scripts.SaveSystem.UnityComponentsData;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AAA_NewSaveSystem.Scripts.SaveSystem.Core
{
    public class RootSaver : MonoBehaviour
    {
        public static Action<bool> LoadingStarted;
        public static Action<bool> Loaded;
        public static Action SavingStarted;

        public string SaveName = "MySave";
        
        private static Action _objectsCreated;

        [SerializeField] private float _progress;
        [SerializeField] private Object[] _assets;

        private static readonly Dictionary<int, Object> _objects = new();
        private static readonly List<(GameObject gameObject, bool saveChildren)> _otherGameObjectsForSave = new();
        private static readonly List<(ComponentData, Component)> _componentsForDeserialization = new();

        private int _totalSavedObjects;
        private int _objectIndex;
        private int _componentIndex;
        private float _deltaTime;
        private int _count;

        private Coroutine _deserializeComponentsCoroutine;

        public static int GetCurrentInstanceIDByPreviousInstanceId(int id)
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
            SavingStarted?.Invoke();
            _objectIndex = 0;
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

            _totalSavedObjects = _objectIndex + _componentIndex;
            
            RootSaverData rootSaverData = new()
            {
                totalSaveObjects = _totalSavedObjects,
                children = children,
                assets = assets,
                otherSavedGameObjects = otherGameObjectsForSave
            };

            string json = JsonUtility.ToJson(rootSaverData);
            File.WriteAllText(savePath, json);
            Debug.Log($"Saved {_objectIndex} GameObjects, {_componentIndex} Components");
        }
        
        [ContextMenu("Load")]
        public void Load()
        {
            _deltaTime = Time.time;
            StopAllCoroutines();
            _count = 0;
            string savePath = Path.Combine(Application.persistentDataPath, $"{SaveName}.json");

            if (!File.Exists(savePath))
            {
                LoadingStarted?.Invoke(false);
                Loaded?.Invoke(false);
                return;
            }
            
            LoadingStarted?.Invoke(true);
            string json = File.ReadAllText(savePath);
            RootSaverData rootData = JsonUtility.FromJson<RootSaverData>(json);
            _totalSavedObjects = rootData.totalSaveObjects;
            _objectIndex = 0;
            _componentIndex = 0;

            StartCoroutine(LoadRoutine(rootData));
        }

        [ContextMenu("ClearSave")]
        public void ClearSave()
        {
            string savePath = Path.Combine(Application.persistentDataPath, $"{SaveName}.json");
            File.Delete(savePath);
        }
        
        private void Start()
        {
            Loaded += result =>
            {
                _deltaTime = Time.time - _deltaTime;
                Debug.Log(result
                    ? $"Loaded {_objectIndex} GameObjects, {_componentIndex} Components. Loading time {_deltaTime} seconds."
                    : $"Save file not found");
            };

            _objectsCreated += DeserializeComponents;
            
            Load();
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
                _objectsCreated?.Invoke();
            }
        }

        private void DeserializeComponents()
        {
            if (_deserializeComponentsCoroutine != null) StopCoroutine(_deserializeComponentsCoroutine);
            _deserializeComponentsCoroutine = StartCoroutine(DeserializeComponentsRoutine());
        }

        private GameObjectData SaveObject(GameObject go, bool saveChildren)
        {
            _objectIndex++;
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
        
        private GameObject GetGameObjectByPreviousId(int id)
        {
            if (_objects.ContainsKey(id) == false) return null;
            
            if (_objects[id] is GameObject go)
            {
                return go;
            }

            return null;
        }

        private void AddObject(Object obj, int id)
        {
            _objects.Add(id, obj);
        }
        
        private void AddComponentForDeserialization(ComponentData componentData, Component component)
        {
            _componentsForDeserialization.Add((componentData, component));
        }

        private IEnumerator LoadRoutine(RootSaverData rootData)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
                yield return null;
            }

            _objects.Clear();
            _otherGameObjectsForSave.Clear();
            StartLoad();

            for (int i = 0; i < rootData.assets.Count; i++)
            {
                _objects.Add(rootData.assets[i], _assets[i]);
            }

            for (int i = 0; i < rootData.children.Count; i++)
            {
                GameObjectData child = rootData.children[i];
                StartCoroutine(LoadObjectRoutine(child, transform));
                if (i % 50 == 0) yield return null;
            }

            for (int i = 0; i < rootData.otherSavedGameObjects.Count; i++)
            {
                StartCoroutine(LoadObjectRoutine(rootData.otherSavedGameObjects[i], null));
                if (i % 50 == 0) yield return null;
            }

            yield return null;
            LoadComplete();
        }

        private IEnumerator LoadObjectRoutine(GameObjectData gameObjectData, Transform parent)
        {
            StartLoad();
            _objectIndex++;
            GameObject newObject = new();
            AddObject(newObject, gameObjectData.instanceId);
            newObject.name = gameObjectData.name;
            newObject.layer = gameObjectData.layer;
            newObject.tag = gameObjectData.tag;
            newObject.transform.SetParent(parent);

            foreach (GameObjectData child in gameObjectData.children)
            {
                StartCoroutine(LoadObjectRoutine(child, newObject.transform));
                
                if (_objectIndex % 50 == 0) yield return null;
                _progress = (float)((float)(_objectIndex + _componentIndex) / (float)_totalSavedObjects);
            }

            foreach (ComponentData compData in gameObjectData.components)
            {
                Type type = Type.GetType(compData.typeName);

                if (type == null) continue;

                if (type == typeof(Transform))
                {
                    TransformData transformData = new();
                    transformData = (TransformData)JsonUtility.FromJson(compData.jsonData, transformData.GetType());
                    GameObject objParent = GetGameObjectByPreviousId(transformData.ParentInstanceId);
                    if (objParent != null) newObject.transform.SetParent(objParent.transform);
                    newObject.transform.localPosition = transformData.LocalPosition;
                    newObject.transform.localRotation = transformData.LocalRotation;
                    newObject.transform.localScale = transformData.LocalScale;
                    AddObject(newObject.transform, compData.instanceId);
                    _componentIndex++;
                    continue;
                }
                
                Component component = newObject.AddComponent(type);
                AddObject(component, compData.instanceId);
                AddComponentForDeserialization(compData, component);
            }
            
            newObject.SetActive(gameObjectData.activeSelf);
            _progress = (float)((float)(_objectIndex + _componentIndex) / (float)_totalSavedObjects);
            LoadComplete();
        }

        private IEnumerator DeserializeComponentsRoutine()
        {
            yield return null;
            
            for (int i = 0; i < _componentsForDeserialization.Count; i++)
            {
                try
                {
                    (ComponentData data, Component component) = _componentsForDeserialization[i];
                    ComponentSerializer.DeserializeComponent(data, component);
                }
                catch
                {
                    // ignored
                }
                
                _componentIndex++;
                if (i % 1000 == 0) yield return null;
                _progress = (float)((float)(_objectIndex + _componentIndex) / (float)_totalSavedObjects);
            }
            
            _componentsForDeserialization.Clear();
            Loaded?.Invoke(true);
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            _objects.Clear();
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            Loaded = null;
            SavingStarted = null;
            LoadingStarted = null;
            _objectsCreated = null;
        }
    }
}