using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AAAProject.Scripts.Entities
{
    public class Camera : Entity
    {
        public class CameraCtx : EntityCtx
        {
            public string       TargetKey;
        }
        
        [SerializeField]private FollowController   _followController;
        [SerializeField]private Entity             _target;

        protected override void SaveAdditionalParams()
        {
            ((CameraCtx)_ctx).TargetKey = _target.Key;
        }

        protected override void NewEntityCtx()
        {
            _ctx = new CameraCtx();
        }

        private void Start()
        {
            Entity entity = Root.GetEntityByKey(((CameraCtx)_ctx).TargetKey);

            if (entity != null)
            {
                _target = entity;
            }

            if (_target != null)
            {
                _followController.Target = _target.transform;
            }
        }
    }
}