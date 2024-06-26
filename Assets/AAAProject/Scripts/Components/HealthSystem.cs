using System;
using AAAProject.Scripts.Extensions;
using UnityEngine;

namespace AAAProject.Scripts.Components
{
    public class HealthSystem : MBehaviour
    {
        public event Action<float> HealthChanged
        {
            add => _healthChangedListener.Subscribe(value);
            remove => _healthChangedListener.Unsubscribe(value);
        }
        
        public event Action<float> MaxHealthChanged
        {
            add => _healthChangedListener.Subscribe(value);
            remove => _healthChangedListener.Unsubscribe(value);
        }
        
        public event Action<float> MinHealthChanged
        {
            add => _healthChangedListener.Subscribe(value);
            remove => _healthChangedListener.Unsubscribe(value);
        }
        
        public event Action Died
        {
            add => _diedListener.Subscribe(value);
            remove => _diedListener.Unsubscribe(value);
        }
        
        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;
        public float MinHealth => _minHealth;
        public bool IsDead { get; protected set; }

        [Min(0)]
        [SerializeField] protected float _currentHealth = 100;
        [Min(0)]
        [SerializeField] protected float _maxHealth = 100;
        [Min(0)]
        [SerializeField] protected float _minHealth = 0;

        private readonly DelegateListener<Action<float>> _healthChangedListener = new();
        private readonly DelegateListener<Action<float>> _maxHealthChangedListener = new();
        private readonly DelegateListener<Action<float>> _minHealthChangedListener = new();
        private readonly DelegateListener<Action> _diedListener = new();

        /// <summary>
        /// Добавить или убавить здоровье
        /// </summary>
        /// <param name="addHealth">На сколько добавить</param>
        public virtual void ChangeHealth(float addHealth)
        {
            if (addHealth == 0) return;

            var previousHealth = _currentHealth;
            _currentHealth += addHealth;

            if (_currentHealth > _maxHealth)
            {
                _currentHealth = _maxHealth;
            }
            else if (_currentHealth <= _minHealth)
            {
                _currentHealth = _minHealth;
            }
            
            if (_currentHealth == previousHealth) return;
            
            _healthChangedListener.InvokeAll(_currentHealth - previousHealth);

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                _diedListener.InvokeAll();
            }
        }

        /// <summary>
        /// Установить новое значение здоровья
        /// </summary>
        /// <param name="health">Новое значение</param>
        public virtual void SetHealth(float health)
        {
            if (_currentHealth == health) return;

            var previousHealth = _currentHealth;
            _currentHealth = health;

            if (_currentHealth > _maxHealth)
            {
                _currentHealth = _maxHealth;
            }
            else if (_currentHealth <= _minHealth)
            {
                _currentHealth = _minHealth;
            }
            
            _healthChangedListener.InvokeAll(_currentHealth - previousHealth);

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                _diedListener.InvokeAll();
            }
        }

        /// <summary>
        /// Прибавить или убавить макс. здоровье
        /// </summary>
        /// <param name="addMaxHealth">На сколько прибавить</param>
        public virtual void ChangeMaxHealth(float addMaxHealth)
        {
            var previousMaxHealth = _maxHealth;
            _maxHealth += addMaxHealth;
            
            if (_maxHealth < 0)
            {
                _maxHealth = 0;
            }
            
            _minHealthChangedListener.InvokeAll(_maxHealth - previousMaxHealth);
        }
        
        /// <summary>
        /// Установить новое значение макс. здоровья
        /// </summary>
        /// <param name="maxHealth">Новое значение</param>
        public virtual void SetMaxHealth(float maxHealth)
        {
            var previousMaxHealth = _maxHealth;
            _maxHealth = maxHealth;
            
            if (_maxHealth < 0)
            {
                _maxHealth = 0;
            }
            
            _minHealthChangedListener.InvokeAll(_maxHealth - previousMaxHealth);
        }

        /// <summary>
        /// Прибавить или убавить макс. здоровье
        /// </summary>
        /// <param name="addMinHealth">На сколько прибавить</param>
        public virtual void ChangeMinHealth(float addMinHealth)
        {
            var previousMinHealth = _maxHealth;
            _minHealth += addMinHealth;
            
            if (_minHealth < 0)
            {
                _minHealth = 0;
            }
            else if (_minHealth > _maxHealth)
            {
                _minHealth = _maxHealth;
            }
            
            _minHealthChangedListener.InvokeAll(_minHealth - previousMinHealth);
        }
        
        /// <summary>
        /// Установить новое значение мин. здоровья
        /// </summary>
        /// <param name="minHealth">Новое значение</param>
        public virtual void SetMinHealth(float minHealth)
        {
            var previousMinHealth = _maxHealth;
            _minHealth = minHealth;

            if (_minHealth < 0)
            {
                _minHealth = 0;
            }
            else if (_minHealth > _maxHealth)
            {
                _minHealth = _maxHealth;
            }
            
            _minHealthChangedListener.InvokeAll(_minHealth - previousMinHealth);
        }
    }
}