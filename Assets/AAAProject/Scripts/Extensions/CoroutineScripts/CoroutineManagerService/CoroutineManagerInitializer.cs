using UnityEngine;

namespace AAAProject.Scripts.Extensions.CoroutineScripts.CoroutineManagerService
{
    public class CoroutineManagerInitializer : Object
    {
        private static CoroutineManagerInitializer _instance;

        public static CoroutineManagerInitializer GetInstance()
        {
            if (_instance == null)
            {
                _instance = new CoroutineManagerInitializer();
            }

            return _instance;
        }

        private CoroutineManagerInitializer()
        {
            GameObject obj = new GameObject("CoroutineManager");
            obj.AddComponent<CoroutineManager>();
            DontDestroyOnLoad(CoroutineManager.GetInstance());
        }
    }
}