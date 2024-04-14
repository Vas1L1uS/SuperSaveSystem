using System;
using System.Collections;
using AAAProject.Scripts.Extensions.CoroutineScripts.CoroutineManagerService;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AAAProject.Scripts.Extensions.CoroutineScripts
{
    public class CurveCoroutine
    {
        public event Action Finished
        {
            add => _finishedListener.Subscribe(value);
            remove => _finishedListener.Unsubscribe(value);
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

        public bool IsPaused => _isPaused;
        public float CurrentValue { get; private set; }
        public float PreviousValue { get; private set; }
        public float CurrentTime => _coroutineCurrentTime;

        private readonly CoroutineManager _coroutineManager;
        private Coroutine _coroutine;
        private float _coroutineCurrentTime;
        private bool _isPaused;
        private AnimationCurve _animationCurve;
        private Action _action;
        private Action<float> _actionFloat;
        private readonly DelegateListener<Action> _finishedListener = new();
        private readonly DelegateListener<Action> _pausedListener = new();
        private readonly DelegateListener<Action> _resumedListener = new();

        /// <summary>
        /// Создавать только в Awake()
        /// </summary>
        public CurveCoroutine()
        {
            _coroutineManager = CoroutineManager.GetInstance();
            SceneManager.sceneUnloaded += ctx => Stop();
            Finished += Restart;
        }
        
        /// <summary>
        /// Запускает корутину
        /// </summary>
        /// <param name="animationCurve">Анимационная кривая</param>
        /// <param name="action">Метод</param>
        public void Play(AnimationCurve animationCurve, Action action, float time = 1)
        {
            if (_coroutine != null) _coroutineManager.StopCoroutine(_coroutine);
            
            Restart();
            _animationCurve = animationCurve;
            _action = action;
            CurrentValue = animationCurve[0].time;
            PreviousValue = animationCurve[0].time;
            _coroutine = _coroutineManager.StartCoroutine(CurveRoutine(_animationCurve, _action, time));
        }

        /// <summary>
        /// Запускает корутину
        /// </summary>
        /// <param name="animationCurve">Анимационная кривая</param>
        /// <param name="actionFloat">Метод</param>
        public void Play(AnimationCurve animationCurve, Action<float> actionFloat, float time = 1)
        {
            if (_coroutine != null) Stop();
            
            Restart();
            _animationCurve = animationCurve;
            _actionFloat = actionFloat;
            _coroutine = _coroutineManager.StartCoroutine(CurveRoutine(_animationCurve, _actionFloat, time));
        }
        
        /// <summary>
        /// Приостонавливает корутину
        /// </summary>
        public void Pause()
        {
            if (_isPaused) return;
            
            _isPaused = true;
            _pausedListener.InvokeAll();
        }
        
        /// <summary>
        /// Продолжает воспроизводить корутину
        /// </summary>
        public void Resume()
        {
            if (!_isPaused) return;
            
            _isPaused = false;

            if (_action != null)
            {
                _coroutine = _coroutineManager.StartCoroutine(CurveRoutine(_animationCurve, _action));
            }
            else if (_actionFloat != null)
            {
                _coroutine = _coroutineManager.StartCoroutine(CurveRoutine(_animationCurve, _actionFloat));
            }
            
            _resumedListener.InvokeAll();
        }

        /// <summary>
        /// Прекращает действие корутины
        /// </summary>
        public void Stop()
        {
            if (_coroutine != null) _coroutineManager.StopCoroutine(_coroutine);
            Restart();
        }

        public float GetValueByTime(AnimationCurve animationCurve, float time, float timeFactor)
        {
            return animationCurve.Evaluate(time / timeFactor);
        }

        private void Restart()
        {
            _coroutine = null;
            _coroutineCurrentTime = 0;
            CurrentValue = 0;
            PreviousValue = 0;
            _action = null;
            _actionFloat = null;
        }
        
        private IEnumerator CurveRoutine(AnimationCurve animationCurve, Action action, float time = 1)
        {
            while (!_isPaused)
            {
                CurrentValue = animationCurve.Evaluate(_coroutineCurrentTime);
                action();
                yield return null;
                _coroutineCurrentTime += Time.deltaTime / time;
                PreviousValue = CurrentValue;

                if (animationCurve[animationCurve.length - 1].time < _coroutineCurrentTime)
                {
                    _coroutineCurrentTime = animationCurve[animationCurve.length - 1].time;
                    break;
                }
            }
            
            if (!_isPaused)
            {
                CurrentValue = animationCurve.Evaluate(_coroutineCurrentTime);
                action();
                _finishedListener.InvokeAll();
            }
        }

        private IEnumerator CurveRoutine(AnimationCurve animationCurve, Action<float> action, float time = 1)
        {
            while (!_isPaused)
            {
                CurrentValue = animationCurve.Evaluate(_coroutineCurrentTime);
                action(CurrentValue);
                yield return null;
                _coroutineCurrentTime += Time.deltaTime / time;
                PreviousValue = CurrentValue;

                if (animationCurve[animationCurve.length - 1].time < _coroutineCurrentTime)
                {
                    _coroutineCurrentTime = animationCurve[animationCurve.length - 1].time;
                    break;
                }
            }

            if (!_isPaused)
            {
                CurrentValue = animationCurve.Evaluate(_coroutineCurrentTime);
                action(CurrentValue);
                _finishedListener.InvokeAll();
            }
        }
    }
}