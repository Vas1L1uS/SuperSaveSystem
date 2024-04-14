using System;
using System.Collections.Generic;
using AAAProject.Scripts.Extensions;
using UnityEngine;

namespace AAAProject.Scripts.Components
{
    public class CollisionChecker : MonoBehaviour
    {
        public event Action<GameObject> TriggerEntered;
        public event Action<GameObject> Trigger2DEntered;
        public event Action<GameObject> CollisionEntered;
        public event Action<GameObject> Collision2DEntered;

        public event Action<GameObject> TriggerExited;
        public event Action<GameObject> Trigger2DExited;
        public event Action<GameObject> CollisionExited;
        public event Action<GameObject> Collision2DExited;

        [SerializeField] private DetectType _detectType;

        [Header("Detect settings")]
        [SerializeField] private string _tag;
        [SerializeField] private LayerMask _layer;
        [SerializeField] private List<Collider> _colliders;
        [SerializeField] private List<Collider2D> _colliders2D;
        [SerializeField] private string _componentName;

        private void OnTriggerEnter(Collider other)
        {
            CheckAndInvoke(other.gameObject, TriggerEntered);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            CheckAndInvoke(collision.gameObject, Trigger2DEntered);
        }

        private void OnCollisionEnter(Collision collision)
        {
            CheckAndInvoke(collision.gameObject, CollisionEntered);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            CheckAndInvoke(other.gameObject, Collision2DEntered);
        }

        private void OnTriggerExit(Collider other)
        {
            CheckAndInvoke(other.gameObject, TriggerExited);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            CheckAndInvoke(collision.gameObject, Trigger2DExited);
        }

        private void OnCollisionExit(Collision collision)
        {
            CheckAndInvoke(collision.gameObject, CollisionExited);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            CheckAndInvoke(collision.gameObject, Collision2DExited);
        }

        private void CheckAndInvoke(GameObject obj, Action<GameObject> action)
        {
            switch (_detectType)
            {
                case DetectType.All:
                    
                    if (CheckTag(obj, _tag) || CheckLayer(obj, _layer))
                    {
                        action?.Invoke(obj);
                        break;
                    }
                    else if (obj.GetComponent(Type.GetType(_componentName)) != null)
                    {
                        action?.Invoke(obj);
                        break;
                    }
                    else
                    {
                        foreach (var item in _colliders)
                        {
                            if (CheckCollider(obj, item))
                            {
                                action?.Invoke(obj);
                                break;
                            }
                        }
                    }
                    break;  

                case DetectType.ByTag:

                    if (obj.CompareTag(_tag))
                    {
                        action?.Invoke(obj);
                    }

                    break;

                case DetectType.ByLayer:

                    if (AdditionalMethods.CheckBitInNumber(obj.layer, _layer))
                    {
                        action?.Invoke(obj);
                    }

                    break;

                case DetectType.ByCollider:

                    foreach (var item in _colliders)
                    {
                        if (item == obj.GetComponent<Collider>())
                        {
                            action?.Invoke(obj);
                            break;
                        }
                    }

                    foreach (var item in _colliders2D)
                    {
                        if (item == obj.GetComponent<Collider2D>())
                        {
                            action?.Invoke(obj);
                            break;
                        }
                    }
                    
                    break;
                case DetectType.ByComponentName:
                    var component = obj.GetComponent(_componentName);
                    
                    if (component != null)
                    {
                        action?.Invoke(obj);
                    }
                    break;
            }
        }

        private bool CheckTag(GameObject obj, string tag)
        {
            if (obj.CompareTag(tag))
            {
                return true;
            }

            return false;
        }

        private bool CheckLayer(GameObject obj, LayerMask layer)
        {
            if (AdditionalMethods.CheckBitInNumber(obj.layer, layer))
            {
                return true;
            }

            return false;
        }

        private bool CheckCollider(GameObject obj, Collider collider)
        {
            if (collider == gameObject.GetComponent<Collider>() || collider == obj.GetComponent<Collider2D>())
            {
                return true;
            }

            return false;
        }

        private enum DetectType
        {
            All,
            ByTag,
            ByLayer,
            ByCollider,
            ByComponentName
        }
    }
}