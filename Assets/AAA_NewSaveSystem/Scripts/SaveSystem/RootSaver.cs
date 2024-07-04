using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace AAA_NewSaveSystem.Scripts.SaveSystem
{
    public class RootSaver : MonoBehaviour
    {
        public static Action Inited;

        [SerializeField] private Text _loadProgress;

        [SerializeField] private AssetCollection _meshCollections;
        [SerializeField] private int _count;
        private static AssetCollection _sMeshCollections;

        private int _totalObjects;
        private int _currentIndex;
        private int _componentIndex;

        private void LoadStarted()
        {
            _count++;
        }
        
        private void LoadCompleted()
        {
            _count--;
            _loadProgress.text =((float)((float)_currentIndex/(float)_totalObjects)).ToString();

            if (_count == 0)
            {
                Debug.Log($"Loaded {_currentIndex} GameObjects, {_componentIndex} Components");
            }
        }

        public AssetCollection GetAssetCollectionByType(AssetCollectionType assetType)
        {
            switch (assetType)
            {
                case AssetCollectionType.Mesh:
                    return _sMeshCollections;
            }

            return null;
        }

        private void Awake()
        {
            _sMeshCollections = _meshCollections;
            Inited += () =>
            {
                Debug.Log($"Loaded {_currentIndex} GameObjects, {_componentIndex} Components");
            };
        }

        public enum AssetCollectionType
        {
            Mesh
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
            LoadStarted();
            
            foreach (GameObjectData child in rootData.children)
            {
                StartCoroutine(LoadObject(child, transform));
                yield return null;
            }
            
            yield return null;
            LoadCompleted();
        }

        private IEnumerator LoadObject(GameObjectData gameObjectData, Transform parent)
        {
            LoadStarted();
            _currentIndex++;
            GameObject newObject = new GameObject();
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
            LoadCompleted();
        }

        private IEnumerator InitRoutine()
        {
            yield return null;
            yield return null;

            // var actions = Inited.GetInvocationList();
            //
            // int delay = 10;
            // int current = 0;
            //
            // for (int i = 0; i < actions.Length; i++)
            // {
            //     actions[i].DynamicInvoke();
            //     current++;
            //
            //     if (current > delay)
            //     {
            //         yield return null;
            //         current = 0;
            //     }
            // }
            //
            // yield return null;
        }
    }
}