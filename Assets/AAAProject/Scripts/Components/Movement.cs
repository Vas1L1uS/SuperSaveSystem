using UnityEngine;

namespace AAAProject.Scripts.Components
{
    public class Movement : MonoBehaviour
    {
        public Vector3 Direction { get; private set; }
        
        public void SetDirection(Vector3 direction)
        {
            Direction = direction;
        }

        private void Update()
        {
            if (Direction != Vector3.zero)
            {
                this.transform.Translate(Direction * Time.deltaTime);
            }
        }
    }
}