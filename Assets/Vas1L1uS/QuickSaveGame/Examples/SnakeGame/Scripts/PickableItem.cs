using UnityEngine;

namespace Vas1L1uS.QuickSaveGame.Examples.SnakeGame.Scripts
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