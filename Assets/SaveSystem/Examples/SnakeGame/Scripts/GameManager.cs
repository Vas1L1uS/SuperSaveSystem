using SaveSystem.Core.Scripts.Core;
using SaveSystem.Examples.SnakeGame.Scripts.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SaveSystem.Examples.SnakeGame.Scripts
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private SnakeController _snakeController;

        private void Awake()
        {
            SaveManager.Loaded += Init;
        }

        private void Init(bool loaded)
        {
            _snakeController.Died += RestartGame;
        }

        private void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void OnDestroy()
        {
            SaveManager.Loaded -= Init;
            if (_snakeController != null) _snakeController.Died -= RestartGame;
        }
    }
}