using UnityEngine;

namespace AAAProject.CharacterMovement.Scripts
{
    public class AnimMoveController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private string _speedParamName = "Speed";
        [SerializeField] private float _baseSpeed = 3;
        
        public void SetSpeed(float speed)
        {
            var speedFactor = speed / _baseSpeed;
            _animator.SetFloat(_speedParamName, speedFactor);
        }
    }
}