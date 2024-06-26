using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AAAProject.Scripts.Components
{
    public class Spawner : MonoBehaviour
    {
        public virtual event Action<GameObject> NewObject;

        public Transform SpawnPoint => _spawnPoints[0];

        [SerializeField]protected GameObject[] _objectsPrefabs;
        [SerializeField]protected Transform    _parentForNewObjects;

        [Space]
        [SerializeField]protected Transform[] _spawnPoints;
        [Min(0)][SerializeField]protected Vector3 _spawnZoneSize;

        [Space]
        [SerializeField]protected bool _severalObjects;
        [SerializeField]protected bool         _severalSpawnPoints;
        [SerializeField]protected RotationType _rotationMode;
        
        [ContextMenu("Spawn")]
        public virtual void Spawn()
        {
            GameObject prefab;

            if (_severalObjects)
            {
                prefab = GetRandomObject();
            }
            else
            {
                prefab = _objectsPrefabs[0];
            }

            Transform spawnPoint;

            if (_severalSpawnPoints)
            {
                spawnPoint = GetRandomSpawnPoint();
            }
            else
            {
                spawnPoint = _spawnPoints[0];
            }

            Vector3 position = spawnPoint.position + GetRandomPosition();
            Quaternion rotation = GetRandomRotation();
            NewObject?.Invoke(Instantiate(prefab, position, rotation, _parentForNewObjects));
        }

        protected GameObject GetRandomObject()
        {
            return _objectsPrefabs[Random.Range(0, _objectsPrefabs.Length)];
        }

        protected Transform GetRandomSpawnPoint()
        {
            return _spawnPoints[Random.Range(0, _spawnPoints.Length)];
        }

        protected Vector3 GetRandomPosition()
        {
            float x = Random.Range(-_spawnZoneSize.x, _spawnZoneSize.x);
            float y = Random.Range(-_spawnZoneSize.y, _spawnZoneSize.y);
            float z = Random.Range(-_spawnZoneSize.z, _spawnZoneSize.z);

            return new Vector3(x, y, z);
        }

        protected Quaternion GetRandomRotation()
        {
            switch (_rotationMode)
            {
                case RotationType.None:
                    return Quaternion.identity;
                case RotationType.SpawnPoint:
                    return SpawnPoint.rotation;
                case RotationType.RandomX:
                    return Quaternion.Euler(Random.Range(0, 360f), 0, 0);
                case RotationType.RandomY:
                    return Quaternion.Euler(0, Random.Range(0, 360f), 0);
                case RotationType.RandomZ:
                    return Quaternion.Euler(0, 0, Random.Range(0, 360f));
                case RotationType.RandomXY:
                    return Quaternion.Euler(Random.Range(0, 360f), Random.Range(0, 360f), 0);
                case RotationType.RandomXZ:
                    return Quaternion.Euler(Random.Range(0, 360f), 0, Random.Range(0, 360f));
                case RotationType.RandomYZ:
                    return Quaternion.Euler(0, Random.Range(0, 360f), Random.Range(0, 360f));
                case RotationType.RandomXYZ:
                    return Quaternion.Euler(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f));
                default:
                    return Quaternion.identity;
            }
        }

        protected enum RotationType
        {
            None,
            SpawnPoint,
            RandomX,
            RandomY,
            RandomZ,
            RandomXY,
            RandomXZ,
            RandomYZ,
            RandomXYZ
        }
    }
}
