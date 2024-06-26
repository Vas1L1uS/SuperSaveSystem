using System;
using AAAProject.Scripts.Extensions.Interfaces;
using UnityEngine;

namespace AAAProject.Scripts.Extensions
{
    public class MBehaviour : MonoBehaviour, INotifyDestroy
    {
        public event Action BeingDestroyed
        {
            add => _beingDestroyedListener.Subscribe(value);
            remove => _beingDestroyedListener.Unsubscribe(value);
        }
        
        protected readonly DelegateListener<Action> _beingDestroyedListener = new();

        protected virtual void OnDestroy()
        {
            _beingDestroyedListener.InvokeAll();
        }
    }
}