using UnityEngine;
using UnityEngine.UI;
using Vas1L1uS.QuickSaveGame.Core.Scripts.Core;

namespace Vas1L1uS.QuickSaveGame.Examples.SnakeGame.Scripts.Player
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private SnakeController _snakeController;
        [SerializeField] private Text _scoreValueText;

        private void Awake()
        {
            SaveManager.LoadFinished += Init;
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
            SaveManager.LoadFinished -= Init;
            UnsubscribeFromComponents();
        }
    }
}