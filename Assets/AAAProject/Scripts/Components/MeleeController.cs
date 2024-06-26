using System.Collections.Generic;
using AAAProject.Scripts.Extensions;
using UnityEngine;

namespace AAAProject.Scripts.Components
{
    public class MeleeController : MBehaviour
    {
        public float CurrentReloadTime => _currentReloadTime;
        
        [SerializeField]private DamageSystem _damageSystem;
        [SerializeField]private LayerMask    _triggerLayer;
        [SerializeField]private float        _attackRadius = 2f;
        [SerializeField]private float        _reloadTime   = 1f;

        private float _currentReloadTime;

        public void Init(float currentReloadTime)
        {
            _currentReloadTime = currentReloadTime;
        }

        private void Update()
        {
            if (_currentReloadTime < _reloadTime)
            {
                _currentReloadTime += Time.deltaTime;
            }
            else
            {
                Collider[] triggers = Physics.OverlapSphere(this.transform.position, _attackRadius, _triggerLayer);

                if (triggers.Length > 0)
                {
                    List<HealthSystem> healths = new();

                    for (int i = 0; i < triggers.Length; i++)
                    {
                        if (triggers[i].TryGetComponent(out HealthSystem health))
                        {
                            healths.Add(health);
                        }
                    }

                    if (healths.Count > 0)
                    {
                        Attack(healths);
                        _currentReloadTime = 0;
                    }
                }
            }
        }

        private void Attack(List<HealthSystem> targets)
        {
            foreach (var target in targets)
            {
                _damageSystem.MakeDamage(target);
            }
        }
    }
}
