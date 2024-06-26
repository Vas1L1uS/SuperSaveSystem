using System;
using System.Collections.Generic;
using System.Linq;
using AAAProject.Scripts.Extensions;
using UnityEngine;

namespace AAAProject.Scripts
{
    [DisallowMultipleComponent]
    public class Entity : MBehaviour
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

        protected EntityCtx _ctx;

        public void Init(string key = null)
        {
            NewEntityCtx();
            if (key != null || key != "") Key = key;
            Load();
            Root.AddEntity(this);
            SetCtx();
        }

        public void Save()
        {
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

        protected virtual void NoHasKeyOnLoad()
        {
            _ctx.LocalScale = Vector3.one;
            _ctx.Enabled = true;
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
                NoHasKeyOnLoad();
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
                    var newObj = Instantiate(Root.GetEntityPrefabByPrefabPath(words[0]), transform);
                    newObj.name = words[2];
                    newObj.Init(_ctx.ChildKeys[i]);
                }
            }

            SetAdditionalParams();
            
            if (!_ctx.Enabled) this.gameObject.SetActive(false);
        }
    }
}
