using System;
using AAA_NewSaveSystem.Scripts.SaveSystem.Core;
using UnityEngine;

namespace AAA_NewSaveSystem.Scripts.Demo.Player
{
    public class SnakeController : MonoBehaviour
    {
        public event Action<int> ScoreChanged;
        public event Action Died;

        public int CurrentScore => _currentScore;
        
        [SerializeField] private SnakeMovement _snakeMovement;
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private Picker _picker;
        [SerializeField] private CrashTrigger _crashTrigger;
        [Space] 
        [SerializeField] private int _currentScore;

        private void Awake()
        {
            SaveManager.Loaded += Init;
        }

        private void Init(bool loaded)
        {
            _snakeMovement.Init();
            SubscribeOnComponents();
        }

        private void SetDirectionForSnakeMovement(PlayerInput.InputDirection direction)
        {
            switch (direction)
            {
                case PlayerInput.InputDirection.Up:
                    _snakeMovement.SetDirection(Vector3Int.forward);
                    break;
                case PlayerInput.InputDirection.Left:
                    _snakeMovement.SetDirection(Vector3Int.left);
                    break;
                case PlayerInput.InputDirection.Down:
                    _snakeMovement.SetDirection(Vector3Int.back);
                    break;
                case PlayerInput.InputDirection.Right:
                    _snakeMovement.SetDirection(Vector3Int.right);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        private void Pick(PickableItem item)
        {
            Destroy(item.gameObject);
            _currentScore += 1;
            _snakeMovement.AddBodyElement();
            ScoreChanged?.Invoke(_currentScore);
        }

        private void SubscribeOnComponents()
        {
            _playerInput.MovementPressed += SetDirectionForSnakeMovement;
            _picker.Picked += Pick;
            _crashTrigger.Crashed += Dead;
        }
        
        private void UnsubscribeOnComponents()
        {
            if (_playerInput != null) _playerInput.MovementPressed -= SetDirectionForSnakeMovement;
            if (_picker != null) _picker.Picked -= Pick;
            if (_crashTrigger != null) _crashTrigger.Crashed -= Dead;
        }

        private void Dead()
        {
            Died?.Invoke();
        }

        private void OnDestroy()
        {
            SaveManager.Loaded -= Init;
            UnsubscribeOnComponents();
        }
    }
}