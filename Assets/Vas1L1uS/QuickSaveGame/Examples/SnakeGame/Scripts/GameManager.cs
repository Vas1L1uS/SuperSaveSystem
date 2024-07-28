using UnityEngine;
using UnityEngine.SceneManagement;
using Vas1L1uS.QuickSaveGame.Core.Scripts.Core;
using Vas1L1uS.QuickSaveGame.Examples.SnakeGame.Scripts.Player;

namespace Vas1L1uS.QuickSaveGame.Examples.SnakeGame.Scripts
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private SnakeController _snakeController;

        private void Awake()
        {
            SaveManager.LoadFinished += Init;
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
            SaveManager.LoadFinished -= Init;
            if (_snakeController != null) _snakeController.Died -= RestartGame;
        }
    }
}