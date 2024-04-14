using UnityEngine;

namespace AAAProject.Scripts.Extensions.CoroutineScripts.CoroutineManagerService
{
    public class CoroutineManager : MonoBehaviour
    {
        private static CoroutineManager _instance;
        
        public static CoroutineManager GetInstance()
        {
            if (_instance == null)
            {
                CoroutineManagerInitializer.GetInstance();
            }
            
            return _instance;
        }
        
        private CoroutineManager() {}

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance == this)
            {
                Destroy(this.gameObject);
            }
        }
    }
}