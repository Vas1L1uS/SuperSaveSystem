using AAA_NewSaveSystem.Scripts.Demo.Player;
using AAA_NewSaveSystem.Scripts.SaveSystem.Core;
using UnityEngine;

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
            if (loaded)
            {
                
            }
            
        }
    }
}