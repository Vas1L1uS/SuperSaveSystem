using System;
using UnityEngine;

namespace AAA_NewSaveSystem.Scripts.Demo.Player
{
    public class CrashTrigger : MonoBehaviour
    {
        public event Action Crashed;
        
        [SerializeField] private LayerMask _crashLayer;
        
        private void OnTriggerEnter(Collider other)
        {

            if (CheckBitInNumber(other.gameObject.layer, _crashLayer))
            {
                Crashed?.Invoke();
            }
        }
        
        private bool CheckBitInNumber(int bit, int number)
        {
            bit = (int)Mathf.Pow(2, bit);

            for (int i = 31; i >= 0; i--)
            {
                if ((int)Mathf.Pow(2, i) == bit)
                {
                    if (number - Mathf.Pow(2, i) >= 0)
                    {
                        return true;
                    }
                    return false;
                }

                if (number - Mathf.Pow(2, i) >= 0)
                {
                    number -= (int)Mathf.Pow(2, i);
                }
            }

            return false;
        }
    }
}
