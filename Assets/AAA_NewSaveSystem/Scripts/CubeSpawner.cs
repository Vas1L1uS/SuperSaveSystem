using UnityEngine;

namespace AAA_NewSaveSystem.Scripts
{
    public class CubeSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _cubePrefab;
        
        [ContextMenu("SpawnCube")]
        public void SpawnCube()
        {
            for (int i = 0; i < 100; i++)
            {
                //Instantiate(_cubePrefab, new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100)), Quaternion.identity, transform).GetComponent<CubeController>().Init(_cubePrefab);
            }
        }
    }
}