using SaveSystem.Core.Scripts.Core;
using UnityEngine;
using UnityEngine.UI;

namespace SaveSystem.Examples.SnakeGame.Scripts.Player
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private SnakeController _snakeController;
        [SerializeField] private Text _scoreValueText;

        private void Awake()
        {
            SaveManager.Loaded += Init;
        }

        private void Init(bool value)
        {
            SubscribeOnComponents();
        }

        private void SubscribeOnComponents()
        {
            UpdateScore(_snakeController.CurrentScore);
            _snakeController.ScoreChanged += UpdateScore;
        }
        
        private void UnsubscribeFromComponents()
        {
            if (_snakeController != null) _snakeController.ScoreChanged -= UpdateScore;
        }
        
        private void UpdateScore(int value)
        {
            _scoreValueText.text = value.ToString();
        }

        private void OnDestroy()
        {
            SaveManager.Loaded -= Init;
            UnsubscribeFromComponents();
        }
    }
}