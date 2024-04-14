using System;
using AAAProject.Scripts.Extensions.Interfaces;
using UnityEngine;

namespace AAAProject.Scripts.Extensions
{
    public class MBehaviour : MonoBehaviour, INotifyDestroy
    {
        public event Action Initialized
        {
            add => _initializedListener.Subscribe(value);
            remove => _initializedListener.Unsubscribe(value);
        }
        
        public event Action Restarted
        {
            add => _restartedListener.Subscribe(value);
            remove => _restartedListener.Unsubscribe(value);
        }
        
        public event Action Played
        {
            add => _playedListener.Subscribe(value);
            remove => _playedListener.Unsubscribe(value);
        }
        
        public event Action Stopped
        {
            add => _stoppedListener.Subscribe(value);
            remove => _stoppedListener.Unsubscribe(value);
        }
        
        public event Action Paused
        {
            add => _pausedListener.Subscribe(value);
            remove => _pausedListener.Unsubscribe(value);
        }
        
        public event Action Resumed
        {
            add => _resumedListener.Subscribe(value);
            remove => _resumedListener.Unsubscribe(value);
        }

        public event Action BeingDestroyed
        {
            add => _beingDestroyedListener.Subscribe(value);
            remove => _beingDestroyedListener.Unsubscribe(value);
        }
        
        public virtual bool IsPlayed { get; protected set; }
        public virtual bool IsPaused { get; protected set; }
        public virtual bool IsInitialized { get; protected set; }
        public virtual Vector3 StartPosition { get; protected set; }
        public virtual Quaternion StartRotation { get; protected set; }
        public virtual Vector3 StartLocalScale { get; protected set; }
        public virtual Transform StartParent { get; protected set; }

        protected readonly DelegateListener<Action> _initializedListener = new();
        protected readonly DelegateListener<Action> _restartedListener = new();
        protected readonly DelegateListener<Action> _playedListener = new();
        protected readonly DelegateListener<Action> _stoppedListener = new();
        protected readonly DelegateListener<Action> _pausedListener = new();
        protected readonly DelegateListener<Action> _resumedListener = new();
        protected readonly DelegateListener<Action> _beingDestroyedListener = new();

        protected virtual void Awake()
        {
            StartParent = this.transform.parent;
            StartPosition = this.transform.position;
            StartRotation = this.transform.rotation;
            StartLocalScale = this.transform.localScale;
        }

        /// <summary>
        /// Вызывается через GameStateController
        /// </summary>
        protected virtual void Initialize()
        {
            if (IsInitialized) return;
            
            IsInitialized = true;
            _initializedListener?.InvokeAll();
        }
        
        /// <summary>
        /// Вызывается через GameStateController
        /// Сбрасывает состояние на момент после появления Awake(), не влияет на паузу
        /// </summary>
        protected virtual void Restart()
        {
            this.transform.SetParent(StartParent);
            this.transform.SetLocalPositionAndRotation(StartPosition, StartRotation);
            this.transform.localScale = StartLocalScale;
            IsInitialized = false;
            IsPlayed = false;
        }
        
        /// <summary>
        /// Вызывается через GameStateController
        /// </summary>
        protected virtual void Play()
        {
            if (IsPlayed) return;
            
            IsPlayed = true;
            _playedListener.InvokeAll();
        }

        /// <summary>
        /// Вызывается через GameStateController
        /// </summary>
        protected virtual void Stop()
        {
            if (!IsPlayed) return;
            
            IsPlayed = false;
            _stoppedListener.InvokeAll();
        }
        
        /// <summary>
        /// Вызывается через PauseController
        /// </summary>
        protected virtual void Pause()
        {
            if (IsPaused) return;

            IsPaused = true;
            _pausedListener.InvokeAll();
        }

        /// <summary>
        /// Вызывается через PauseController
        /// </summary>
        protected virtual void Resume()
        {
            if (!IsPaused) return;

            IsPaused = false;
            _resumedListener.InvokeAll();
        }

        protected virtual void OnDestroy()
        {
            _beingDestroyedListener.InvokeAll();
        }
    }
}