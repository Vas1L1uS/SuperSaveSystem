using System;
using System.Collections;
using AAAProject.Scripts.Extensions.CoroutineScripts.CoroutineManagerService;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AAAProject.Scripts.Extensions.CoroutineScripts
{
    public class TimerCoroutine
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
        public float MaxTime { get; private set; }
        public float CurrentTime { get; private set; }
        public float PreviousTime { get; private set; }

        private readonly CoroutineManager _coroutineManager;
        private Coroutine _coroutine;
        private bool _isPaused;
        private Action _action;
        private Action<float> _actionFloat;
        private readonly DelegateListener<Action> _finishedListener = new();
        private readonly DelegateListener<Action> _pausedListener = new();
        private readonly DelegateListener<Action> _resumedListener = new();

        /// <summary>
        /// Создавать только в Awake()
        /// </summary>
        public TimerCoroutine()
        {
            _coroutineManager = CoroutineManager.GetInstance();
            SceneManager.sceneUnloaded += ctx => Stop();
            Finished += Restart;
        }
        
        public void Play(float time)
        {
            if (_coroutine != null) _coroutineManager.StopCoroutine(_coroutine);
            
            Restart();
            MaxTime = time;
            _coroutine = _coroutineManager.StartCoroutine(TimerRoutine());
        }
        
        public void Play(float time, Action action)
        {
            if (_coroutine != null) _coroutineManager.StopCoroutine(_coroutine);
            
            Restart();
            MaxTime = time;
            _action = action;
            _coroutine = _coroutineManager.StartCoroutine(TimerRoutine(_action));
        }
        
        public void Play(float time, Action<float> actionFloat)
        {
            if (_coroutine != null) Stop();
            
            Restart();
            MaxTime = time;
            _actionFloat = actionFloat;
            _coroutine = _coroutineManager.StartCoroutine(TimerRoutine(_actionFloat));
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
                _coroutine = _coroutineManager.StartCoroutine(TimerRoutine(_action));
            }
            else if (_actionFloat != null)
            {
                _coroutine = _coroutineManager.StartCoroutine(TimerRoutine(_actionFloat));
            }
            else
            {
                _coroutine = _coroutineManager.StartCoroutine(TimerRoutine());
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

        private void Restart()
        {
            _coroutine = null;
            MaxTime = 0;
            CurrentTime = 0;
            PreviousTime = 0;
            _action = null;
            _actionFloat = null;
        }
        
        private IEnumerator TimerRoutine()
        {
            if (MaxTime > 0)
            {
                while (!_isPaused)
                {
                    yield return null;

                    CurrentTime += Time.deltaTime;
                    PreviousTime = CurrentTime;

                    if (CurrentTime >= MaxTime)
                    {
                        CurrentTime = MaxTime;

                        break;
                    }
                }
            }

            if (!_isPaused)
            {
                yield return new WaitForEndOfFrame();
                _finishedListener.InvokeAll();
            }
        }
        
        private IEnumerator TimerRoutine(Action action)
        {
            if (MaxTime > 0)
            {
                while (!_isPaused)
                {
                    action();

                    yield return null;

                    CurrentTime += Time.deltaTime;
                    PreviousTime = CurrentTime;

                    if (CurrentTime >= MaxTime)
                    {
                        CurrentTime = MaxTime;

                        break;
                    }
                }
            }

            if (!_isPaused)
            {
                action();
                yield return new WaitForEndOfFrame();
                _finishedListener.InvokeAll();
            }
        }

        private IEnumerator TimerRoutine(Action<float> action)
        {
            if (MaxTime > 0)
            {
                while (!_isPaused)
                {
                    action(CurrentTime);

                    yield return null;

                    CurrentTime += Time.deltaTime;
                    PreviousTime = CurrentTime;

                    if (CurrentTime >= MaxTime)
                    {
                        CurrentTime = MaxTime;

                        break;
                    }
                }
            }

            if (!_isPaused)
            {
                action(CurrentTime);
                yield return new WaitForEndOfFrame();
                _finishedListener.InvokeAll();
            }
        }
    }
}