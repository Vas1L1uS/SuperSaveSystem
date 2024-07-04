using System.Collections.Generic;
using UnityEngine;

namespace AAA_NewSaveSystem.Scripts.SaveSystem
{
    [CreateAssetMenu(fileName = "AssetCollection", menuName = "AssetCollection", order = 0)]
    public class AssetCollection : ScriptableObject
    {
        [SerializeField] private List<Object> _objects;

        public Object GetObjectByIndex(int index)
        {
            return _objects[index];
        }
        
        public int GetIndexByObject(Object obj)
        {
            for (int i = 0; i < _objects.Count; i++)
            {
                if (_objects[i] == obj)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}