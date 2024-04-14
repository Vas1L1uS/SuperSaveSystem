using System;
using System.Collections.Generic;
using AAAProject.Scripts.Extensions.Interfaces;

namespace AAAProject.Scripts.Extensions
{
    /// <summary>
    /// Слушатель событий с автопоотпиской, если объект уничтожен
    /// </summary>
    /// <typeparam name="T">Делегат</typeparam>
    public class DelegateListener<T> where T : Delegate
    {
        public List<T> Listeners { get; private set; }

        public DelegateListener(List<T> listeners = null)
        {
            if (listeners != null)
            {
                Listeners = listeners;
            }
            else
            {
                Listeners = new List<T>();
            }
        }

        /// <summary>
        /// Подписаться. Автоотписка, если подписавшийся уничтожен
        /// </summary>
        /// <param name="listener">Делегат</param>
        public void Subscribe(T listener)
        {
            Listeners.Add(listener);

            if (listener.Target is INotifyDestroy obj)
            {
                obj.BeingDestroyed += () =>
                {
                    if (Listeners.Contains(listener))
                    {
                        Unsubscribe(listener);
                    }
                };
            }
        }
        
        public void Unsubscribe(T listener)
        {
            Listeners.Remove(listener);
        }
        
        public void UnsubscribeAll()
        {
            for (int i = 0; i < Listeners.Count; i++)
            {
                Listeners.Remove(Listeners[i]);
            }
        }
        
        public object[] InvokeAll(params object[] parameters)
        {
            object[] results = new object[Listeners.Count];

            for (var i = 0; i < Listeners.Count; i++)
            {
                var listener = Listeners[i];
                results[i] = listener.DynamicInvoke(parameters);
            }

            return results;
        }
    }
}