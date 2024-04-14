using UnityEngine;

namespace AAAProject.Scripts.Entities
{
    [SelectionBase]
    public class Player : Entity
    {
        public class PlayerCtx : EntityCtx
        {
            public string       CameraKey;
        }

        [SerializeField]private Camera                                      _camera;
        [SerializeField]private CharacterMovement.Scripts.CharacterMovement _characterMovement;

        protected override void NewEntityCtx()
        {
            _ctx = new PlayerCtx();
        }

        protected override void SaveAdditionalParams()
        {
            ((PlayerCtx)_ctx).CameraKey = _camera.Key;
        }

        private void Start()
        {
            Entity cameraEntity = Root.GetEntityByKey(((PlayerCtx)_ctx).CameraKey);

            if (cameraEntity != null)
            {
                _camera = cameraEntity as Camera;
            }

            _characterMovement.Camera = _camera;
        }
    }
}
