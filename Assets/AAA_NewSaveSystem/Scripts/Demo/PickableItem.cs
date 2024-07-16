using UnityEngine;

namespace AAA_NewSaveSystem.Scripts.Demo
{
    public class PickableItem : MonoBehaviour
    {
        public ItemTypes ItemType => _itemType;

        [SerializeField] private ItemTypes _itemType;

        public enum ItemTypes
        {
            Food
        }
    }
}