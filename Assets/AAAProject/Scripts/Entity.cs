using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AAAProject.Scripts
{
    [DisallowMultipleComponent]
    public class Entity : MonoBehaviour
    {
        public class EntityCtx
        {
            public List<string> ChildKeys;
            public bool         Enabled;
            public Vector3      LocalPosition;
            public Quaternion   LocalRotation;
            public Vector3      LocalScale;
        }

        public string Key { get; protected set; }
        public string PrefabPath => _prefabPath;

        [SerializeField]private string _prefabPath;
        
        protected EntityCtx        _ctx;
        protected EntityCollection _entityCollection;

        public void Init(string key, EntityCollection entityCollection)
        {
            _entityCollection = entityCollection;
            NewEntityCtx();
            Key = key;
            Load();
            Root.AddEntity(this);
            SetCtx();
        }

        public void Save()
        {
            PlayerPrefs.DeleteKey(Key);

            if (_ctx != null && _ctx.ChildKeys != null && _ctx.ChildKeys.Count > 0)
            {
                for (int i = 0; i < _ctx.ChildKeys.Count; i++)
                {
                    PlayerPrefs.DeleteKey(_ctx.ChildKeys[i]);
                }
            }

            NewEntityCtx();
            _ctx.ChildKeys = new List<string>();

            for (var i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent(out Entity child))
                {
                    child.Save();
                    _ctx.ChildKeys.Add(child.Key);
                    Debug.Log($"Saved: {child.Key}");
                }
            }

            _ctx.Enabled = this.gameObject.activeSelf;
            _ctx.LocalPosition = transform.localPosition;
            _ctx.LocalRotation = transform.localRotation;
            _ctx.LocalScale = transform.localScale;
            SaveAdditionalParams();
            SaveManager.Save(_ctx, Key);
        }

        protected virtual void SaveAdditionalParams()
        {
            
        }
        
        protected virtual void SetAdditionalParams()
        {
            
        }
        
        protected virtual void NewEntityCtx()
        {
            _ctx = new EntityCtx();
        }
        
        protected void InitKey()
        {
            if (Key == null || Key == "")
            {
                Key = PrefabPath + "%" + GetType() + "%" + gameObject.name + "%" + Guid.NewGuid();
            }
        }

        protected virtual void Awake()
        {
            NewEntityCtx();
            InitKey();
        }
        
        private void Load()
        {
            _ctx.ChildKeys = new List<string>();
            SaveManager.Load(ref _ctx, Key, out bool hasKey);

            if (!hasKey)
            {
                _ctx.LocalScale = Vector3.one;
                _ctx.Enabled = true;
            }
        }

        private void SetCtx()
        {
            transform.localPosition = _ctx.LocalPosition;
            transform.localRotation = _ctx.LocalRotation;
            transform.localScale = _ctx.LocalScale;

            if (_ctx.ChildKeys != null && _ctx.ChildKeys.Count != 0)
            {
                for (var i = 0; i < _ctx.ChildKeys.Count; i++)
                {
                    var words = _ctx.ChildKeys[i].Split('%');
                    var newObj = Instantiate(_entityCollection.GetEntityByPath(words[0]), transform);
                    newObj.name = words[2];
                    newObj.Init(_ctx.ChildKeys[i], _entityCollection);
                }
            }

            SetAdditionalParams();
            
            if (!_ctx.Enabled) this.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            PlayerPrefs.DeleteKey(Key);
        }
    }
}
