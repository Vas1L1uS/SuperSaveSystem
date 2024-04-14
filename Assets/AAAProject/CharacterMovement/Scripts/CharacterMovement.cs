using System;
using UnityEngine;
using Camera = AAAProject.Scripts.Entities.Camera;

namespace AAAProject.CharacterMovement.Scripts
{
    public class CharacterMovement : MonoBehaviour
    {
        public Camera Camera { get; set; }
        
        [SerializeField]private CharacterController _characterController;
        [SerializeField]private AnimMoveController  _animMoveController;
        [Min(0)]
        [SerializeField] private float _speed = 3;

        private void Update()
        {
            if (Camera == null) return;
            
            Move();
        }

        private void Move()
        {
            var input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            
            Vector3 direction = Vector3.zero;
            direction = new Vector3(input.x, 0, input.y);
            var rotation = this.transform.rotation;
            rotation = Quaternion.Euler(rotation.x, (float)GetRotationAngleY(Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0) * direction), rotation.z);
            this.transform.rotation = rotation;
            _animMoveController.SetSpeed(_speed * input.magnitude);
            _characterController.SimpleMove(Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0) * direction * _speed);
        }

        private double GetRotationAngleY(Vector3 directionVector)
        {
            if (directionVector.z == 0)
            {
                if (directionVector.x > 0)
                {
                    return 90;
                }
                else if (directionVector.x < 0)
                {
                    return -90;
                }
                else
                {
                    return 0;
                }
            }
            else if (directionVector.x == 0)
            {
                if (directionVector.z > 0)
                {
                    return 0;
                }
                else
                {
                    return 180;
                }
            }

            double tang = directionVector.x / directionVector.z;

            double result = Math.Atan(tang) * 180 / Mathf.PI;

            if (directionVector.z < 0)
            {
                result -= 180;
            }

            return result;
        }
    }
}