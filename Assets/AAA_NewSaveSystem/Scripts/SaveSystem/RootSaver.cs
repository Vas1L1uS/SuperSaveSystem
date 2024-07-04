using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AAA_NewSaveSystem.Scripts.SaveSystem
{
    public class RootSaver : MonoBehaviour
    {
        public static Action Inited;

        [SerializeField] private AssetCollection _meshCollections;
        private static AssetCollection _sMeshCollections;

        private int _currentIndex;
        private int _componentIndex;

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

            return gameObjectData;
        }

        [ContextMenu("Load")]
        public void Load()
        {
            string savePath = Path.Combine(Application.persistentDataPath, "sceneData.json");

            if (!File.Exists(savePath)) return;

            string json = File.ReadAllText(savePath);
            GameObjectData rootData = JsonUtility.FromJson<GameObjectData>(json);

            string savePathLoad = Path.Combine(Application.persistentDataPath, "sceneDataLoad.json");
            File.WriteAllText(savePathLoad, json);

            _currentIndex = 0;
            _componentIndex = 0;

            // Clear current scene (optional)
            foreach (GameObject go in FindObjectsOfType<GameObject>())
            {
                if (go != gameObject) // Only destroy objects that are part of the scene
                {
                    Destroy(go);
                }
            }

            foreach (GameObjectData child in rootData.children)
            {
                LoadObject(child, transform);
            }

            Debug.Log($"Loaded {_currentIndex} GameObjects, {_componentIndex} Components");
        }

        private void LoadObject(GameObjectData gameObjectData, Transform parent)
        {
            _currentIndex++;
            GameObject newObject = new GameObject();
            newObject.name = gameObjectData.name;
            newObject.transform.SetParent(parent);

            foreach (GameObjectData child in gameObjectData.children)
            {
                LoadObject(child, newObject.transform);
            }

            foreach (ComponentData compData in gameObjectData.components)
            {
                ComponentSerializer.DeserializeComponent(newObject, compData);
                _componentIndex++;
            }

            StartCoroutine(InitRoutine());
        }

        private IEnumerator InitRoutine()
        {
            yield return null;
            yield return null;

            var actions = Inited.GetInvocationList();

            int delay = 10;
            int current = 0;

            for (int i = 0; i < actions.Length; i++)
            {
                actions[i].DynamicInvoke();
                current++;

                if (current > delay)
                {
                    yield return null;
                    current = 0;
                }
            }
            
            yield return null;
        }
    }
}