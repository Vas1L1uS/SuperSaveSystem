using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Vas1L1uS.QuickSaveGame.Examples.SnakeGame.Scripts.Player
{
    public class SnakeMovement : MonoBehaviour
    {
        [SerializeField] private GameObject _bodyElementPrefab;  
        [SerializeField] private List<Transform> _bodyElementsTransforms;
        [SerializeField] private float _speed = 3f;
        [SerializeField] private Vector3 _currentDirection = new Vector3(0, 0, 1);
        [SerializeField] private Transform _bodyElementsParent;
        [Space] 
        [SerializeField] private List<BodyElement> _bodyElements;
        
        private bool _inited;

        public void Init()
        {
            if (_bodyElements == null || _bodyElements.Count == 0)
            {
                _bodyElements = new();
            
                for (int i = 1; i < _bodyElementsTransforms.Count; i++)
                {
                    _bodyElements.Add(new BodyElement{Transform = _bodyElementsTransforms[i]});
                }
            
                foreach (BodyElement bodyElement in _bodyElements)
                {
                    bodyElement.CurrentDirection = _currentDirection;
                }
            }

            _inited = true;
        }

        public void SetDirection(Vector3Int direction)
        {
            if (direction == _currentDirection) return;
            
            _currentDirection = direction;

            for (int i = 0; i < _bodyElements.Count; i++)
            {
                BodyElement bodyElement = _bodyElements[i];
                bodyElement.FutureDirections.Add(_currentDirection);
                bodyElement.FutureChangeDirectionPositions.Add(_bodyElementsTransforms[0].position);

                float deltaX = bodyElement.FutureChangeDirectionPositions[0].x - bodyElement.Transform.position.x;

                if (deltaX != 0)
                {
                    bodyElement.CurrentDirection =
                        deltaX > 0 ? Vector3.right : Vector3.left;
                }
                else
                {
                    float deltaZ = bodyElement.FutureChangeDirectionPositions[0].z - bodyElement.Transform.position.z;
                    bodyElement.CurrentDirection =
                        deltaZ > 0 ? Vector3.forward : Vector3.back;
                }
            }

            Rotate(_bodyElementsTransforms[0].transform, direction);
        }
        
        public void SetSpeed(float newSpeed)
        {
            _speed = newSpeed;
        }

        public void AddBodyElement()
        {
            Vector3 offset = _currentDirection * -0.5f;

            if (_bodyElements.Count > 0)
            {
                offset = _bodyElements[^1].CurrentDirection * -0.5f;
            }

            GameObject newBodyElementObj = Instantiate(_bodyElementPrefab, _bodyElementsTransforms[^1].position + offset, _bodyElementsTransforms[^1].rotation, _bodyElementsParent);
            _bodyElementsTransforms.Add(newBodyElementObj.transform);
            BodyElement newBodyElement;

            if (_bodyElements.Count > 0)
            {
                newBodyElement = new BodyElement
                {
                    Transform = _bodyElementsTransforms[^1],
                    CurrentDirection = _bodyElements[^1].CurrentDirection,
                    FutureDirections = _bodyElements[^1].FutureDirections.ToList(),
                    FutureChangeDirectionPositions = _bodyElements[^1].FutureChangeDirectionPositions.ToList(),
                };
            }
            else
            {
                newBodyElement = new BodyElement
                {
                    Transform = _bodyElementsTransforms[^1],
                    CurrentDirection = _currentDirection,
                };
            }
            
            _bodyElements.Add(newBodyElement);
        }

        private void Update()
        {
            if (_inited == false) return;

            Move();
        }

        private void Move()
        {
            _bodyElementsTransforms[0].Translate(_currentDirection * (_speed * Time.deltaTime), Space.World);

            for (int i = 0; i < _bodyElements.Count; i++)
            {
                _bodyElements[i].MoveBody(_speed);
                CheckFutureDirection(_bodyElements[i]);
            }
        }

        private void CheckFutureDirection(BodyElement bodyElement)
        {
            if (bodyElement.FutureChangeDirectionPositions.Count < 1) return;

            float fixDelta = 0;

            if (bodyElement.CurrentDirection == Vector3.forward)
            {
                if (bodyElement.Transform.position.z < bodyElement.FutureChangeDirectionPositions[0].z) return;
                fixDelta = bodyElement.Transform.position.z - bodyElement.FutureChangeDirectionPositions[0].z;
            }
            else if (bodyElement.CurrentDirection == Vector3.left)
            {
                if (bodyElement.Transform.position.x > bodyElement.FutureChangeDirectionPositions[0].x) return;
                fixDelta = bodyElement.FutureChangeDirectionPositions[0].x - bodyElement.Transform.position.x;
            }
            else if (bodyElement.CurrentDirection == Vector3.back)
            {
                if (bodyElement.Transform.position.z > bodyElement.FutureChangeDirectionPositions[0].z) return;
                fixDelta = bodyElement.FutureChangeDirectionPositions[0].z - bodyElement.Transform.position.z;
            }
            else if (bodyElement.CurrentDirection == Vector3.right)
            {
                if (bodyElement.Transform.position.x < bodyElement.FutureChangeDirectionPositions[0].x) return;
                fixDelta = bodyElement.Transform.position.x - bodyElement.FutureChangeDirectionPositions[0].x;
            }

            Rotate(bodyElement.Transform, bodyElement.FutureDirections[0]);
            bodyElement.CurrentDirection = bodyElement.FutureDirections[0];

            if (bodyElement.CurrentDirection == Vector3.right)
            {
                Vector3 newPosition = bodyElement.Transform.position;
                newPosition = new Vector3(newPosition.x + fixDelta, newPosition.y, bodyElement.FutureChangeDirectionPositions[0].z);
                bodyElement.Transform.position = newPosition;
            }
            else if (bodyElement.CurrentDirection == Vector3.left)
            {
                Vector3 newPosition = bodyElement.Transform.position;
                newPosition = new Vector3(newPosition.x - fixDelta, newPosition.y, bodyElement.FutureChangeDirectionPositions[0].z);
                bodyElement.Transform.position = newPosition;
            }
            else if (bodyElement.CurrentDirection == Vector3.forward)
            {
                Vector3 newPosition = bodyElement.Transform.position;
                newPosition = new Vector3(bodyElement.FutureChangeDirectionPositions[0].x, newPosition.y, newPosition.z + fixDelta);
                bodyElement.Transform.position = newPosition;
            }
            else if (bodyElement.CurrentDirection == Vector3.back)
            {
                Vector3 newPosition = bodyElement.Transform.position;
                newPosition = new Vector3(bodyElement.FutureChangeDirectionPositions[0].x, newPosition.y, newPosition.z - fixDelta);
                bodyElement.Transform.position = newPosition;
            }
            
            bodyElement.FutureChangeDirectionPositions.RemoveAt(0);
            bodyElement.FutureDirections.RemoveAt(0);
        }

        private void Rotate(Transform t, Vector3 direction)
        {
            t.rotation = Quaternion.LookRotation(direction);
        }
    }
    
    [Serializable]
    public class BodyElement
    {
        [SerializeField] public Transform Transform;
        [SerializeField] public List<Vector3> FutureChangeDirectionPositions = new();
        [SerializeField] public List<Vector3> FutureDirections = new();
        [SerializeField] public Vector3 CurrentDirection;

        public void MoveBody(float speed)
        {
            Transform.Translate(CurrentDirection * (speed * Time.deltaTime), Space.World);
        }
    }
}
