using System;
using UnityEngine;

namespace AAA_NewSaveSystem.Scripts.Demo.Player
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