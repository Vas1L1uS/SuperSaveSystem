using System;
using UnityEngine;

namespace SaveSystem.Examples.SnakeGame.Scripts.Player
{
    public class PlayerInput : MonoBehaviour
    {
        public event Action<InputDirection> MovementPressed;
        
        private void Update()
        {
            if (Input.GetKeyDown("w"))
            {
                MovementPressed?.Invoke(InputDirection.Up);
            }
            
            if (Input.GetKeyDown("a"))
            {
                MovementPressed?.Invoke(InputDirection.Left);
            }
            
            if (Input.GetKeyDown("s"))
            {
                MovementPressed?.Invoke(InputDirection.Down);
            }
            
            if (Input.GetKeyDown("d"))
            {
                MovementPressed?.Invoke(InputDirection.Right);
            }
        }

        public enum InputDirection
        {
            Up,
            Left,
            Down,
            Right
        }
    }
}