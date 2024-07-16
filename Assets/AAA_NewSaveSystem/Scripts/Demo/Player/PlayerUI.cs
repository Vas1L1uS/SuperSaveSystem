using AAA_NewSaveSystem.Scripts.SaveSystem;
using AAA_NewSaveSystem.Scripts.SaveSystem.Core;
using UnityEngine;
using UnityEngine.UI;

namespace AAA_NewSaveSystem.Scripts.Demo.Player
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private SnakeController _snakeController;
        [SerializeField] private Text _scoreValueText;

        private void Awake()
        {
            RootSaver.Loaded += Init;
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
            _snakeController.ScoreChanged -= UpdateScore;
        }
        
        private void UpdateScore(int value)
        {
            _scoreValueText.text = value.ToString();
        }

        private void OnDestroy()
        {
            RootSaver.Loaded -= Init;
            UnsubscribeFromComponents();
        }
    }
}