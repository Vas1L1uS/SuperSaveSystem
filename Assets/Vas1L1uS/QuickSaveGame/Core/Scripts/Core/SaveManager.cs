using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vas1L1uS.QuickSaveGame.Core.Scripts.UnityComponentsData;
using Object = UnityEngine.Object;

namespace Vas1L1uS.QuickSaveGame.Core.Scripts.Core
{
    public class SaveManager : MonoBehaviour
    {
        public static event Action<bool> LoadStarted;
        public static event Action<bool> LoadFinished;
        public static event Action SaveStarted;

        public static SaveManager Instance => _instance;
        public float LoadProgress => _loadProgress;
        
        public string SaveName = "MySave";

        private static event Action ObjectsCreated;
        
        [SerializeField] private bool _loadOnAwake = true;
        [SerializeField] private SaveType _saveType;
        [SerializeField] private Object[] _assets;
        [SerializeField] private bool _enableDebugLogs = true;
        [Header("Debug")]
        [SerializeField] private float _loadProgress;

        private static readonly Dictionary<int, Object> _objects = new();
        private static readonly List<(GameObject gameObject, bool saveChildren)> _otherGameObjectsForSave = new();
        private static readonly List<(ComponentData, Component)> _componentsForDeserialization = new();

        private static SaveManager _instance;
        private int _totalSavedObjects;
        private int _objectIndex;
        private int _componentIndex;
        private float _deltaTime;
        private int _count;
        private Coroutine _deserializeComponentsCoroutine;
        
        public static void AddGameObjectForSave(GameObject go, bool saveChildren)
        {
            for (int i = 0; i < _otherGameObjectsForSave.Count; i++)
            {
                if (_otherGameObjectsForSave[i].gameObject != go) continue;

                throw new Exception("This object is already in the list to save");
            }
            
            _otherGameObjectsForSave.Add((go, saveChildren));
        }

        [ContextMenu("RestartScene")]
        public void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        [ContextMenu("Save")]
        public void Save()
        {
            SaveStarted?.Invoke();
            _objectIndex = 0;
            _componentIndex = 0;
            
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
            
            switch (_saveType)
            {
                case SaveType.PlayerPrefs:
                    PlayerPrefs.SetString(SaveName, json);
                    break;
                case SaveType.PersistentDataPath:
                    string savePath = Path.Combine(Application.persistentDataPath, $"{SaveName}.json");
                    File.WriteAllText(savePath, json);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            if (_enableDebugLogs )Debug.Log($"Saved {_objectIndex} GameObjects, {_componentIndex} Components");
        }
        
        [ContextMenu("Load")]
        public void Load()
        {
            _deltaTime = Time.time;
            StopAllCoroutines();
            _count = 0;
            string json;

            switch (_saveType)
            {
                case SaveType.PlayerPrefs:
                    if (!PlayerPrefs.HasKey(SaveName))
                    {
                        LoadStarted?.Invoke(false);
                        LoadFinished?.Invoke(false);
                        return;
                    }

                    json = PlayerPrefs.GetString(SaveName);
                    break;
                case SaveType.PersistentDataPath:
                    string savePath = Path.Combine(Application.persistentDataPath, $"{SaveName}.json");

                    if (!File.Exists(savePath))
                    {
                        LoadStarted?.Invoke(false);
                        LoadFinished?.Invoke(false);
                        return;
                    }
                    
                    json = File.ReadAllText(savePath);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            LoadStarted?.Invoke(true);
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

        private void OnValidate()
        {
            SetInstance();
        }

        private void Awake()
        {
            SetInstance();

            if (_enableDebugLogs)
            {
                LoadFinished += result =>
                {
                    _deltaTime = Time.time - _deltaTime;
                    Debug.Log(result
                        ? $"Loaded {_objectIndex} GameObjects, {_componentIndex} Components. Loading time {_deltaTime} seconds."
                        : $"Save file not found");
                };
            }

            ObjectsCreated += DeserializeComponents;
            
            if (_loadOnAwake) Load();
        }

        private void SetInstance()
        {
            if (_instance == null)
            { 
                _instance = this;
            }
            else if (_instance != this)
            {
                if (_enableDebugLogs) Debug.LogError("There can only be one SaveManger object in the scene!");
                
                Destroy(gameObject);
            }
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
                ObjectsCreated?.Invoke();
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
                ComponentData compData = SerializeComponent(component);
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
        
        private int GetCurrentInstanceIDByPreviousInstanceId(int id)
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

        private void AddObject(Object obj, int id)
        {
            _objects.Add(id, obj);
        }
        
        private void AddComponentForDeserialization(ComponentData componentData, Component component)
        {
            _componentsForDeserialization.Add((componentData, component));
        }
        
        private void DeserializeComponent(ComponentData data, Component component)
        {
            JObject jsonObject = JObject.Parse(data.jsonData);
            ReplaceInstanceIDWithZero(jsonObject);
            JsonUtility.FromJsonOverwrite(jsonObject.ToString(), component);
        }
        
        private void ReplaceInstanceIDWithZero(JToken token)
        {
            if (token is JProperty jProperty && jProperty.Name == "instanceID")
            {
                jProperty.Value = GetCurrentInstanceIDByPreviousInstanceId(Convert.ToInt32(jProperty.Value.ToString()));
            }

            if (token is JProperty)
            {
                foreach (var child in token.Children())
                {
                    ReplaceInstanceIDWithZero(child);
                }
            }
            else if (token is JObject)
            {
                foreach (var child in token.Children<JProperty>())
                {
                    ReplaceInstanceIDWithZero(child);
                }
            }
            else if (token is JArray)
            {
                foreach (var child in token.Children())
                {
                    ReplaceInstanceIDWithZero(child);
                }
            }
        }
        
        private ComponentData SerializeComponent(Component component)
        {
            ComponentData data = new()
            {
                typeName = component.GetType().AssemblyQualifiedName,
                instanceId = component.GetInstanceID(),
            };

            switch (component)
            {
                case Transform transformComponent:
                    int parentInstanceId = 0;

                    if (transformComponent.parent != null) parentInstanceId = transformComponent.parent.gameObject.GetInstanceID();

                    TransformData transformData = new()
                    {
                        LocalPosition = transformComponent.localPosition,
                        LocalRotation = transformComponent.localRotation,
                        LocalScale = transformComponent.localScale,
                        ParentInstanceId = parentInstanceId,
                    };
                    
                    data.jsonData = JsonUtility.ToJson(transformData);

                    break;
                case Rigidbody rb:
                    RigidbodyData rigidbodyData = new()
                    {
                        isKinematic = rb.isKinematic,
                        velocity = rb.velocity,
                        angularVelocity = rb.angularVelocity,
                        useGravity = rb.useGravity,
                        drag = rb.drag,
                        mass = rb.mass,
                        angularDrag = rb.angularDrag,
                        detectCollisions = rb.detectCollisions,
                        freezeRotation = rb.freezeRotation,
                        inertiaTensor = rb.inertiaTensor,
                        sleepThreshold = rb.sleepThreshold,
                        solverIterations = rb.solverIterations,
                        automaticInertiaTensor = rb.automaticInertiaTensor,
                        centerOfMass = rb.centerOfMass,
                        inertiaTensorRotation = rb.inertiaTensorRotation,
                        maxAngularVelocity = rb.maxAngularVelocity,
                        maxDepenetrationVelocity = rb.maxDepenetrationVelocity,
                        maxLinearVelocity = rb.maxLinearVelocity,
                        solverVelocityIterations = rb.solverVelocityIterations,
                        automaticCenterOfMass = rb.automaticCenterOfMass,
                    };
                    
                    data.jsonData = JsonUtility.ToJson(rigidbodyData);
                    break;
                default:
                    try
                    {
                        data.jsonData = JsonUtility.ToJson(component);
                    }
                    catch
                    {
                        // ignored
                    }

                    break;
            }

            return data;
        }

        private IEnumerator LoadRoutine(RootSaverData rootData)
        {
            if (transform.childCount > 0)
            {
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    Destroy(transform.GetChild(i).gameObject);
                    yield return null;
                }
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
                _loadProgress = (float)((float)(_objectIndex + _componentIndex) / (float)_totalSavedObjects);
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
                
                if (type == typeof(Rigidbody))
                {
                    RigidbodyData rigidbodyData = new();
                    rigidbodyData = (RigidbodyData)JsonUtility.FromJson(compData.jsonData, rigidbodyData.GetType());
                    Rigidbody rb = (Rigidbody) component;
                    rb.isKinematic = true;
                    rb.useGravity = rigidbodyData.useGravity;
                    rb.automaticInertiaTensor = rigidbodyData.automaticInertiaTensor;
                    rb.automaticCenterOfMass = rigidbodyData.automaticCenterOfMass;
                    rb.drag = rigidbodyData.drag;
                    rb.mass = rigidbodyData.mass;
                    rb.angularDrag = rigidbodyData.angularDrag;
                    rb.inertiaTensor = rigidbodyData.inertiaTensor;
                    rb.sleepThreshold = rigidbodyData.sleepThreshold;
                    rb.solverIterations = rigidbodyData.solverIterations;
                    rb.centerOfMass = rigidbodyData.centerOfMass;
                    rb.inertiaTensorRotation = rigidbodyData.inertiaTensorRotation;
                    rb.maxAngularVelocity = rigidbodyData.maxAngularVelocity;
                    rb.maxDepenetrationVelocity = rigidbodyData.maxDepenetrationVelocity;
                    rb.maxLinearVelocity = rigidbodyData.maxLinearVelocity;
                    rb.solverVelocityIterations = rigidbodyData.solverVelocityIterations;
                    rb.detectCollisions = rigidbodyData.detectCollisions;
                    rb.freezeRotation = rigidbodyData.freezeRotation;
                    _componentIndex++;
                    continue;
                }

                AddObject(component, compData.instanceId);
                AddComponentForDeserialization(compData, component);
            }
            
            newObject.SetActive(gameObjectData.activeSelf);
            _loadProgress = (float)((float)(_objectIndex + _componentIndex) / (float)_totalSavedObjects);
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
                    DeserializeComponent(data, component);
                }
                catch
                {
                    // ignored
                }
                
                _componentIndex++;
                if (i % 1000 == 0) yield return null;
                _loadProgress = (float)((float)(_objectIndex + _componentIndex) / (float)_totalSavedObjects);
            }
            
            _componentsForDeserialization.Clear();
            LoadFinished?.Invoke(true);
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
            LoadFinished = null;
            SaveStarted = null;
            LoadStarted = null;
            ObjectsCreated = null;
        }

        public enum SaveType
        {
            PlayerPrefs,
            PersistentDataPath
        }
    }
}