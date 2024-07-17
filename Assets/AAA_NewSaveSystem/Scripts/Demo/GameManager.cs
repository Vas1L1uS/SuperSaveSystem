using AAA_NewSaveSystem.Scripts.Demo.Player;
using AAA_NewSaveSystem.Scripts.SaveSystem.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AAA_NewSaveSystem.Scripts.Demo
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private SnakeController _snakeController;

        private void Awake()
        {
            RootSaver.Loaded += Init;
        }

        private void Init(bool loaded)
        {
            Debug.Log("Init");
            _snakeController.Died += RestartGame;
        }

        private void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void OnDestroy()
        {
            RootSaver.Loaded -= Init;
            _snakeController.Died -= RestartGame;
        }
    }
}