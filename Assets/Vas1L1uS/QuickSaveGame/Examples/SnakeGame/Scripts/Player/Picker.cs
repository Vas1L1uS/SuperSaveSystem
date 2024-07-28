using System;
using UnityEngine;

namespace Vas1L1uS.QuickSaveGame.Examples.SnakeGame.Scripts.Player
{
    public class Picker : MonoBehaviour
    {
        public event Action<PickableItem> Picked;
        
        [SerializeField] private LayerMask _triggerLayer;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out PickableItem item))
            {
                Picked?.Invoke(item);
            }
        }

        private void OnDestroy()
        {
            Picked = null;
        }
    }
}