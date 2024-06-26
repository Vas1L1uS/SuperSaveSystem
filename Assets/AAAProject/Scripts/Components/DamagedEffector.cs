using AAAProject.Scripts.Extensions;
using UnityEngine;

namespace AAAProject.Scripts.Components
{
    public class DamagedEffector : MBehaviour
    {
        [SerializeField]private ParticleSystem _vfx;
        [SerializeField]private HealthSystem   _healthSystem;

        private void Awake()
        {
            _healthSystem.HealthChanged += HealthChanged;
        }

        private void HealthChanged(float delta)
        {
            _vfx.Play();
        }
    }
}
