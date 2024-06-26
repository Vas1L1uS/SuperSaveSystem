using System;
using AAAProject.Scripts.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AAAProject.Scripts.Components
{
    public class DamageSystem : MBehaviour
    {
        public event Action<float> MaxDamageChanged
        {
            add => _maxDamageChangedListener.Subscribe(value);
            remove => _maxDamageChangedListener.Unsubscribe(value);
        }
        
        public event Action<float> MinDamageChanged
        {
            add => _minDamageChangedListener.Subscribe(value);
            remove => _minDamageChangedListener.Unsubscribe(value);
        }
        
        public event Action<float> Damaged
        {
            add => _damagedListener.Subscribe(value);
            remove => _damagedListener.Unsubscribe(value);
        }
        
        public float MaxDamage => _maxDamage;
        public float MinDamage => _minDamage;
        /// <summary>
        /// Во сколько макс.урон больше мин.урона
        /// </summary>
        public float DamageRangeFactor => _damageRangeFactor;
        public bool IsDead { get; protected set; }
        
        [Min(0)]
        [SerializeField] protected float _maxDamage = 12.5f;
        [Min(0)]
        [SerializeField] protected float _minDamage = 10;
        [Min(1)]
        [Tooltip("Во сколько макс.урон больше мин.урона")]
        [SerializeField] protected float _damageRangeFactor = 1.25f;
        
        private readonly DelegateListener<Action<float>> _maxDamageChangedListener = new();
        private readonly DelegateListener<Action<float>> _minDamageChangedListener = new();
        private readonly DelegateListener<Action<float>> _damagedListener = new();
        
        /// <summary>
        /// Прибавить или убавить макс. урон
        /// </summary>
        /// <param name="addMaxDamage">На сколько прибавить</param>
        public virtual void ChangeMaxDamage(float addMaxDamage)
        {
            var previousMaxDamage = _maxDamage;
            _maxDamage += addMaxDamage;
            
            if (_maxDamage < 0)
            {
                _maxDamage = 0;
            }
            
            _maxDamageChangedListener.InvokeAll(_maxDamage - previousMaxDamage);
        }
        
        /// <summary>
        /// Установить новое значение макс. урона
        /// </summary>
        /// <param name="maxDamage">Новое значение</param>
        public virtual void SetMaxDamage(float maxDamage)
        {
            var previousMaxDamage = _maxDamage;
            _maxDamage = maxDamage;
            
            if (_maxDamage < 0)
            {
                _maxDamage = 0;
            }
            
            _maxDamageChangedListener.InvokeAll(_maxDamage - previousMaxDamage);
        }

        /// <summary>
        /// Прибавить или убавить мин. урон
        /// </summary>
        /// <param name="addMinDamage">На сколько прибавить</param>
        public virtual void ChangeMinDamage(float addMinDamage)
        {
            var previousMinDamage = _minDamage;
            _minDamage += addMinDamage;
            
            if (_minDamage < 0)
            {
                _minDamage = 0;
            }
            else if (_minDamage > _maxDamage)
            {
                _minDamage = _maxDamage;
            }
            
            _maxDamageChangedListener.InvokeAll(_minDamage - previousMinDamage);
        }
        
        /// <summary>
        /// Установить новое значение макс. урона
        /// </summary>
        /// <param name="minDamage">Новое значение</param>
        public virtual void SetMinDamage(float minDamage)
        {
            var previousMinDamage = _minDamage;
            _minDamage = minDamage;
            
            if (_minDamage < 0)
            {
                _minDamage = 0;
            }
            else if (_minDamage > _maxDamage)
            {
                _minDamage = _maxDamage;
            }
            
            _maxDamageChangedListener.InvokeAll(_minDamage - previousMinDamage);
        }
        
        /// <summary>
        /// Прибавить или убавить мин. урон и применить диапазон для макс. урона
        /// </summary>
        /// <param name="damage">На сколько прибавить</param>
        public virtual void ChangeDamageWithRangeFactor(float damage)
        {
            ChangeMinDamage(damage);
            SetMaxDamage(_minDamage * _damageRangeFactor);
        }

        /// <summary>
        /// Установить новое значение мин. урона и применить диапазон для макс. урона
        /// </summary>
        /// <param name="damage"></param>
        public virtual void SetDamageWithRangeFactor(float damage)
        {
            SetMaxDamage(damage * _damageRangeFactor);
            SetMinDamage(damage);
        }

        /// <summary>
        /// Установить новый диапазон урона
        /// </summary>
        /// <param name="damageRangeFactor">Диапазон</param>
        public virtual void SetDamageRangeFactor(float damageRangeFactor)
        {
            _damageRangeFactor = damageRangeFactor;

            if (_damageRangeFactor < 1)
            {
                _damageRangeFactor = 1;
            }
            
            SetMaxDamage(_minDamage * _damageRangeFactor);
        }

        /// <summary>
        /// Вызывается из инспектора для применения диапазона урона
        /// </summary>
        [ContextMenu("ApplyDamageRangeFactor")]
        public void ApplyDamageRangeFactor()
        {
            SetMaxDamage(_minDamage * _damageRangeFactor);
        }

        /// <summary>
        /// Нанести урон
        /// </summary>
        /// <param name="healthSystem">Цель</param>
        public void MakeDamage(HealthSystem healthSystem)
        {
            var damage = Random.Range(_minDamage, _maxDamage);
            healthSystem.ChangeHealth(-damage);
            _damagedListener.InvokeAll(damage);
        }
    }
}